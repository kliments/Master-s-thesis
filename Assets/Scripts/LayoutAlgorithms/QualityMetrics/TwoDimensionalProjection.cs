using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDimensionalProjection : MonoBehaviour {
    public bool project, print;
    public GameObject camera, projectionPlane;

    private Observer _observer;
    private RaycastHit _hit;
    private Vector3 direction;
    private LayerMask _layerMask;
    private Vector3  _averageNode;
	// Use this for initialization
	void Start () {
        _observer = (Observer)FindObjectOfType(typeof(Observer));
        _layerMask = 1 << LayerMask.NameToLayer("ProjectionPlane");
        _averageNode = new Vector3();
        camera = Camera.main.gameObject;
        projectionPlane = GameObject.Find("ProjectionPlane");
    }
	
	// Update is called once per frame
	void Update () {
		if(project)
        {
            project = false;
            foreach(var op in _observer.GetOperators())
            {
                direction = op.GetIcon().transform.position - camera.transform.position;
                if(Physics.Raycast(camera.transform.position, direction, out _hit, 50, _layerMask))
                {
                    if(op.GetComponent<LineRenderer>() != null) op.GetComponent<LineRenderer>().positionCount = 2;
                    op.GetIcon().transform.position = _hit.point;
                }
            }
            if (GetComponent<ConeTreeAlgorithm>().RDT) GetComponent<ConeTreeAlgorithm>().CalculateRDT();
        }
	}

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
}
