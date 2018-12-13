using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAlgorithm : GeneralLayoutAlgorithm {

    public List<Vector3> positions;
    private Observer observer;
	// Use this for initialization
	void Start () {
        observer = (Observer)FindObjectOfType(typeof(Observer));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void StartAlgorithm()
    {
        GetComponent<LayoutAlgorithm>().currentLayout = this;
        for(int i=0; i<observer.GetOperators().Count; i++)
        {
            observer.GetOperators()[i].GetIcon().transform.position = positions[i];
        }
    }
}
