using UnityEngine;

public class OperationDescription : MonoBehaviour
{

	public GameObject OperationDescriptionImage;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("ItemShow"))
		{
			if (OperationDescriptionImage)
			{
				OperationDescriptionImage.SetActive(true);
			}
		}
		else if (Input.GetButtonUp("ItemShow"))
		{
			if (OperationDescriptionImage)
			{
				OperationDescriptionImage.SetActive(false);
			}
		}
	}
}
