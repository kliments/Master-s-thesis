using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantRotationOfGraph : MonoBehaviour {
    public List<Vector3> positions;
    public Transform graph;
    public Transform viewPortPos;
    public bool set;

    private Observer observer;
    private Vector3 cameraPos;
    private Transform[] childNodes;
	// Use this for initialization
	void Start () {
        observer = (Observer)FindObjectOfType(typeof(Observer));
    }
	
	// Update is called once per frame
	void Update () {
		if(set)
        {
            set = false;
            cameraRotation();
        }
	}

    public void cameraRotation()
    {
        SetBackToZero();
        SetCenter();
        childNodes = new Transform[observer.GetOperators().Count];

        //remove children objects from graph parent
        for(int c= graph.childCount-1; c>=0; c--)
        {
            childNodes[c] = graph.GetChild(c);
            graph.GetChild(c).parent = null;
        }
        //set rotation of graph parent towards viewpoint
        graph.LookAt(viewPortPos);

        //assign node objects as children to graph parent
        for(int c=0; c<childNodes.Length; c++)
        {
            childNodes[c].parent = graph;
        }

        //rotate graph parent towards camera
        graph.LookAt(Camera.main.transform);
    }

    //setting the position of the parent object of the graph to the center of the graph visualization
    void SetCenter()
    {
        Vector3 center = new Vector3();
        Vector3[] nodePositions = new Vector3[observer.GetOperators().Count];
        for(int i=0; i<nodePositions.Length; i++)
        {
            nodePositions[i] = graph.GetChild(i).position;
            center += nodePositions[i];
        }
        center /= nodePositions.Length;
        graph.position = center;
        for(int i=0; i<nodePositions.Length; i++)
        {
            graph.GetChild(i).position = nodePositions[i];
        }
    }

    void SetBackToZero()
    {
        graph.position = new Vector3(0, 2.5f, 0);
        graph.rotation = Quaternion.identity;
        foreach(Transform childIcon in graph)
        {
            childIcon.position = childIcon.GetComponent<IconProperties>().newPos;
        }
    }
}
