﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewIcon : GenericIcon
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void onClicked()
    {
        Debug.Log("fassfdasa");
        op.observer.getVisualizationSpaceController().installNewVisualization(op);
    }
}
