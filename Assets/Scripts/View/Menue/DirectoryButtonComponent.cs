using GracesGames.SimpleFileBrowser.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectoryButtonComponent : Targetable {
    private UserInterface UI;
    // Use this for initialization
    void Start ()
    {
        UI = (UserInterface)FindObjectOfType(typeof(UserInterface));
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
        if(UI != null) UI.dirList.Remove(transform.parent.gameObject);
    }
    protected override void OnLeftClickOnTargetEventAction()
    {
        transform.parent.GetComponent<GenericMenueComponent>().getListeners()[0].menueChanged(transform.parent.GetComponent<GenericMenueComponent>());
    }
}
