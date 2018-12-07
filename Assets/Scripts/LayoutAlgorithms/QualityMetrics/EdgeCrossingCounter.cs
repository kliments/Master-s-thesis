using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeCrossingCounter : MonoBehaviour {
    public bool countEdgeCrossings;
    public int count;
    private List<List<Vector3>> _edges;
    private Observer observer;
    private Transform projectionPlane;
	// Use this for initialization
	void Start () {
        observer = (Observer)FindObjectOfType(typeof(Observer));
        _edges = new List<List<Vector3>>();
        projectionPlane = GameObject.Find("ProjectionPlane").transform;
	}
	
	// Update is called once per frame
	void Update () {
		if(countEdgeCrossings)
        {
            countEdgeCrossings = false;
            PopulateEdges();
            Debug.Log(CountEdgeCrossings() + " crossings");
        }
	}

    void PopulateEdges()
    {
        _edges = new List<List<Vector3>>();
        List<Vector3> temp = new List<Vector3>();
        foreach(var op in observer.GetOperators())
        {
            if(op.Children != null)
            {
                foreach(var child in op.Children)
                {
                    temp = new List<Vector3>();
                    temp.Add(projectionPlane.InverseTransformPoint(op.GetIcon().transform.position));
                    temp.Add(projectionPlane.InverseTransformPoint(child.GetIcon().transform.position));
                    _edges.Add(temp);
                }
            }
        }
    }

    int CountEdgeCrossings()
    {
        count = 0;
        for(int i=0; i<_edges.Count; i++)
        {
            for(int j=i+1; j<_edges.Count; j++)
            {
                if (FasterLineSegmentIntersection(_edges[i][0], _edges[i][1], _edges[j][0], _edges[j][1])) count++;
            }
        }
        return count;
    }

    bool FasterLineSegmentIntersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        Vector3 a = p2 - p1;
        Vector3 b = p3 - p4;
        Vector3 c = p1 - p3;
        if (p1 == p3 || p1 == p4 || p2 == p3 || p2 == p4) return false;
        float alphaNumerator = b.y * c.x - b.x * c.y;
        float alphaDenominator = a.y * b.x - a.x * b.y;
        float betaNumerator = a.x * c.y - a.y * c.x;
        float betaDenominator = alphaDenominator;
        if (alphaDenominator == 0 || betaDenominator == 0)
        {
            return false;
        }
        else
        {
            if (alphaDenominator > 0)
            {
                if (alphaNumerator < 0 || alphaNumerator > alphaDenominator) return false;
            }
            else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator) return false;

            if (betaDenominator > 0)
            {
                if (betaNumerator < 0 || betaNumerator > betaDenominator) return false;
            }
            else if (betaNumerator > 0 || betaNumerator < betaDenominator) return false;
        }
        return true;
    }
}
