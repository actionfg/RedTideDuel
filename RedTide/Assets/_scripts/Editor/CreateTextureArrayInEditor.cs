using System.Collections;
using System.Collections.Generic;
using Game04.Util;
using UnityEditor;
using UnityEngine;

public class CreateTextureArrayInEditor : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		var texture2DArray = TextureUtil.CreateArray("textures/rainTexs/cv0_vPositive_{0:0000}", 370, 16, 526, TextureFormat.RGB24);
		string path = "Assets/_prefabs/TextureArray.asset";
		AssetDatabase.CreateAsset(texture2DArray, path);
		Debug.Log("Saved asset to " + path);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
