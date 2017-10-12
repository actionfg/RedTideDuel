using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

// 继续游戏功能
public class ContinueGame : MonoBehaviour
{

    public void Start()
    {
//        if (!File.Exists(Application.persistentDataPath + ResumeGameIO.RESUME_DATA))
//        {
//            gameObject.SetActive(false);
//        }
    }

    public void OnContinueClick()
    {
//        var path = Application.persistentDataPath + ResumeGameIO.RESUME_DATA;
//        if (File.Exists(path))
//        {
//            var bf = new BinaryFormatter();
//            var fileStream = File.Open(path, FileMode.Open);
//
//            GameContext.LoadGameData(bf, fileStream);
//
//            fileStream.Close();
//            File.Delete(path);
//
//            // 加载特定场景
//            SceneManager.LoadScene("main");
//        }

        var cloth = GameObject.FindObjectOfType<Cloth>();
        if (cloth != null)
        {
            Destroy(cloth.gameObject);    
        }
        SceneManager.LoadScene("main");
    }

}