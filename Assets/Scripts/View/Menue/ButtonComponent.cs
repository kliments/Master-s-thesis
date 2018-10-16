using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonComponent : Targetable {

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
        Debug.Log("button pressed");
    }
}
