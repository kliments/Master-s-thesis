using Assets.Scripts.Model;
using System.Collections.Generic;
using UnityEngine;

/*
 * Calculates the position of the nodes in Cone Tree Algorithm
 * Also implements time-dependent positioning of the nodes, by stretching the base of a cone along the Y axis
 * Also implements RDT (Reconfigurable Disk Trees) algorithm for reduction of edge crossings
 */
public class ConeTreeAlgorithm : GeneralLayoutAlgorithm
{
    public Observer observer;
    public bool RDT, reposition, startConeTree;
    public float height;

    private float _minRadius = 0.25f;
    private float _minDepth, _maxDepth;
    private Vector3 _anchor;
    private GenericOperator root;
    private float[] _timeStamps;
    private GraphSpaceController _graphSpace;
    private bool calculationFinished;
    // Use this for initialization
    void Start()
    {
        _graphSpace = GameObject.Find("GraphSpace").GetComponent<GraphSpaceController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<LayoutAlgorithm>().currentLayout == this)
        {
            if (calculationFinished && RDT)
            {
                if (AllNodesPlaced()) calculationFinished = false;
                CalculateRDT();
            }
        }
    }

    public override void StartAlgorithm()
    {
        //don't start if another algorithm is in process
        if (!GetComponent<LayoutAlgorithm>().currentLayout.AlgorithmHasFinished()) return;
        //set flag that this algorithm is running
        SetStart();

        //get root and anchor of tree
        root = observer.GetOperators()[0];
        _anchor = root.GetIcon().transform.position;
        //Normalize time stamps between 0 and 2 for Y position
        observer.NormalizeTimeStamps();
        //start Layout algorithm
        Layout(root, _anchor.x, _anchor.z);
        //set current algorithm to ConeTree
        GetComponent<LayoutAlgorithm>().currentLayout = this;
        calculationFinished = true;

        //set flag for this algorithm has finished
        SetFinish();
    }

    public void SetRDT()
    {
        RDT = true;
    }

    public void UnsetRDT()
    {
        RDT = false;
    }

    // Checks whether all nodes are placed in its place
    private bool AllNodesPlaced()
    {
        foreach (var node in observer.GetOperators())
        {
            if (node.GetIcon().GetComponent<IconProperties>().repos) return false;
        }
        return true;
    }
    private void Layout(GenericOperator root, float x, float z)
    {
        ResetValues();
        NormalizeDepth();
        FirstWalk(root);
        SecondWalk(root, x, z, 0.5f, 0f);
        reposition = true;
        GetComponent<TwoDimensionalProjection>().SetPlane();
    }

    /* Bottom up proceeding, computing value for distances 
     * possibly computing scaling factor for children and
     * computing radius of circles
     */
    public void FirstWalk(GenericOperator node)
    {
        IconProperties np = node.GetIcon().GetComponent<IconProperties>();
        np.d = 0;
        float s = 0;
        foreach (var child in node.Children)
        {
            if (child.GetType() == typeof(NewOperator)) continue;
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
        if (s > Mathf.PI)
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
        //normalization of radius length
        np.r = (Mathf.Max(np.d, _minRadius) + np.d);
    }

    // Computation of the absolute x, y and z coordinates for each node
    public void SecondWalk(GenericOperator nodeN, float x, float z, float l, float t)
    {
        IconProperties np = nodeN.GetIcon().GetComponent<IconProperties>();
        double y = 0;
        if (GetTemporal()) y = 2 - nodeN.normalizedTimeStamp;
        else y = 2 - np.normalizedDepth;
        Vector3 pos = new Vector3(x, (float)y, z);
        //nodeN.GetIcon().transform.position = pos;
        np.newPos = pos;
        np.repos = true;
        float dd = l * np.d;
        float p = t + Mathf.PI;
        float freeSpace = (nodeN.Children.Count == 0 ? 0 : np.f / nodeN.Children.Count);
        float previous = 0;

        if(nodeN.Children.Count == 1)
        {
            IconProperties cp = nodeN.Children[0].GetIcon().GetComponent<IconProperties>();
            float aa = np.c * cp.a;
            float rr = np.d * Mathf.Tan(aa) / (1 - Mathf.Tan(aa));
            p += previous + aa + freeSpace + freeSpace;
            previous = aa;
            SecondWalk(nodeN.Children[0], np.newPos.x, np.newPos.z, l * rr / cp.r, p);
        }
        else
        {
            foreach (var child in nodeN.Children)
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
    }

    /*
     * Calculates the reference point between parend and child node for RDT algorithm
     */
    public void CalculateRDT()
    {
        int x = 0;
        GraphSpaceController gsc = (GraphSpaceController)FindObjectOfType(typeof(GraphSpaceController));
        for (int op = 0; op < observer.GetOperators().Count; op++)
        {
            if (observer.GetOperators()[op].Children != null)
            {
                if (observer.GetOperators()[op].Children.Count > 0)
                {
                    Vector3 avgPt = new Vector3();
                    Vector3 refPt = new Vector3();
                    Vector3 sum = new Vector3();
                    for (int i = 0; i < observer.GetOperators()[op].Children.Count; i++)
                    {
                        sum += observer.GetOperators()[op].Children[i].GetIcon().transform.position;
                        x++;
                    }
                    avgPt = sum / observer.GetOperators()[op].Children.Count;
                    avgPt.x = observer.GetOperators()[op].GetIcon().transform.position.x;
                    avgPt.z = observer.GetOperators()[op].GetIcon().transform.position.z;
                    refPt = Vector3.Lerp(observer.GetOperators()[op].GetIcon().transform.position, avgPt, height);
                    observer.GetOperators()[op].GetIcon().GetComponent<IconProperties>().refPoint = refPt;
                    for (int i = 0; i < observer.GetOperators()[op].Children.Count; i++)
                    {
                        observer.GetOperators()[op].Children[i].GetComponent<LineRenderer>().positionCount = 3;
                        observer.GetOperators()[op].Children[i].GetComponent<LineRenderer>().SetPosition(0, observer.GetOperators()[op].GetIcon().transform.position);
                        observer.GetOperators()[op].Children[i].GetComponent<LineRenderer>().SetPosition(1, refPt);
                        observer.GetOperators()[op].Children[i].GetComponent<LineRenderer>().SetPosition(2, observer.GetOperators()[op].Children[i].GetIcon().transform.position);
                    }
                }
            }
        }
    }
    
    //Normalizes depth between 0-2
    public void NormalizeDepth()
    {
        int i = 0;
        _minDepth = 1;
        _maxDepth = 0;
        float depth = 0;
        List<GenericOperator> list = observer.GetOperators();
        foreach (var op in observer.GetOperators())
        {
            if (_maxDepth < op.GetIcon().GetComponent<IconProperties>().depth) _maxDepth = op.GetIcon().GetComponent<IconProperties>().depth;
            i++;
        }
        i = 0;
        foreach (var op in observer.GetOperators())
        {
            depth = op.GetIcon().GetComponent<IconProperties>().depth;
            op.GetIcon().GetComponent<IconProperties>().normalizedDepth = 2 * ((depth - _minDepth) / (_maxDepth - _minDepth));
            i++;
        }
    }

    void ResetValues()
    {
        foreach(var op in observer.GetOperators())
        {
            op.GetIcon().GetComponent<IconProperties>().Reset();
        }
    }


    void OnEnable()
    {
        subscriber.addListener(this);
    }
}