using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Calculates the length of all edges and
 * returns the standard deviation of it
 */
public class EdgeLength : MonoBehaviour {
    public float edgeLengthSTD;
    public List<float> edges;
    public bool calculateEdgeLength;

    private Observer _observer;
	// Use this for initialization
	void Start () {
        _observer = (Observer)FindObjectOfType(typeof(Observer));
	}
	
	// Update is called once per frame
	void Update () {
	}

    public float CalculateEdgeLength()
    {
        edgeLengthSTD = 0;
        edges = new List<float>();

        foreach(var op in _observer.GetOperators())
        {
            if(op.Children != null) if(op.Children.Count!=0)
                {
                    foreach (var child in op.Children)
                    {
                        edges.Add(Vector3.Distance(op.GetIcon().transform.position, child.GetIcon().transform.position));
                    }
                }
        }
        edgeLengthSTD = CalculateSTD(edges);
        return edgeLengthSTD;
    }

    // Returns the standard deviation of all lengths of edges
    private float CalculateSTD(List<float> edges)
    {
        float sum = 0;
        float mean = 0;
        for(int i=0; i<edges.Count; i++)
        {
            mean += edges[i];
        }
        mean /= edges.Count;
        for(int i=0; i<edges.Count; i++)
        {
            sum += Mathf.Pow(edges[i] - mean, 2);
        }
        sum /= edges.Count;
        return Mathf.Sqrt(sum);
    }
}
