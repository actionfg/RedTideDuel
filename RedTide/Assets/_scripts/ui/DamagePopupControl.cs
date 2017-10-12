using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamagePopupControl : MonoBehaviour
{

    public Animator Animator;
    public Text DamageText;

    public GameUnit target { get; set; }
    private Camera _camera;

    void Awake()
    {
        _camera = Camera.main; 
    }

    // Use this for initialization
	void Start ()
	{
	    AnimatorClipInfo[] clipInfos = Animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfos[0].clip.length);

	    var worldPos = target.transform.position + new Vector3(0, 2f, 0);
	    Vector3 point = _camera.WorldToScreenPoint(worldPos);
	    gameObject.transform.position = point + new Vector3(0f, 10f, 0f);
	}

    public void SetDamage(float damage)
    {
        DamageText.text = ((int) Math.Round(damage)).ToString();
    }

	public void SetText(string text)
	{
		DamageText.text = text;
	}
}
