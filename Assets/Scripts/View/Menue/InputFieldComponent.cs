﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldComponent : Targetable{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }
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
        transform.parent.GetComponent<InputField>().Select();
        Debug.Log("input field pressed");
    }
}
