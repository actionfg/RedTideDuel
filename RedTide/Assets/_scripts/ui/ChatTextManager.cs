using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Lib.SimpleJSON;
using UnityEngine;

public class ChatTextManager : MonoBehaviour
{
    private static readonly string CT_FILE = "Chat/ct.json";

    public static ChatTextManager instance;

    // NPC的台词
    private JSONNode _chatJson;

    // 台词进度
    private Dictionary<string, int> _chatProcedure = new Dictionary<string, int>();

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        InitChatText();
    }

    private void InitChatText()
    {
        var loadAsset = Resources.Load(CT_FILE) as TextAsset;
        if (loadAsset != null)
            _chatJson = JSON.Parse(loadAsset.text);
    }

    public static void Reload()
    {
        instance.InitChatText();
    }

    public static string GetChatText(string npcName)
    {
        var jsonNode = instance._chatJson[npcName];
        var index = 0;
        if (instance._chatProcedure.ContainsKey(npcName))
        {
            index = instance._chatProcedure[npcName];
        }
        return jsonNode[index];
    }

    public static void ProcedureNext(string npcName)
    {
        var jsonNode = instance._chatJson[npcName];
        var index = 0;
        if (instance._chatProcedure.ContainsKey(npcName))
        {
            index = instance._chatProcedure[npcName];
        }

        instance._chatProcedure[npcName] = Math.Min(index + 1, jsonNode.Count - 1);
    }
}