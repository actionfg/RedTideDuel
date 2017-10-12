using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// BlackScreen淡出(从1 - 0耗时0.3s),
// 加载完成后, blackScreen淡入至Alpha1
// LoadingProgress消失, blackScreen淡出

// 用于显示加载界面, 后期可能加入一些剧情
public class IntermissionControl : MonoBehaviour
{
    public const float ASTAR_PERCENT = 0.2f; // 暂定生成NAVMesh, 占80%
    public const float STEP_PERCENT = ASTAR_PERCENT / 5f;
    public static IntermissionControl intermissionControl;

    public enum IntermissionState
    {
        None,
        Emerge,        // Black + Loading出现
        Stay,          // 等待Loading结束
        Vanish,        // Loading + Black消失
    }

    private float _decaySpeed = 2f; // 1/_decaySpeed秒后重新变为透明, 后期可能需要根据后面场景加载时间决定
    private float _totalPercent;
    private IntermissionState _state = IntermissionState.None;
//    private RuntimeDungeon _gen;
//    private NavMeshAdapter _navMeshAdapter;

    public GameObject BlackScreen;
    public GameObject LoadingProgress;
    private bool _canVanish = false;

    public delegate void LoadChapterCompleteDelegate();

    public event LoadChapterCompleteDelegate LoadChapterComplete;
    
