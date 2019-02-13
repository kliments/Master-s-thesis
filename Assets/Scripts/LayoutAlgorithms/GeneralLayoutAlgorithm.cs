using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GeneralLayoutAlgorithm : MonoBehaviour, IMenueComponentListener {
    
    public bool _temporal;

    //subscriber button
    public GenericMenueComponent subscriber;

    //speed of animation
    public int speed;

    public bool _finished = true;
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

    public void menueChanged(GenericMenueComponent changedComponent)
    {
        StartAlgorithm();
    }

    public bool AlgorithmHasFinished()
    {
        return _finished;
    }

    public void SetFinish()
    {
        _finished = true;
    }

    public void SetStart()
    {
        _finished = false;
    }
}
