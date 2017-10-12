using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolRouteSetter : MonoBehaviour
{
    // 在刷新点固定半径圆形范围随意走动
    public bool RandPatrol;
    public float Radius;

    // 循环沿固定路点移动
    public List<Transform> RoutePoints;

    public float PatrolDelay;

    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
