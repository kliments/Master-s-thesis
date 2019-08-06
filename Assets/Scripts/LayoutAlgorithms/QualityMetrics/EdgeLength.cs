﻿using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Calculates the length of all edges and
 * returns the standard deviation of it
 */
public class EdgeLength : MonoBehaviour {
    public float edgeLengthSTD, totalEdgeLength;
    public List<float> edges;
    public bool calculateEdgeLength;

    private Observer _observer;
	// Use this for initialization
	void Start () {
        _observer = (Observer)FindObjectOfType(typeof(Observer));
	}
	
	// Update is called once per frame
	void Update () {
        if(calculateEdgeLength)
        {
            calculateEdgeLength = false;
            GetComponent<TwoDimensionalProjection>().ProjectTree();
            edgeLengthSTD = CalculateEdgeLength();
            GetComponent<TwoDimensionalProjection>().RestorePositions();
        }
	}

    public float CalculateEdgeLength()
    {
        edgeLengthSTD = 0;
        totalEdgeLength = 0;
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
        totalEdgeLength = TotalEdgeLength(edges);
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

    private float TotalEdgeLength(List<float> edges)
    {
        float sum = 0;
        foreach(var edge in edges)
        {
            sum += edge;
        }
        return sum;
    }
}
