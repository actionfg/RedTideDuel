using System.Collections;
using System.Collections.Generic;
using Mgl;
using UnityEngine;
using UnityEngine.UI;

public class LocaleImageSelector : MonoBehaviour
{
    public Sprite EnSprite;
    public Sprite ZhSprite;
    private Image _image;

    // Use this for initialization
	void Start ()
	{
	    _image = GetComponent<Image>();
	    I18n.OnLanguageChange += OnLanguageChange;
	}

    private void OnLanguageChange(string language)
    {
        if (_image)
        {
            if (language.Contains("ZH"))
            {
                _image.sprite = ZhSprite;
            }
            else
            {
                _image.sprite = EnSprite;
            }
        }

    }

    void OnDestroy()
    {
        I18n.OnLanguageChange -= OnLanguageChange;
    }
}
