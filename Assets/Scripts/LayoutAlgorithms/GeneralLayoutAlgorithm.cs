using Assets.Scripts.Model;
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
    public Observer observer;

    private float maxDepth;
    private float minDepth = 1;
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

    //update color of edges every time new node is added
    protected void ColorEdges()
    {
        if (observer == null) observer = (Observer)FindObjectOfType(typeof(Observer));
        float depth = 0;
        maxDepth = 0;
        foreach (var op in observer.GetOperators())
        {
            if (maxDepth < op.GetIcon().GetComponent<IconProperties>().depth) maxDepth = op.GetIcon().GetComponent<IconProperties>().depth;
        }
        foreach (var op in observer.GetOperators())
        {
            if (op.Parents == null || op.Parents.Count == 0) continue;
            depth = op.GetIcon().GetComponent<IconProperties>().depth;
            op.GetComponent<LineRenderer>().startColor = new Color(NormalizeColor(depth - 1), NormalizeColor(depth - 1), 1);
            op.GetComponent<LineRenderer>().endColor = new Color(NormalizeColor(depth - 1), NormalizeColor(depth - 1), 1);
            //op.GetComponent<LineRenderer>().endColor = new Color(NormalizeColor(depth), NormalizeColor(depth), 1);
        }
    }
    private float NormalizeColor(float depth)
    {
        return (depth - minDepth) / (maxDepth - minDepth);
    }
}
