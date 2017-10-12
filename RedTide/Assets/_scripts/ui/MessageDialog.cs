using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MessageDialog : MonoBehaviour
{

    public static MessageDialog dialog;

    public GameObject dimmer;
    public Button yes;
    public Button no;
    public Button ok;
    public Text text;
    private float dimmerAlpha;
    private Image _dimmerImage;
    private Canvas _canvas;

    void Awake()
    {
        if (dialog == null)
        {
            DontDestroyOnLoad(gameObject);
            dialog = this;
        }
        else if (dialog != this)
        {
            Destroy(gameObject);
        }
    }

    public static void ShowMessageBox(string message, MessageDialog.DialogType type, MessageDialog.DialogCallback callback)
    {
        if (dialog)
        {
            dialog.ShowMessage(message, type, callback);
        }
    }

    // Use this for initialization
	void Start ()
	{
	    _dimmerImage = dimmer.GetComponent<Image>();
	    _canvas = GetComponent<Canvas>();
	}

	// Update is called once per frame
	void Update () {
	    if (dimmerAlpha <= 1f)
	    {
	        dimmerAlpha = Mathf.Min(dimmerAlpha + Time.deltaTime * 2f, 1f);
	        var oldColor = _dimmerImage.color;
	        _dimmerImage.color = new Color(oldColor.r, oldColor.g, oldColor.b, dimmerAlpha * 0.8f);
	    }
	}

    public enum DialogButton
    {
        Yes,
        No,
        OK
    }

    public enum DialogType
    {
        YesNo,
        OkOnly
    }

    public delegate void DialogCallback(DialogButton button);

    public void ShowMessage(string message, DialogType type, DialogCallback callback)
    {
        text.text = message;
        this.callback = callback;

        _canvas.enabled = true;
        dimmer.gameObject.SetActive(true);
        dimmerAlpha = 0f;
        yes.gameObject.SetActive(type == DialogType.YesNo);
        no.gameObject.SetActive(type == DialogType.YesNo);
        ok.gameObject.SetActive(type == DialogType.OkOnly);
    }

    public DialogCallback callback { get; set; }

    public void OnYesClicked()
    {
        if (callback != null)
        {
            callback(DialogButton.Yes);
        }

        callback = null;
        _canvas.enabled = false;
    }

    public void OnNoClicked()
    {
        if (callback != null)
        {
            callback(DialogButton.No);
        }
        callback = null;
        _canvas.enabled = false;
    }

    public void OnOKClicked()
    {
        if (callback != null)
        {
            callback(DialogButton.OK);
        }
        callback = null;
        _canvas.enabled = false;
    }
}
