using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using _scripts;


public class SwitchGameStage : MonoBehaviour
{
    public void OnClick()
    {
        GameContext.GameStage =
            (GameStage) (((int) (GameContext.GameStage + 1)) % (Enum.GetValues(typeof(GameStage)).Length));
        Debug.Log("Stage: " + GameContext.GameStage);
    }
}