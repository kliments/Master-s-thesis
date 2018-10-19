using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownComponent : Targetable {
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
        ShowOptions();
    }

    public void ShowOptions()
    {
        transform.parent.GetComponent<Dropdown>().Show();
        foreach(var selectable in Selectable.allSelectables)
        {
            if(selectable.name.Contains("Item"))
            {
                var box = selectable.gameObject.AddComponent<BoxCollider>();
                box.size = new Vector3(160, 20, 1.1f);
                selectable.gameObject.AddComponent<DropdownSelectableComponent>();
            }
        }
    }
}
