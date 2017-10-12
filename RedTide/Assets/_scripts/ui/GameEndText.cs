using Mgl;
using UnityEngine;
using UnityEngine.UI;

public class GameEndText : MonoBehaviour
{
    private static string FAIL_KEY = "GameOver";
    private static string SUCCESS_KEY = "Success";

	// Use this for initialization
	void Start ()
	{
	    var text = GetComponent<Text>();
	    if (text)
	    {
//	        text.text = GameContext.Success ? I18n.Instance.__(SUCCESS_KEY) : I18n.Instance.__(FAIL_KEY + GameContext.MessageCount);
//
//		    GameContext.MessageCount += 1;
	    }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
