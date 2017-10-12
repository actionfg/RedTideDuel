using System.Collections;
using System.Collections.Generic;
using Mgl;
using UnityEngine;
using UnityEngine.UI;

public class LocaleTextSelector : MonoBehaviour {
    private string _key;

    // Use this for initialization
	void Start ()
	{
	    var text = GetComponent<Text>();
	    if (text)
	    {
	        _key = text.text;
	        // 如果_key会改变, 则需加入EventTrigger
	        text.text = I18n.Instance.__(_key);
	    }

	    I18n.OnLanguageChange += OnLanguageChange;
	}

    private void OnLanguageChange(string language)
    {
        var text = GetComponent<Text>();
        if (text)
        {
            text.text = I18n.Instance.__(_key);
        }
    }

    void OnDestroy()
    {
        I18n.OnLanguageChange -= OnLanguageChange;
    }
}
