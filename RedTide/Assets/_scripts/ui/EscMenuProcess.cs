using UnityEngine;
using UnityEngine.SceneManagement;

public class EscMenuProcess : MonoBehaviour
{
    public static EscMenuProcess Process;

    public GameObject panel;
    private GameObject _talentContainer;

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

    void Start()
    {
//        var canvas = GameContext.FloatingCombatPopupManager.Canvas;
//        _talentContainer = canvas.GetComponentInChildren<TalentMapUpdater>(true).gameObject.transform.parent.gameObject;
    }

    public void OnResumeClick()
    {
        Time.timeScale = 1f;
        panel.SetActive(false);
    }

    public void OnBackToMainClick()
    {

        // TODO 提示最近小节以后的数据会丢失
        OnResumeClick();

//        ChapterSwitcher.ClearScene();
//
//        GameContext.stage = -1;
//        GameContext.chapter = 0;
        SceneManager.LoadScene(0);
    }


    public void ActiveEscMenu(bool active)
    {
        if (active)
        {
            if (!_talentContainer.activeSelf)
            {
                Time.timeScale = 0f;
                panel.SetActive(true);
            }
        }
    }
}