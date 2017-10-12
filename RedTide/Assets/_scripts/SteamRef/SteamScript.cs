using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

// 有关steam操作整合至此
public class SteamScript : MonoBehaviour
{
    const string MESSAGE_FILE_NAME = "test.dat";
    private static SteamScript s_instance;
    public static SteamScript Instance {
        get {
            if (s_instance == null) {
                return new GameObject("SteamScript").AddComponent<SteamScript>();
            }
            else {
                return s_instance;
            }
        }
    }

    protected Callback<GameOverlayActivated_t> _gameOverlayActivated;
    private CallResult<NumberOfCurrentPlayers_t> _numOfCurrentPlayers;
    private CallResult<RemoteStorageFileWriteAsyncComplete_t> OnRemoteStorageFileWriteAsyncCompleteCallResult;
    private CallResult<RemoteStorageFileReadAsyncComplete_t> OnRemoteStorageFileReadAsyncCompleteCallResult;

    // 远端保存和读取Callback
    public delegate void OnRemoteSaveDelegate(bool success);
    public delegate void OnRemoteLoadDelegate(SteamAPICall_t callT, bool success, byte[] data, uint fileOffset, uint size);

    private event OnRemoteSaveDelegate OnRemoteSave;
    private event OnRemoteLoadDelegate OnRemoteLoad;

    private Queue<string> _loadingQueue;                            // 加载队列, 确保Async文件读取完之后才进行下一次读取
    private Dictionary<string, SteamAPICall_t> _loadingCallT;       // 异步加载对应的CallT
    private bool _loadingComplete = false;

    public SteamAPICall_t GetLoadedCallT(string file)
    {
        SteamAPICall_t rst;
        if (_loadingCallT.TryGetValue(file, out rst))
        {
            return rst;
        }
        return SteamAPICall_t.Invalid;
    }

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
        else if (s_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        _loadingQueue = new Queue<string>();
        _loadingCallT = new Dictionary<string, SteamAPICall_t>();
    }

    private void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            _gameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActiated);
            _numOfCurrentPlayers = CallResult<NumberOfCurrentPlayers_t>.Create(OnNumberCurrentPlayers);

            OnRemoteStorageFileWriteAsyncCompleteCallResult =
                CallResult<RemoteStorageFileWriteAsyncComplete_t>.Create(OnRemoteStorageFileWriteAsyncComplete);
            OnRemoteStorageFileReadAsyncCompleteCallResult =
                CallResult<RemoteStorageFileReadAsyncComplete_t>.Create(OnRemoteStorageFileReadAsyncComplete);

            OnRemoteSave += GameContext.OnSteamDataSaved;
            OnRemoteLoad += GameContext.OnSteamDataLoaded;
        }
    }

    private void OnNumberCurrentPlayers(NumberOfCurrentPlayers_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_bSuccess != 1 || bIOFailure)
        {
            Debug.Log("There was an error retrieving the NumberOfCurrentPlayers.");
        }
        else
        {
            Debug.Log("The number of players playing your game: " + pCallback.m_cPlayers);
        }
    }

    private void OnGameOverlayActiated(GameOverlayActivated_t callback)
    {
        if (callback.m_bActive != 0)
        {
            Debug.Log("Steam Overlay has been activated");
        }
        else
        {
            Debug.Log("Steam Overlay has been closed");
        }
    }

    // Use this for initialization
    void Start()
    {
        if (!SteamManager.Initialized)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (_loadingQueue.Count >0 && _loadingComplete)
        {
            // 加载下一个文件
            var nextFile = _loadingQueue.Peek();
            BeginLoadAsync(nextFile);
            _loadingComplete = false;
        }
    }

//    private void OnGUI()
//    {
//        if (GUILayout.Button("FileWriteAsync(MESSAGE_FILE_NAME, Data, (uint)Data.Length)"))
//        {
//            // 测试SteamUserStats
////            SteamAPICall_t handle = SteamUserStats.GetNumberOfCurrentPlayers();
////            _numOfCurrentPlayers.Set(handle);
////            Debug.Log("Called GetNumberOfCurrentPlayers()");
//
//            var stringContent = "Test remote storage, content is empty!!!!!!!!!!!!!!!!TestTestTestTestTestTestTestTestTestTestTestTest";
//            byte[] data = new byte[System.Text.Encoding.UTF8.GetByteCount(stringContent)];
//            System.Text.Encoding.UTF8.GetBytes(stringContent, 0, stringContent.Length, data, 0);
//            RemoteSave(MESSAGE_FILE_NAME, data, null);
//        }

