using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatDialogControl : MonoBehaviour
{
    public Text ChatText;

    private string _speaker;
    private Vector3 _worldPos;

    private void Update()
    {
        Vector3 point = Camera.main.WorldToScreenPoint(_worldPos);
        transform.position = point + new Vector3(0f, 10f, 0f);
    }

    public void SetSpeaker(string speaker, Vector3 worldPos)
    {
        _speaker = speaker;
        ChatText.text = ChatTextManager.GetChatText(_speaker);
        _worldPos = worldPos + 2f * Vector3.up;
    }

    public void Next()
    {
        ChatTextManager.ProcedureNext(_speaker);
        ChatText.text = ChatTextManager.GetChatText(_speaker);
    }
}