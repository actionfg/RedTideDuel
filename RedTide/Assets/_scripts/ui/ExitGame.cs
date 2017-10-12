using UnityEngine;
using Mgl;

public class ExitGame : MonoBehaviour
{

    public void OnClickExit()
    {
        MessageDialog.ShowMessageBox(I18n.Instance.__("ExitGame"), MessageDialog.DialogType.YesNo, Callback);
    }

    public void Callback(MessageDialog.DialogButton button)
    {
        if (button == MessageDialog.DialogButton.Yes)
        {
            Application.Quit();
        }
    }
}