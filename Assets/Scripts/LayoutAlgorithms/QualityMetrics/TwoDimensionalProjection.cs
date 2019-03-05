﻿using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Projects the current viewport of the tree on 2D plane
 * Used for calculation of number of edge crossings
 * and angles between them
 */
public class TwoDimensionalProjection : MonoBehaviour {
    public GeneralLayoutAlgorithm current;
    public RDTAlgorithm RDT;
    private Transform projectionPlane;
    private Observer _observer;
    private RaycastHit _hit;
    private Vector3 direction, _averageNode;
    private LayerMask _layerMask;
    private List<Vector3> _originalPositions;
	// Use this for initialization
	void Start () {
        current = GetComponent<LayoutAlgorithm>().currentLayout;
        _observer = (Observer)FindObjectOfType(typeof(Observer));
        _layerMask = LayerMask.GetMask("ProjectionPlane");
        _averageNode = new Vector3();
    }
	
	// Update is called once per frame
	void Update () {

	}

    // Sets the plane in the center of the tree-graph
    public void SetPlane()
    {
        _averageNode = new Vector3();
        projectionPlane = Camera.main.transform.GetChild(0);
        foreach (var op in _observer.GetOperators())
        {
            _averageNode += op.GetIcon().transform.position;
        }
        _averageNode /= _observer.GetOperators().Count;
        float distance = Vector3.Distance(Camera.main.transform.position, _averageNode);
        Vector3 planePos = new Vector3(projectionPlane.localPosition.x, projectionPlane.localPosition.y, distance);
        projectionPlane.localPosition = planePos;
    }

    public void RestorePositions()
    {
        for(int i=0; i<_observer.GetOperators().Count; i++)
        {
            _observer.GetOperators()[i].GetIcon().transform.position = _originalPositions[i];
        }
        if (current != RDT) GetComponent<LayoutAlgorithm>().currentLayout.PlaceEdges();
        else RDT.CalculateRDT();
    }

    public void ProjectTree()
    {
        current = GetComponent<LayoutAlgorithm>().currentLayout;
        _originalPositions = new List<Vector3>();
        foreach (var op in _observer.GetOperators())
        {
            _originalPositions.Add(op.GetIcon().transform.position);
            direction = op.GetIcon().transform.position - Camera.main.transform.position;
            if (Physics.Raycast(Camera.main.transform.position, direction, out _hit, 50, _layerMask))
            {
                if (op.GetComponent<LineRenderer>() != null) op.GetComponent<LineRenderer>().positionCount = 2;
                op.GetIcon().transform.position = _hit.point;
            }
        }
        if (current != RDT) current.PlaceEdges();
        else RDT.CalculateRDT();
    }
}
