using Assets.Scripts.Model;
using System.Collections.Generic;
using UnityEngine;

public class ConeTreeAlgorithm : MonoBehaviour {
    public Observer observer;
    public bool runConeTree, timeDependent, RDT;
    public float height;
    public List<Vector3> referencePoints;

    private float _minRadius = 0.5f;
    private Vector3 _anchor;
    private GenericOperator root;
    private float[] _timeStamps;
    private GraphSpaceController _graphSpace;
	// Use this for initialization
	void Start () {
        referencePoints = new List<Vector3>();
        _graphSpace = GameObject.Find("GraphSpace").GetComponent<GraphSpaceController>();
	}
	
	// Update is called once per frame
	void Update () {
		if(runConeTree)
        {
            root = observer.GetOperators()[0];
            _anchor = root.GetIcon().transform.position;
            runConeTree = false;
            observer.NormalizeTimeStamps();
            Layout(root, _anchor.x, _anchor.z);
        }
        if(RDT)
        {
            RDT = false;
            CalculateRDT();
        }
	}

    private void Layout(GenericOperator root, float x, float z)
    {
        NormalizeDepth();
        FirstWalk(root);
        //SecondWalk(root, x, z, 1f, 0f);
        SecondWalk(root, x, z, 1f, 0f);
        CalculateRDT();
    }

    /* Bottom up proceeding, computing value for distances 
     * possibly computing scaling factor for children and
     * computing radius of circles
     */
    private void FirstWalk(GenericOperator node)
    {
        IconProperties np = node.GetIcon().GetComponent<IconProperties>();
        np.d = 0;
        float s = 0;
        foreach(var child in node.Children)
        {
            FirstWalk(child);
            IconProperties cp = child.GetIcon().GetComponent<IconProperties>();
            np.d = Mathf.Max(np.d, cp.r);
            cp.a = Mathf.Atan(cp.r / (np.d + cp.r));
            s += cp.a;
        }
        //if(node.Children.Count > 0) s -= (s / node.Children.Count);
        AdjustChildren(np, s);
        SetRadius(np);
        //SetRadius(node, np);
    }

    // Adjusting the radii of the halfsectors of the children
    private void AdjustChildren(IconProperties np, float s)
    {
        if(s > Mathf.PI)
        {
            np.c = Mathf.PI / s;
            np.f = 0;
        }
        else
        {
            np.c = 1;
            np.f = Mathf.PI - s;
        }
    }

    //Setting the radius of a node
    private void SetRadius(IconProperties np)
    {
        np.r = Mathf.Max(np.d, _minRadius) + 2 * np.d;
    }

    //Setting the radius of a node
    /*private void SetRadius(GenericOperator nodeN, IconProperties np)
    {
        int numChildren = nodeN.Children.Count;
        float pi = Mathf.PI;
        float freeSpace = (numChildren == 0 ? 0 : np.f / numChildren);
        float previous = 0;
        float bx = 0, bz = 0;
        foreach (var child in nodeN.Children)
        {
            IconProperties cp = child.GetIcon().GetComponent<IconProperties>();
            pi += previous + cp.a + freeSpace;
            bx += (cp.r) * Mathf.Cos(pi);
            bz += (cp.r) * Mathf.Sin(pi);
            previous = cp.a;
        }

        if (numChildren != 0)
        {
            bx /= numChildren;
            bz /= numChildren;
        }

        np.rx = -bx;
        np.rz = -bz;

        pi = Mathf.PI;
        previous = 0;
        np.r = 0;

        foreach (var child in nodeN.Children)
        {
            IconProperties cp = child.GetIcon().GetComponent<IconProperties>();
            pi += previous + cp.a + freeSpace;
            float x = cp.r * Mathf.Cos(pi) - bx;
            float z = cp.r * Mathf.Sin(pi) - bz;
            float d = Mathf.Sqrt(x * x + z * z) + cp.r;
            np.r = Mathf.Max(np.r, (int)Mathf.Round(d));
            previous = cp.a;
        }
        if (np.r == 0) np.r = _minRadius + 2 * np.d;
    }*/

    // Computation of the absolute x and z coordinates for each node
    private void SecondWalk(GenericOperator nodeN, float x, float z, float l, float t)
    {
        IconProperties np = nodeN.GetIcon().GetComponent<IconProperties>();
        double y = 0;
        if (timeDependent) y = 2 - nodeN.normalizedTimeStamp;
        else y = 2 - np.normalizedDepth;
        Vector3 pos = new Vector3(x, (float)y, z);
        nodeN.GetIcon().transform.position = pos;
        float dd = l * np.d;
        float p = t + Mathf.PI;
        float freeSpace = (nodeN.Children.Count == 0 ? 0 : np.f / nodeN.Children.Count);
        float previous = 0;

        foreach(var child in nodeN.Children)
        {
            IconProperties cp = child.GetIcon().GetComponent<IconProperties>();
            float aa = np.c * cp.a;
            float rr = np.d * Mathf.Tan(aa) / (1 - Mathf.Tan(aa));
            p += previous + aa + freeSpace + freeSpace;
            float xx = (l * rr + dd) * Mathf.Cos(p);
            float zz = (l * rr + dd) * Mathf.Sin(p);
            previous = aa;
            SecondWalk(child, x + xx, z + zz, l * rr / cp.r, p);
        }
    }

