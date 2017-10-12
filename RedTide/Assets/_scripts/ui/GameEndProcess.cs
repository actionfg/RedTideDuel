using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// 游戏通关或者失败时的处理
public class GameEndProcess : MonoBehaviour
{
    public static GameEndProcess Process;

    public GameObject panel;

    void Awake()
    {
        if (Process == null)
        {
            Process = this;
        }
        else if (Process != this)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnClick()
    {
//        // TODO 坟墓数据的填充
//
//        // TODO TalentUnlockManager的保存放在解锁的瞬间, 如技能的拾取?
//        GameContext.DeleteSavedPosition();
//        File.Delete(Application.persistentDataPath + "/gold.gd");
//        File.Delete(Application.persistentDataPath + "/rc.gd");
//
//        GameContext.talentData.SetLastGameFragments(GameContext.GoldData.GetTalentFragments());
//        GameContext.SaveTalentData();
////        #if UNITY_EDITOR
//            StatisticManager.GetInstance().Save();
////        #endif
//
//        GameContext.DeleteSavedGameData();
//        GameContext.ConfigInfo = null;
//
//        var objs = GameObject.FindObjectsOfType<GameObject>();
//        foreach (var o in objs)
//        {
//            if (o != Camera.main.gameObject)
//            {
//                Destroy(o);
//            }
//        }
//
//        GameContext.Success = false;
//        SceneManager.LoadScene("Menu UI");
    }

    public void SetActive(bool active)
    {
//        if (active)
//        {
//            GameContext.stage = -1;
//        }
        if (panel)
        {
            panel.SetActive(active);
        }
    }
}