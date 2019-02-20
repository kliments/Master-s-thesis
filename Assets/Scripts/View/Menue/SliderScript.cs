using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderScript : GenericMenueComponent {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }
    private void OnDisable()
    {
        List<IMenueComponentListener> list = getListeners();
        for (int i = list.Count - 1; i > 0; i--)
        {
            removeListener(getListeners()[i]);
        }
    }
}