    // Computation of the absolute x and z coordinates for each node
    /*private void SecondWalk(GenericOperator nodeN, float bx, float bz, float l, float t)
    {
        double y;
        IconProperties np = nodeN.GetIcon().GetComponent<IconProperties>();
        if(timeDependent) y = 2 - nodeN.normalizedTimeStamp;
        else y = 2 - np.normalizedDepth;
        float cosT = Mathf.Cos(t);
        float sinT = Mathf.Sin(t);
        float nx = bx + l * (np.rx * cosT - np.rz * sinT);
        float nz = bz + l * (np.rx * sinT + np.rz * cosT);
        Vector3 pos = new Vector3(nx, (float)y, bz);
        //if (nodeN.Parents != null && nodeN.Parents.Count != 0) graphSpace.DrawEdge(nodeN.Parents[0], nodeN);
        nodeN.GetIcon().transform.position = pos;
        float dd = (l/5) * np.d;
        float p = Mathf.PI;
        float freeSpace = np.f / (nodeN.Children.Count + 1);
        float previous = 0;
        foreach (var child in nodeN.Children)
        {
            IconProperties cp = child.GetIcon().GetComponent<IconProperties>();
            float aa = np.c * cp.a;
            float rr = np.d * Mathf.Tan(aa) / (1 - Mathf.Tan(aa));
            p += previous + aa + freeSpace + freeSpace;
            float xx = (l * rr + dd) * Mathf.Cos(p) + np.rx;
            float zz = (l * rr + dd) * Mathf.Sin(p) + np.rz;
            float x2 = xx * cosT - zz * sinT;
            float z2 = xx * sinT + zz * cosT;
            previous = aa;
            SecondWalk(child, bx + x2, bz + z2, l * rr / cp.r, p);
        }
    }*/

    private void CalculateRDT()
    {
        int x = 0;
        int counter = 0;
        Vector3 referencePoint = new Vector3();
        GraphSpaceController gsc = (GraphSpaceController)FindObjectOfType(typeof(GraphSpaceController));
        Vector3 dir = new Vector3();
        float distance = 0;
        for (int op= 0; op < observer.GetOperators().Count; op++)
        {
            if(observer.GetOperators()[op].Children!=null)
            {
                if(observer.GetOperators()[op].Children.Count > 0)
                {
                    float avgPt = 0;
                    float refPt = 0;
                    float sum = 0;
                    for(int i=0; i< observer.GetOperators()[op].Children.Count; i++)
                    {
                        sum += observer.GetOperators()[op].Children[i].GetIcon().transform.position.y;
                        x++;
                    }
                    avgPt = sum / observer.GetOperators()[op].Children.Count;
                    dir = observer.GetOperators()[op].GetIcon().transform.position - new Vector3(observer.GetOperators()[op].GetIcon().transform.position.x, avgPt, observer.GetOperators()[op].GetIcon().transform.position.z);
                    distance = Vector3.Distance(observer.GetOperators()[op].GetIcon().transform.position, new Vector3(observer.GetOperators()[op].GetIcon().transform.position.x, avgPt, observer.GetOperators()[op].GetIcon().transform.position.z));
                    refPt = (observer.GetOperators()[op].GetIcon().transform.position.y + avgPt) * height;
                    referencePoint = observer.GetOperators()[op].GetIcon().transform.position - dir * (distance * height);
                    referencePoints.Add(referencePoint);
                    for(int i=0; i < observer.GetOperators()[op].Children.Count; i++)
                    {
                        gsc.graphEdges[counter].positionCount = 3;
                        gsc.graphEdges[counter].SetPosition(0, observer.GetOperators()[op].GetIcon().transform.position);
                        gsc.graphEdges[counter].SetPosition(1, referencePoint);
                        gsc.graphEdges[counter].SetPosition(2, observer.GetOperators()[op].Children[i].GetIcon().transform.position);
                        counter++;
                    }
                }
            }
        }
        Debug.Log("end");
    }

    private void NormalizeDepth()
    {
        float minDepth = 1;
        float maxDepth = 0;
        float depth = 0;
        foreach(var op in observer.GetOperators())
        {
            if (maxDepth < op.GetIcon().GetComponent<IconProperties>().depth) maxDepth = op.GetIcon().GetComponent<IconProperties>().depth;
        }
        foreach(var op in observer.GetOperators())
        {
            depth = op.GetIcon().GetComponent<IconProperties>().depth;
            op.GetIcon().GetComponent<IconProperties>().normalizedDepth = 2 * ((depth - minDepth) / (maxDepth - minDepth));
        }
    }
}
