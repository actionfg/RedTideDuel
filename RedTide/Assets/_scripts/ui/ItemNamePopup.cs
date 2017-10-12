using UnityEngine;
using UnityEngine.UI;

public class ItemNamePopup : MonoBehaviour {
	
	public RectTransform Popup;
	public Text Name;

	// Use this for initialization
	void Start ()
	{

	}

	public void Show(string itemName)
	{
		Name.text = itemName;
		
		var rectTransform = Name.gameObject.GetComponent<RectTransform>();
		int nameTextWidth = (int) LayoutUtility.GetPreferredWidth(rectTransform);
		Popup.sizeDelta = new Vector2(nameTextWidth, 20);
	}
}
