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
        //close all opened menus
        GenericMenueComponent[] list = FindObjectsOfType<GenericMenueComponent>();
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] == transform.parent.GetComponent<GenericMenueComponent>().getListeners()[0]) continue;
            list[i].CloseAllMenus();
        }

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
