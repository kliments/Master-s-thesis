using Assets.Scripts.Model;
using System.Collections.Generic;
using UnityEngine;

/*
 * Calculates the position of the nodes in Cone Tree Algorithm
 */
public class ConeTreeAlgorithm : GeneralLayoutAlgorithm
{
    public bool reposition, startConeTree, test;
    public float height;

    private float _minRadius = 0.25f;
    private float _minDepth, _maxDepth;
    private Vector3 _anchor;
    private GenericOperator root;
    private float[] _timeStamps;
    private GraphSpaceController _graphSpace;
    private bool calculationFinished;
    private LayoutAlgorithm algorithm;
    // Use this for initialization
    void Start()
    {
        _graphSpace = GameObject.Find("GraphSpace").GetComponent<GraphSpaceController>();
        algorithm = GetComponent<LayoutAlgorithm>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void StartAlgorithm()
    {
        //don't start if another algorithm is in process
        if (!algorithm.currentLayout.AlgorithmHasFinished()) return;
        //set flag that this algorithm is running
        SetStart();
        if (observer == null) observer = (Observer)FindObjectOfType(typeof(Observer));
        //set 2 lines, in case previous algorithm had changed it to 3 (ex. RDT)
        if (GetComponent<LayoutAlgorithm>().currentLayout!=this)
        {
            PlaceEdges();
        }

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

        base.ColorEdges();
        //set flag for this algorithm has finished
        SetFinish();
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
        y = 2 - np.normalizedDepth;
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
    public override void PreScanCalculation()
    {
        StartAlgorithm();
        foreach(var op in observer.GetOperators())
        {
            op.GetIcon().transform.position = op.GetIcon().GetComponent<IconProperties>().newPos;
        }
        PlaceEdges();
    }
}