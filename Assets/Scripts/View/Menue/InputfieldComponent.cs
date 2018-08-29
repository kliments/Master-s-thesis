using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputfieldComponent : Targetable
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
		InputfieldScript inputfield = gameObject.GetComponentInParent<InputfieldScript>();
		if(inputfield != null)
		{
			inputfield.Toggle();
		}
		else
		{
			Debug.Log("Cannot find Button script.");
		}
	}
}