//        if (GUILayout.Button("FileReadAsync(MESSAGE_FILE_NAME, Data, (uint)Data.Length)"))
//        {
//            RemoteLoadAsync(GameContext.RESUME_DATA);
//        }
//    }

    public SteamAPICall_t RemoteSaveAsync(string fileName, byte[] data)
    {
        SteamAPICall_t handle = SteamRemoteStorage.FileWriteAsync(fileName, data, (uint) data.Length);
        OnRemoteStorageFileWriteAsyncCompleteCallResult.Set(handle);
        //Debug.Log("SteamRemoteStorage.FileWriteAsync(" + fileName + ", dataSize: " + (uint) data.Length + ")");
        return handle;
    }

    // 查看每个用户云存储的最大容量
    private static void CheckRemoteStorageQuota()
    {
        ulong AvailableBytes, m_TotalBytes;
        bool ret = SteamRemoteStorage.GetQuota(out m_TotalBytes, out AvailableBytes);
        Debug.Log("MaxBytes: " + m_TotalBytes + " availableBytes: " + AvailableBytes);
    }

    public void AddRemoteLoadAsncTask(string fileName)
    {
        // 加入队列机制, CallHandler只在调用实际代码时才获取
        if (_loadingQueue.Count == 0)
        {
            BeginLoadAsync(fileName);
        }
        _loadingQueue.Enqueue(fileName);
    }

    private SteamAPICall_t BeginLoadAsync(string fileName)
    {
        var fileSize = SteamRemoteStorage.GetFileSize(fileName);
        if (fileSize <= 0)
        {
            Debug.LogWarning("steam file " + fileName + " content is empty!");
            return SteamAPICall_t.Invalid;
        }
        SteamAPICall_t handle = SteamRemoteStorage.FileReadAsync(fileName, 0, (uint) fileSize);
        OnRemoteStorageFileReadAsyncCompleteCallResult.Set(handle);
        Debug.Log("steam load: "+ fileName + " call: " + handle + " fileSzie: " + fileSize);
        _loadingCallT[fileName] = handle;
        return handle;
    }

    void OnRemoteStorageFileWriteAsyncComplete(RemoteStorageFileWriteAsyncComplete_t pCallback, bool bIOFailure)
    {
        //Debug.Log("[" + RemoteStorageFileWriteAsyncComplete_t.k_iCallback + " - RemoteStorageFileWriteAsyncComplete] - " + pCallback.m_eResult);
        if (OnRemoteSave != null)
        {
            OnRemoteSave(pCallback.m_eResult == EResult.k_EResultOK);
        }
    }

    void OnRemoteStorageFileReadAsyncComplete(RemoteStorageFileReadAsyncComplete_t pCallback, bool bIOFailure)
    {
        Debug.Log("[" + RemoteStorageFileReadAsyncComplete_t.k_iCallback + " - RemoteStorageFileReadAsyncComplete] - " +
                  pCallback.m_hFileReadAsync + " -- " + pCallback.m_eResult + " -- " + pCallback.m_nOffset + " -- " +
                  pCallback.m_cubRead);

        if (pCallback.m_eResult == EResult.k_EResultOK)
        {
            byte[] data = new byte[pCallback.m_cubRead + 1];
            bool ret = SteamRemoteStorage.FileReadAsyncComplete(pCallback.m_hFileReadAsync, data, pCallback.m_cubRead);
            if (OnRemoteLoad != null)
            {
                OnRemoteLoad(pCallback.m_hFileReadAsync, ret, data, pCallback.m_nOffset, pCallback.m_cubRead);
            }
        }
        _loadingQueue.Dequeue();
        _loadingComplete = true;
    }

    public bool FileExists(string filePath)
    {
        return SteamRemoteStorage.FileExists(filePath);
    }

    public bool DeleteFile(string filePath)
    {
        return SteamRemoteStorage.FileDelete(filePath);
    }
}