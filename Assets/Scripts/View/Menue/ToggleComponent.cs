using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleComponent : Targetable
{

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
        ToggleScript mainToggle = gameObject.GetComponentInParent<ToggleScript>();
        if(mainToggle != null)
        {
            mainToggle.toggle();
        }
        else
        {
            Debug.Log("Can not found main toggle script!");
        }
    }
}
