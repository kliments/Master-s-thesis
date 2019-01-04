using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutAlgorithm : MonoBehaviour {
    public bool coroutineIsRunning;
    public GeneralLayoutAlgorithm currentLayout;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator StartAlgorithm()
    {
        coroutineIsRunning = true;
        yield return 0;
        if (currentLayout != null) currentLayout.StartAlgorithm();
        coroutineIsRunning = false;
    }
}
