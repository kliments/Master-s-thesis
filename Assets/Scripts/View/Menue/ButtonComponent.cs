using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonComponent : Targetable
{

	private void OnEnable()
	{
		InputController.LeftClickOnTargetEvent += OnLeftClickOnTargetEvent;
	}

	private void OnDisable()
	{
		InputController.LeftClickOnTargetEvent -= OnLeftClickOnTargetEvent;
	}

	protected override void OnLeftClickOnTargetEventAction()
	{
		ButtonScript button = gameObject.GetComponentInParent<ButtonScript>();
		if(button != null)
		{
			button.Toggle();
		}
		else
		{
			Debug.Log("Cannot find Button script.");
		}
	}
}
