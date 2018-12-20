using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeAngularResolution : MonoBehaviour {
    public float angularResolution;
    public bool calculateAngularRes;

    private List<float> _angles;
    private Observer observer;
	// Use this for initialization
	void Start () {
        observer = (Observer)FindObjectOfType(typeof(Observer));
	}
	
	// Update is called once per frame
	void Update () {
	}

    public float CalculateAngularResolution()
    {
        _angles = new List<float>();
        Vector3 edge1 = new Vector3();
        Vector3 edge2 = new Vector3();
        foreach(var op in observer.GetOperators())
        {
            if (op.Children == null) continue;
            if (op.Children.Count < 2) continue;

            for(int i=0; i<op.Children.Count; i++)
            {
                for(int j=i+1; j<op.Children.Count; j++)
                {
                    edge1 = op.Children[i].GetIcon().transform.position - op.GetIcon().transform.position;
                    edge2 = op.Children[j].GetIcon().transform.position - op.GetIcon().transform.position;
                    _angles.Add(Vector3.Angle(edge1, edge2));
                }
            }
        }
        float angRes = _angles[0];
        for(int i=0; i<_angles.Count; i++)
        {
            if (angRes > _angles[i]) angRes = _angles[i];
        }
        return angRes;
    }
}
