﻿using System;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
 using Lib.SimpleJSON;
 using Steamworks;

public class GameContext
{

    private static readonly string RESUME_DATA = "rumGame.gd";
    private static string RESUME_DATA_PATH
    {
        get { return Application.persistentDataPath + "/" + RESUME_DATA; }
    }

    private static readonly string EXTRA_DATA = "extraData.gd";
    private static string EXTRA_DATA_PATH {
        get { return Application.persistentDataPath + "/" + EXTRA_DATA; }
    }

    public delegate void OnResumeDataLoadedDelegate(bool success);

    public static event  OnResumeDataLoadedDelegate OnResumeDataLoaded;


    public static int character = 2;
    public static int mobLevel = 6;

    private static GoldData _GoldData;

    public static GoldData GoldData
    {
        get
        {
            if (_GoldData == null)
            {
                _GoldData = new GoldData();
            }
            return _GoldData;
        }
    }

    public static int stage = 0; // 第几章
    public static int chapter = 0; // 第几节

    public static string LocalName = "zh-CN";

    public static float GlobalImpulseFactor = 1f;
    public static UnitManager UnitManager { get; set; }
    public static FloatingCombatPopupManager FloatingCombatPopupManager { get; set; }

    public static bool Success = false; // 是否通关

    public static int unitId = 0;
    public static int MessageCount = 0;

    public static float ResumeHp;                 // 上次存档时的生命值
    public static float ResumeMp;                 // 上次存档时的魔法值
    public static BasicAttribute ResumeAttribute; // 上次存档数据: 角色属性

    public static void SaveGameData()
    {
        Debug.Log("Save Resume data...");
        // 保存GameContext内容;
        // 保存主角状态和当前关卡数

        // 用Json格式保存
        var rootNode = new JSONClass();
        rootNode.Add("GoldData", GoldData.SaveToJSON());
        rootNode.Add("stage", new JSONData(stage));
        rootNode.Add("chapter", new JSONData(chapter));

        if (SteamManager.Initialized)
        {
            // 上传至Steam服务器中
            string stringContent = rootNode.ToString();
            byte[] data = new byte[System.Text.Encoding.UTF8.GetByteCount(stringContent)];
            System.Text.Encoding.UTF8.GetBytes(stringContent, 0, stringContent.Length, data, 0);

            SteamScript.Instance.RemoteSaveAsync(RESUME_DATA, data);
        }
        else
        {
            // 存在本地
            var stream = File.Create(RESUME_DATA_PATH);
            rootNode.SaveToStream(stream);
            stream.Close();
        }
    }

    public static void LoadResumeData()
    {
        if (!ResumeDataExists())
        {
            return;
        }

        Debug.Log("Load resume data...");
        if (SteamManager.Initialized)
        {
            // 从steam服务器加载
            SteamScript.Instance.AddRemoteLoadAsncTask(RESUME_DATA);
        }
        else
        {// 本地加载
            if (File.Exists(RESUME_DATA_PATH))
            {

                var stream = File.Open(RESUME_DATA_PATH, FileMode.Open);

                try
                {
                    var node = JSONNode.LoadFromStream(stream);
                    DoLoadResumeData(node);

                    stream.Close();
                }
                catch (Exception e)
                {
                    Debug.Log("Saved GameData is corrupted. " + e.Message);
                    stream.Close();
                    Reset();
                    DeleteSavedGameData();
                }
            }
            else
            {
                Reset();
            }
        }
    }

    public static void OnSteamDataLoaded(SteamAPICall_t callT, bool success, byte[] data, uint fileOffset, uint size)
    {
        if (success)
        {
            var content = System.Text.Encoding.UTF8.GetString(data, (int) fileOffset, (int) size);
            var node = JSONNode.Parse(content);
            // 根据CallT判断是什么内容
            if (SteamScript.Instance.GetLoadedCallT(RESUME_DATA) == callT)
            {

                DoLoadResumeData(node);
                if (OnResumeDataLoaded != null)
                {
                    OnResumeDataLoaded(true);
                }
            }
            else if (SteamScript.Instance.GetLoadedCallT(TalentData.FileName) == callT)
            {
                // TODO 加载实际内容
                var talentNode = node["Talent"];
//                talentData = new TalentData();
//                talentData.ReadFromJSON(talentNode);
            }
        }
        else
        {
            Debug.LogWarning("Can't load resume data from steam server!");
        }
    }

