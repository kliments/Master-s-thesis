using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GeneralLayoutAlgorithm : MonoBehaviour {
    
    public bool _temporal;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public abstract void StartAlgorithm();

    public void SetTemporal()
    {
        _temporal = true;
    }

    public void UnsetTemporal()
    {
        _temporal = false;
    }

    public bool GetTemporal()
    {
        return _temporal;
    }
}
