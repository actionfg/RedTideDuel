using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoRotateCameraControl : MonoBehaviour
{

	public float Speed = 10f;
	private Camera _cam;

	// Use this for initialization
	void Start ()
	{
		_cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		_cam.transform.position += new Vector3(horizontal, 0, vertical) * Speed;
	}
}
