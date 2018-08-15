using System.Collections;
using System.Collections.Generic;
using Controller.Input;
using UnityEngine;

public class ScatterplotInteractionController : Targetable
{
	private Vector3 screenPoint;
	private Vector3 offset;
	private Vector3 scanPos;
	
//	private void OnEnable()
//	{
//		InputController.LeftClickOnTargetEvent += OnLeftClickOnTargetEvent;
//	}
//
//	private void OnDisable()
//	{
//		InputController.LeftClickOnTargetEvent -= OnLeftClickOnTargetEvent;
//	}
//
//	protected override void OnLeftClickOnTargetEventAction()
//	{
//		
//	}
	
	void OnMouseDown()
	{
		screenPoint = Camera.main.WorldToScreenPoint(scanPos);
		offset = scanPos - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
	}


	void OnMouseDrag()
	{
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
		transform.position = curPosition;
	}
}
