using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Counts the number of edge crossings including the angles between them 
 */
public class EdgeCrossingCounter : MonoBehaviour {
    public bool countEdgeCrossings;
    public int count;
    public float averageAngle;
    public float edgeCrossRM;

    private List<List<Vector3>> _edges;
    public List<float> _angles;
    private Observer _observer;
    private Transform _projectionPlane;
	// Use this for initialization
	void Start () {
        _observer = (Observer)FindObjectOfType(typeof(Observer));
        _edges = new List<List<Vector3>>();
        _projectionPlane = GameObject.Find("ProjectionPlane").transform;
        _angles = new List<float>();
	}
	
	// Update is called once per frame
	void Update () {
        if (countEdgeCrossings)
        {
            GetComponent<TwoDimensionalProjection>().ProjectTree();
            count = 0;
            _angles = new List<float>();
            countEdgeCrossings = false;
            CountEdgeCrossings();
        }
	}

    // Popupates a list of existing edges
    void PopulateEdges()
    {
        _edges = new List<List<Vector3>>();
        List<Vector3> temp = new List<Vector3>();
        foreach(var op in _observer.GetOperators())
        {
            if(op.Children != null)
            {
                foreach(var child in op.Children)
                {
                    temp = new List<Vector3>();
                    temp.Add(_projectionPlane.InverseTransformPoint(op.GetIcon().transform.position));
                    temp.Add(_projectionPlane.InverseTransformPoint(child.GetIcon().transform.position));
                    _edges.Add(temp);
                }
            }
        }
    }

    // Counts the number of edge crossings
    public int CountEdgeCrossings()
    {
        PopulateEdges();
        count = 0;
        for(int i=0; i<_edges.Count; i++)
        {
            for(int j=i+1; j<_edges.Count; j++)
            {
                if (FasterLineSegmentIntersection(_edges[i][0], _edges[i][1], _edges[j][0], _edges[j][1]))
                {
                    _angles.Add(EdgeAngle(_edges[i][1] - _edges[i][0], _edges[j][1] - _edges[j][0]));
                    count++;
                }
            }
        }
        if (_angles.Count > 0)
        {
            averageAngle = AverageAngle(_angles);
            edgeCrossRM = CrossingAngleReadabilityMetric(_angles);
        }
        else
        {
            edgeCrossRM = 1;
        }
        return count;
    }

    // Edge crossing algorithm
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

    // Returns the angle between two directional vectors
    float EdgeAngle(Vector3 line1, Vector3 line2)
    {
        float angle = 0;
        if (Vector3.Angle(line1, line2) > 90) angle = 180 - Vector3.Angle(line1, line2);
        else angle = Vector3.Angle(line1, line2);
        return angle;
    }

    // Returns the average crossing-edges angle
    float AverageAngle(List<float> angles)
    {
        float averageAngle = 0;
        foreach(var angle in angles)
        {
            averageAngle += angle;
        }
        return averageAngle / angles.Count;
    }

    float CrossingAngleReadabilityMetric(List<float> angles)
    {
        float value = 0;
        float idealAngle = 70;
        foreach(var angle in angles)
        {
            value += Mathf.Abs(idealAngle - angle);
        }
        value /= angles.Count*idealAngle;
        return 1 - value;
    }
}
