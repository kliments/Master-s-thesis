using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButtonComponent : Targetable {

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
        transform.parent.GetComponent<GenericMenueComponent>().getListeners()[0].menueChanged(transform.parent.GetComponent<GenericMenueComponent>());
    }
}
