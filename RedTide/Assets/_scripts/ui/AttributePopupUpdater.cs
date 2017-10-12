using UnityEngine;

public class AttributePopupUpdater : MonoBehaviour
{

	public GameObject ItemAttributePopup;
	public GameObject CurrentAttributePopup;

	private ItemAttributePopupUpdater _itemAttributePopupUpdater;
	private CurrentAttributePopupUpdater _currentAttributePopupUpdater;

	// Use this for initialization
	void Start ()
	{
		_itemAttributePopupUpdater = ItemAttributePopup.GetComponent<ItemAttributePopupUpdater>();
		_currentAttributePopupUpdater = CurrentAttributePopup.GetComponent<CurrentAttributePopupUpdater>();
		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Switch()
	{
		_itemAttributePopupUpdater.Switch();
		_currentAttributePopupUpdater.Switch();
	}

//	public void Show(DroppedItemAttribute droppedItemAttribute)
//	{
//		_itemAttributePopupUpdater.Show(droppedItemAttribute);
//		_currentAttributePopupUpdater.Show(droppedItemAttribute);
//	}
}
