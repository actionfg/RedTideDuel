using System;
using UnityEngine;
using UnityEngine.UI;

public class MobNamePopupControl : MonoBehaviour
{
	public Text MobNameText;
	public Transform PanelTransform;
	public GameUnit GameUnit { get; set; }
	public GameObject Target { get; set; }
	public Transform TargetTransform { get; set; }
	public string Name { get; set; }

	private Camera _camera;

	// Use this for initialization
	void Start () {
		_camera = Camera.main;
		MobNameText.text = GetMobName(Name);
		MobNameText.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (GameUnit != null)
		{
			var worldPos = Target.transform.position + new Vector3(0, 2f, 0);
			if (TargetTransform != null)
			{
				worldPos = TargetTransform.position;
			}
			Vector3 point = _camera.WorldToScreenPoint(worldPos);
			PanelTransform.position = point + new Vector3(0f, 20f, 0f);
		}
		else {
			Destroy(gameObject);
		}

		if (Input.GetButtonDown("ItemShow"))
		{
			MobNameText.gameObject.SetActive(true);
		}
		else if (Input.GetButtonUp("ItemShow"))
		{
			MobNameText.gameObject.SetActive(false);
		}
	}

	private string GetMobName(string name)
	{
		if (!String.IsNullOrEmpty(name))
		{
			int lastIndex = name.IndexOf("(");
			return name.Substring(0, lastIndex);
		}
		return "";
	}
}
