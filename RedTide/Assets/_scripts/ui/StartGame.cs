using UnityEngine;
using System.IO;
using Mgl;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{

    public Canvas characterUI;
    public GameObject buttonPanel;

    public void OnClickStart()
    {
//        StatisticManager.GetInstance().Analysis();
        if (buttonPanel)
        {
		    buttonPanel.SetActive(false);
        }
        if (characterUI)
        {
            characterUI.enabled = true;
        }

//        if (GameContext.ResumeDataExists())
//        {
//            MessageDialog.ShowMessageBox(I18n.Instance.__("StartNewGame"), MessageDialog.DialogType.YesNo, Callback);
//        }
//        else
        {
            StartNewGame();
        }

    }

    public void Callback(MessageDialog.DialogButton button)
    {
        if (button == MessageDialog.DialogButton.Yes)
        {
            StartNewGame();
        }
    }

    private void StartNewGame()
    {
        // delete saved info
//        GameContext.DeleteSavedPosition();
//        GameContext.DeleteSavedGameData();
//        GameContext.Reset();
//        // TODO 考虑移至进入第一关时才清零
////        GameContext.talentData.ClearRemainingTalentFragments();
//        GameContext.GoldData.AddTalentFragments(GameContext.talentData.GetTalentFragmentsLastGame() * GameContext.talentData.GetPoint(TalentType.HomeingSoul));
        SceneManager.LoadScene("main");
    }

    public void OnSelectCharacter(int c)
    {
//        GameContext.character = c;
        SceneManager.LoadScene("main");
    }
}