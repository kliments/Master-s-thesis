using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeAngularResolution : MonoBehaviour {
    public float angularResolution;
    public bool calculateAngularRes;
    public int count = 0;

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
        float angRes = 0;
        float idealAngle = 0;
        float min = 0;
        count = 0;
        foreach(var op in observer.GetOperators())
        {
            if (op.Children == null) continue;
            if (op.Children.Count < 2) continue;
            idealAngle = 360 / op.Children.Count;
            for(int i=0; i<op.Children.Count; i++)
            {
                count++;
                _angles = new List<float>();
                for(int j=0; j<op.Children.Count; j++)
                {
                    if (i == j) continue;
                    edge1 = op.Children[i].GetIcon().transform.position - op.GetIcon().transform.position;
                    edge2 = op.Children[j].GetIcon().transform.position - op.GetIcon().transform.position;
                    _angles.Add(Vector3.Angle(edge1, edge2));
                }
                min = ReturnMinAngle(_angles);
                angRes += Mathf.Abs((idealAngle - min) / idealAngle);
            }
        }
        return (1 - angRes/count);
    }

    private float ReturnMinAngle(List<float> angles)
    {
        float min = angles[0];
        foreach(var angle in angles)
        {
            if (min > angle) min = angle;
        }
        return min;
    }
}
