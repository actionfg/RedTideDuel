using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _scripts;

// 主要用于点选3D模型, 或者指定特定位置
public class MousePick : MonoBehaviour {
	private int _unitLayer;
	private MobUnit _selectTarget;

	// Use this for initialization
	void Start ()
	{
		_unitLayer = LayerMask.GetMask("DynamicUnit");
	}
	
	// Update is called once per frame
	void Update () {
		if (GameContext.GameStage == GameStage.Prepare)
		{
			if(Input.GetMouseButtonDown(0))
			{// left button
				RaycastHit rayHit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out rayHit, 200, _unitLayer))
				{
					var mobUnit = rayHit.collider.GetComponent<MobUnit>();
					if (mobUnit && mobUnit.PlayerId == GameContext.SelfPlayerId)
					{
						_selectTarget = mobUnit;
						Debug.Log("select " + rayHit.collider.name);
					}
				}
			}

//			if (Input.GetMouseButtonDown(1) && _selectTarget != null)
//			{// right button
//				RaycastHit rayHit;
//				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//				if (Physics.Raycast(ray, out rayHit, 200, ~LayerMask.GetMask("Ignore Raycast")))
//				{
//					Vector3 pos = rayHit.point;
//					if (rayHit.collider.tag.Equals("Floor") && AstarPathUtil.IsValid(pos))
//					{
//						_selectTarget.MoveTo(pos);
//					}
//				}
//			}
			// TODo 改为拖拽模式
			if (_selectTarget)
			{
				var floorHitPosition = EffectUtil.GetFloorHitPosition();
				_selectTarget.transform.position = floorHitPosition + Vector3.up * 0.5f;
			}

			if (Input.GetMouseButtonUp(0))
			{
				_selectTarget = null;
			}
		}
	}
}
