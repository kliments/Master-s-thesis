using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownSelectableComponent : Targetable {

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
        /*var selectables = Selectable.allSelectables;
        for(int i=selectables.Count; i>=0; i++)
        {
            if (selectables[selectables.Count - 1] == null) continue;
            if (selectables[selectables.Count - 1].name.Contains("Item"))
            {
                selectables[selectables.Count - 1].GetComponent<Toggle>().isOn = false;
            }
            i--;
        }*/
        GetComponent<Toggle>().isOn = true;
    }
}
