using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButtonScript : GenericMenueComponent {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void CloseAllMenus()
    {
        if(getListeners().Count != 0) getListeners()[0].CloseAllMenus();
    }
}
