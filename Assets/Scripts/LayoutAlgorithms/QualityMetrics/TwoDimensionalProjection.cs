using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Projects the current viewport of the tree on 2D plane
 * Used for calculation of number of edge crossings
 * and angles between them
 */
public class TwoDimensionalProjection : MonoBehaviour {
    public GameObject camera, projectionPlane;
    public ConeTreeAlgorithm RDT;
    private Observer _observer;
    private RaycastHit _hit;
    private Vector3 direction, _averageNode;
    private LayerMask _layerMask;
    private List<Vector3> _originalPositions;
	// Use this for initialization
	void Start () {
        _observer = (Observer)FindObjectOfType(typeof(Observer));
        _layerMask = 1 << LayerMask.NameToLayer("ProjectionPlane");
        _averageNode = new Vector3();
    }
	
	// Update is called once per frame
	void Update () {

	}

    // Sets the plane in the center of the tree-graph
    public void SetPlane()
    {
        foreach (var op in _observer.GetOperators())
        {
            _averageNode += op.GetIcon().transform.position;
        }
        _averageNode /= _observer.GetOperators().Count;
        float distance = Vector3.Distance(camera.transform.position, _averageNode);
        Vector3 planePos = new Vector3(projectionPlane.transform.localPosition.x, projectionPlane.transform.localPosition.y, distance);
        projectionPlane.transform.localPosition = planePos;
    }

    public void RestorePositions()
    {
        for(int i=0; i<_observer.GetOperators().Count; i++)
        {
            _observer.GetOperators()[i].GetIcon().transform.position = _originalPositions[i];
            if (_observer.GetOperators()[i].GetComponent<LineRenderer>() != null)
            {
                _observer.GetOperators()[i].GetComponent<LineRenderer>().positionCount = 2;
                _observer.GetOperators()[i].GetComponent<LineRenderer>().SetPosition(0, _observer.GetOperators()[i].GetIcon().transform.position);
                _observer.GetOperators()[i].GetComponent<LineRenderer>().SetPosition(1, _observer.GetOperators()[i].Parents[0].GetIcon().transform.position);
            }
        }
    }

    public void ProjectTree()
    {
        _originalPositions = new List<Vector3>();
        foreach (var op in _observer.GetOperators())
        {
            _originalPositions.Add(op.GetIcon().transform.position);
            direction = op.GetIcon().transform.position - camera.transform.position;
            if (Physics.Raycast(camera.transform.position, direction, out _hit, 50, _layerMask))
            {
                if (op.GetComponent<LineRenderer>() != null) op.GetComponent<LineRenderer>().positionCount = 2;
                op.GetIcon().transform.position = _hit.point;
            }
        }
        if (RDT.RDT) RDT.CalculateRDT();
    }
}