    private static void DoLoadResumeData(JSONNode node)
    {
//        var configInfoNode = node["ConfigInfo"];
//        ConfigInfo = new PlayerConfigInfo();
//        ConfigInfo.ReadFromJSON(configInfoNode);
//        GoldData.ReadFromJSON(node["GoldData"]);
//        stage = node["stage"].AsInt;
//        chapter = node["chapter"].AsInt;
//        Difficulty = (Difficulty) node["Difficulty"].AsInt;
//
//        var playerNode = node["Player"];
//        ResumeHp = playerNode["CurrentHp"].AsFloat;
//        ResumeMp = playerNode["CurrentMp"].AsFloat;
//        ResumeAttribute = new BasicAttribute();
//        ResumeAttribute.ReadFromJSON(playerNode["BaseAttribute"]);
//        ResumeWeaponId = playerNode["WeaponId"].AsInt;
//        ResumeWeaponAttribute = new EquipmentAttribute();
//        ResumeWeaponAttribute.Skill = ConfigInfo.WeaponSkill;
//        ResumeWeaponAttribute.ReadFromJSON(playerNode["WeaponAttribute"]);
//        var helmIdNode = playerNode["HelmId"];
//        if (helmIdNode != null)
//        {
//            ResumeHelmId = helmIdNode.AsInt;
//            ResumeHelmAttribute = new EquipmentAttribute();
//            ResumeHelmAttribute.ReadFromJSON(playerNode["HelmAttribute"]);
//        }
//        var cloakIdNode = playerNode["CloakId"];
//        if (cloakIdNode != null)
//        {
//            ResumeCloakId = cloakIdNode.AsInt;
//            ResumeCloakAttribute = new EquipmentAttribute();
//            ResumeCloakAttribute.ReadFromJSON(playerNode["CloakAttribute"]);
//        }
    }

    public static void OnSteamDataSaved(bool success)
    {
        if (!success)
        {
            Debug.LogWarning("Can't save data on steam server!");
        }
    }

    public static void DeleteSavedGameData()
    {
        Debug.Log("Deleting saved GameData...");
        if (SteamManager.Initialized)
        {
            SteamScript.Instance.DeleteFile(RESUME_DATA);
        }
        else
        {
            File.Delete(RESUME_DATA_PATH);
        }
    }

    public static void Reset()
    {
        Debug.Log("Resetting GameData...");
        stage = 0;
        chapter = 0;
        Success = false;
        unitId = 0;
        ResumeHp = 0f;
        ResumeMp = 0f;
        ResumeAttribute = null;
        _GoldData = new GoldData();
    }

//    public static void LoadTalentData()
//    {
//        talentData = new TalentData();
//        if (!TalentDataExists())
//        {
//            Debug.Log("Talent data file not exist");
//            return;
//        }
//
//        try
//        {
//            if (SteamManager.Initialized)
//            {
//                // 用Steam加载天赋数据
//                SteamScript.Instance.AddRemoteLoadAsncTask(TalentData.FileName);
//            }
//            else
//            {
//                var fileStream = File.Open(TalentData.SavePath, FileMode.Open);
//                var node = JSONNode.LoadFromStream(fileStream);
//                var talentNode = node["Talent"];
//                talentData.ReadFromJSON(talentNode);
//                fileStream.Close();
//
//            }
//        }
//        catch (Exception e)
//        {
//            Debug.Log("Talent Data corrupted, recreated, " + e.Message);
//            talentData = new TalentData();
//        }
//    }

//    public static void SaveTalentData()
//    {
//        var rootNode = new JSONClass();
//        rootNode.Add("Talent", talentData.SaveToJSON());
//
//        if (SteamManager.Initialized)
//        {
//            // 上传至Steam服务器中
//            string stringContent = rootNode.ToString();
//            byte[] data = new byte[System.Text.Encoding.UTF8.GetByteCount(stringContent)];
//            System.Text.Encoding.UTF8.GetBytes(stringContent, 0, stringContent.Length, data, 0);
//
//            SteamScript.Instance.RemoteSaveAsync(TalentData.FileName, data);
//        }
//        else
//        {
//            // 存在本地
//            var stream = File.Create(TalentData.SavePath);
//            rootNode.SaveToStream(stream);
//            stream.Close();
//        }
//    }

    public static bool ResumeDataExists()
    {
        if (SteamManager.Initialized)
        {
            return SteamScript.Instance.FileExists(RESUME_DATA);
        }
        else
        {
            return (File.Exists(RESUME_DATA_PATH));
        }
    }

    private static bool TalentDataExists()
    {
        if (SteamManager.Initialized)
        {
            return SteamScript.Instance.FileExists(TalentData.FileName);
        }
        else
        {
            return File.Exists(TalentData.SavePath);
        }
    }

    public static bool ExtraDataExists()
    {
        if (SteamManager.Initialized)
        {
            return SteamScript.Instance.FileExists(EXTRA_DATA);
        }
        else
        {
            return File.Exists(EXTRA_DATA_PATH);
        }
    }

}