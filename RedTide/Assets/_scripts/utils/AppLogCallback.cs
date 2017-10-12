using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AppLogCallback : MonoBehaviour
{
    public string output = "";
    public string stack = "";
    
    private string logName;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        logName = DateTime.Now.ToBinary().ToString();
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        output = logString;
        stack = stackTrace;
        if (type != LogType.Log && type != LogType.Warning)
        {
            using (FileStream _fs = File.Open(logName, FileMode.Append, FileAccess.Write))
            {
                string all = output + " \n" + stack + " \n";
                var bytes = System.Text.Encoding.Default.GetBytes(all);
                _fs.Write(bytes, 0, bytes.Length);
            }

        }
    }
}