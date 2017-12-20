using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubTarget : Targetable {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void onClicked()
    {
        if(transform.parent.GetComponent<Targetable>() != null)
            transform.parent.GetComponent<Targetable>().onClicked();
    }
}