    void Awake()
    {
        if (intermissionControl == null)
        {
            intermissionControl = this;
        }
        else if (intermissionControl != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        if (BlackScreen == null || LoadingProgress == null)
        {
            gameObject.SetActive(false);
        }
        CheckProgressSource();
    }

    private void CheckProgressSource()
    {
//        if (GameContext.ChapterManager)
//        {
//            _gen = GameContext.ChapterManager.RuntimeDungeon;
//            if (_gen)
//                _gen.Generator.OnGenerationStatusChanged += OnGenerationStatusChanged;
//        }
//
//        _navMeshAdapter = GameObject.FindObjectOfType<NavMeshAdapter>();
//        if (_navMeshAdapter)
//        {
//            _navMeshAdapter.OnProgress += OnAstarProgress;
//        }
    }

    // Update is called once per frame
    void Update()
    {
//        if (_gen == null || _navMeshAdapter == null)
//        {
//            CheckProgressSource();
//        }

        if (_totalPercent > 0.95f && _canVanish && _state == IntermissionState.Stay)
        {
            _state = IntermissionState.Vanish;

            var alphaCtrl = BlackScreen.GetComponent<ImageAlphaControl>();
            alphaCtrl.SetEmerge(true, _decaySpeed);
        }
    }

    private void ChangeTextContext(Text text)
    {
        text.text = ((int) (_totalPercent * 100)).ToString();
    }

    public void SetEnable(bool enable)
    {
        if (enable)
        {
            _totalPercent = 0f;
            ChangeProgressText();
            _canVanish = false;
            _state = IntermissionState.Emerge;
            var alphaCtrl = BlackScreen.GetComponent<ImageAlphaControl>();
            if (alphaCtrl)
            {
                alphaCtrl.SetEmerge(true, _decaySpeed);
                alphaCtrl.OnAlphaChange -= OnBlackScreenChange;
                alphaCtrl.OnAlphaChange += OnBlackScreenChange;
            }
            // blackScreen淡入, 到alpha1时,loadingProgress出现
            LoadingProgress.SetActive(false);
            Time.timeScale = 0f;
        }
    }

    protected virtual void OnBlackScreenChange(float alpha)
    {
        if (_state == IntermissionState.Emerge && alpha >= 1f)
        {
            LoadingProgress.SetActive(true);
            var alphaCtrl = BlackScreen.GetComponent<ImageAlphaControl>();
            alphaCtrl.SetEmerge(false, _decaySpeed);

            _state = IntermissionState.Stay;
        }
        else if (_state == IntermissionState.Stay && alpha <= 0f)
        {
            _canVanish = true;
        }
        else if (_state == IntermissionState.Vanish && alpha >= 1f)
        {
            LoadingProgress.SetActive(false);
            var alphaCtrl = BlackScreen.GetComponent<ImageAlphaControl>();
            alphaCtrl.SetEmerge(false, _decaySpeed);
            StartCoroutine(ResetTimeScale());
        }
    }

    private IEnumerator ResetTimeScale()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        // 检查特性或者开局剧情等
        if (LoadChapterComplete != null)
        {
            LoadChapterComplete();
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

//    protected virtual void OnGenerationStatusChanged(BaseDungeonGenerator generator, GenerationStatus status)
//    {
//        switch (status)
//        {
//            case GenerationStatus.NotStarted:
//                _totalPercent = 0f;
//                break;
//            case GenerationStatus.PreProcessing:
//                _totalPercent = STEP_PERCENT;
//                break;
//            case GenerationStatus.TileInjection:
//                break;
//            case GenerationStatus.MainPath:
//                _totalPercent = 2f * STEP_PERCENT;
//                break;
//            case GenerationStatus.Branching:
//                _totalPercent = 3f * STEP_PERCENT;
//                break;
//            case GenerationStatus.PostProcessing:
//                _totalPercent = 4f * STEP_PERCENT;
//                break;
//            case GenerationStatus.Complete:
//                _totalPercent = Math.Max(5f * STEP_PERCENT, _totalPercent);
////                LogGenerationTime(generator);
//                break;
//            case GenerationStatus.Failed:
//                _totalPercent = 0;
//                break;
//            default:
//                Debug.LogWarning(" Have not define " + status);
//                break;
//        }
//        ChangeProgressText();
////        Debug.Log("onDungeonGenerate progress: " + _totalPercent);
//    }

    private void ChangeProgressText()
    {
        if (LoadingProgress)
        {
            for (int i = 0; i < LoadingProgress.transform.childCount; i++)
            {
                var child = LoadingProgress.transform.GetChild(i);
                var childText = child.GetComponent<Text>();
                if (childText)
                {
                    ChangeTextContext(childText);
                    break;
                }
            }
        }
    }

//    public void OnAstarProgress(NavMeshAdapter.NavMeshGenerationProgress progress)
//    {
//        _totalPercent = ASTAR_PERCENT + progress.Percentage * (1 - ASTAR_PERCENT);
//        ChangeProgressText();
//    }

//    private void LogGenerationTime(BaseDungeonGenerator generator)
//    {
//        StringBuilder infoText = new StringBuilder();
//
//        infoText.Length = 0;
//        infoText.AppendLine("Seed: " + generator.ChosenSeed);
//        infoText.AppendLine();
//        infoText.Append("## TIME TAKEN ##");
//        infoText.AppendFormat("\n\tPre-Processing:\t\t{0:0.00} ms", generator.GenerationStats.PreProcessTime);
//        infoText.AppendFormat("\n\tMain Path Generation:\t{0:0.00} ms",
//            generator.GenerationStats.MainPathGenerationTime);
//        infoText.AppendFormat("\n\tBranch Path Generation:\t{0:0.00} ms",
//            generator.GenerationStats.BranchPathGenerationTime);
//        infoText.AppendFormat("\n\tPost-Processing:\t\t{0:0.00} ms", generator.GenerationStats.PostProcessTime);
//        infoText.Append("\n\t-------------------------------------------------------");
//        infoText.AppendFormat("\n\tTotal:\t\t\t{0:0.00} ms", generator.GenerationStats.TotalTime);
//
//        infoText.AppendLine();
//        infoText.AppendLine();
//
//        infoText.AppendLine("## ROOM COUNT ##");
//        infoText.AppendFormat("\n\tMain Path: {0}", generator.GenerationStats.MainPathRoomCount);
//        infoText.AppendFormat("\n\tBranch Paths: {0}", generator.GenerationStats.BranchPathRoomCount);
//        infoText.Append("\n\t-------------------");
//        infoText.AppendFormat("\n\tTotal: {0}", generator.GenerationStats.TotalRoomCount);
//
//        infoText.AppendLine();
//        infoText.AppendLine();
//
//        infoText.AppendFormat("Retry Count: {0}", generator.GenerationStats.TotalRetries);
//
//        infoText.AppendLine();
//
//        Debug.Log(infoText.ToString());
//    }
}