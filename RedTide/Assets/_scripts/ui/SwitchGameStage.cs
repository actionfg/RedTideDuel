using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using _scripts;

// TODO 改为只有服务器收到所有人准备完毕的消息后, 才切换GameStage
public class SwitchGameStage : MonoBehaviour
{
    public void OnClick()
    {
        bool allReady = true;
        foreach (var player in GameDuel.NetworkGameManager.sPlayers)
        {
            if (player.isLocalPlayer)
            {
                player.CmdChangeBattleReady();
            }

            if (!player.ReadyForBattle)
            {
                allReady = false;
            }
        }

        if (allReady)
        {
            GameContext.GameStage =
                (GameStage) (((int) (GameContext.GameStage + 1)) % (Enum.GetValues(typeof(GameStage)).Length));
            Debug.Log("Stage: " + GameContext.GameStage);
        }
    }
}