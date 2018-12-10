using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPortOptimizer : MonoBehaviour {
    public Vector3 position;
    public bool scan;

    private Observer observer;
	// Use this for initialization
	void Start () {
        observer = (Observer)FindObjectOfType(typeof(Observer));
	}
	
	// Update is called once per frame
	void Update () {
		if(scan)
        {
            scan = false;
            //set gameobject in the center of the graph
            transform.position = FindCenterOfGraph();
            //set child to position of the furthest node + 2 meters
            transform.GetChild(0).localPosition = FindFurthestNode();
        }
	}

    Vector3 FindCenterOfGraph()
    {
        Vector3 pos = new Vector3();
        foreach(var op in observer.GetOperators())
        {
            pos += op.GetIcon().transform.position;
        }
        pos /= observer.GetOperators().Count;
        return pos;
    }

    Vector3 FindFurthestNode()
    {
        Vector3 pos = observer.GetOperators()[0].GetIcon().transform.position;
        foreach(var op in observer.GetOperators())
        {
            if (Vector3.Distance(transform.position, pos) < Vector3.Distance(transform.position, op.GetIcon().transform.position)) pos = op.GetIcon().transform.position;
        }
        pos = new Vector3(Vector3.Distance(transform.position, pos) + 1, 0, 0);
        return pos;
    }
}
