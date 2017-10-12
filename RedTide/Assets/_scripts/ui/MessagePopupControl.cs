using UnityEngine;
using UnityEngine.UI;

public class MessagePopupControl : MonoBehaviour
{

    public Text Text;
    
    private float _duration;
    private float _acc;

    // Use this for initialization
    void Start ()
    {
        _duration = 2f;
        _acc = 0;
    }
	
    // Update is called once per frame
    void Update ()
    {
        _acc += Time.deltaTime;
        if (_acc > _duration)
        {
            gameObject.SetActive(false);
            _acc = _duration;
        }
    }

    public void Show(PrecastType precastType)
    {
        Text.text = GetMessage(precastType);
        gameObject.SetActive(true);
        _acc = 0;
    }

    private string GetMessage(PrecastType precastType)
    {
        switch (precastType)
        {
            case PrecastType.Range:
                return "范围内没有目标!";
            case PrecastType.Target:
                return "没有选定的目标!";
            case PrecastType.Location:
                return "目标位置不合法!";
        }
        return "";
    }
}
