using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeBurst : GeneralLayoutAlgorithm {
    public bool start;
    public Transform defaultParent;
    private ConeTreeAlgorithm coneTreeAlg;
    private GenericOperator _root;
    private Vector3 _anchor;
    private LayoutAlgorithm algorithm;
    // Use this for initialization
    void Start () {
        coneTreeAlg = (ConeTreeAlgorithm)FindObjectOfType(typeof(ConeTreeAlgorithm));
        observer = (Observer)FindObjectOfType(typeof(Observer));
        algorithm = GetComponent<LayoutAlgorithm>();
    }
	
	// Update is called once per frame
	void Update () {
        if(start)
        {
            start = false;
            StartAlgorithm();
        }
    }

    public override void StartAlgorithm()
    {
        //stop previous algorithm
        if (!algorithm.currentLayout.Equals(this))
        {
            algorithm.currentLayout.SetFinish();
        }
        //set flag that this algorithm is running
        SetStart();

        if (observer == null) observer = (Observer)FindObjectOfType(typeof(Observer));
        //set 2 lines, in case previous algorithm had changed it to 3 (ex. RDT)
        if (GetComponent<LayoutAlgorithm>().currentLayout != this)
        {
            foreach (var op in observer.GetOperators())
            {
                if (op.Parents != null)
                {
                    if (op.Parents.Count != 0)
                    {
                        op.GetComponent<LineRenderer>().positionCount = 2;
                        op.GetComponent<LineRenderer>().SetPositions(new Vector3[] { op.Parents[0].GetIcon().transform.position, op.GetIcon().transform.position });
                    }
                }
            }
        }
        //get root and anchor of tree
        _root = observer.GetOperators()[0];
        _anchor = _root.GetIcon().GetComponent<IconProperties>().newPos;

        float dist = 0;
        //Normalize time stamps between 0 and 2 for Y position
        observer.NormalizeTimeStamps();
        //Normalize depth between 0-2
        coneTreeAlg.NormalizeDepth();
        foreach (var op in observer.GetOperators())
        {
            op.GetIcon().GetComponent<IconProperties>().originalPos = op.GetIcon().transform.position;
        }
        //First walk of cone tree algorithm
        coneTreeAlg.FirstWalk(_root);
        //Second walk of cone tree algorithm
        coneTreeAlg.SecondWalk(_root, _anchor.x, _anchor.z, 0.5f, 0);
        //set the new Cone Tree position of each node
        foreach(var op in observer.GetOperators())
        {
            op.GetIcon().transform.position = op.GetIcon().GetComponent<IconProperties>().newPos;
            op.GetIcon().GetComponent<IconProperties>().repos = false;
        }
        /*iterate over root's children to set as transform parent to
          all their children, grandchildren and so on */
        foreach (var rootChild in _root.Children)
        {
            DFS(rootChild);
            rootChild.GetIcon().GetComponent<IconProperties>().oldPos = rootChild.GetIcon().transform.position;
            //get the biggest distance, to position all root's children on sphere with radius of the distance
            if (Vector3.Distance(_root.GetIcon().transform.position, rootChild.GetIcon().transform.position) > dist) dist = Vector3.Distance(_root.GetIcon().transform.position, rootChild.GetIcon().transform.position);
            rootChild.GetIcon().transform.parent = _root.GetIcon().transform;
        }
        //set location of root to zero for proper calculation of points of sphere
        _root.GetIcon().transform.position = Vector3.zero;
        Vector3[] rootChildrenPos = PointsOnSphere(_root.Children.Count);
        int i = 0;
        //Place root's children on a sphere with radius of the biggest distance
        foreach(var pos in rootChildrenPos)
        {
            _root.Children[i].GetIcon().transform.position = pos * dist;
            i++;
        }

        //put back root to its old position
        _root.GetIcon().transform.position = _anchor;
        _root.GetIcon().GetComponent<IconProperties>().newPos = _anchor;
        _root.GetIcon().transform.position = _anchor;

        //rotate root's children in direction of root to child, save root's children's children location and rotate back
        foreach (var child in _root.Children)
        {
            child.GetIcon().GetComponent<IconProperties>().newPos = child.GetIcon().transform.position;

            var oldRot = child.GetIcon().transform.rotation;
            var dir = _anchor - child.GetIcon().transform.position;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, dir);
            child.GetIcon().transform.rotation = rotation;

            //save position of grand children for animated positioning
            foreach(Transform grandChild in child.GetIcon().transform)
            {
                if (grandChild.GetComponent<IconProperties>() == null) continue;
                grandChild.GetComponent<IconProperties>().newPos = grandChild.position;
            }
            //set back rotation
            child.GetIcon().transform.rotation = Quaternion.identity;
            //set back position
            child.GetIcon().transform.position = child.GetIcon().GetComponent<IconProperties>().originalPos;
            //unparent from root's transform
            child.GetIcon().transform.parent = defaultParent;

            //return grandchildren's position to original
            foreach (Transform grandChild in child.GetIcon().transform)
            {
                if (grandChild.GetComponent<IconProperties>() == null) continue;
                grandChild.position = grandChild.GetComponent<IconProperties>().originalPos;
            }
            //unparent grandchildren from children transform
            for (int c= child.GetIcon().transform.childCount - 1; c>=0; c--)
            {
                if (child.GetIcon().transform.GetChild(c).GetComponent<IconProperties>() != null)
                {
                    child.GetIcon().transform.GetChild(c).transform.parent = defaultParent;
                }
            }
        }
        foreach (var op in observer.GetOperators())
        {
            op.GetIcon().transform.position = op.GetIcon().GetComponent<IconProperties>().newPos;
        }

        GetComponent<TwoDimensionalProjection>().SetPlane();
        GetComponent<LayoutAlgorithm>().currentLayout = this;

        base.ColorEdges();
        //set flag for this algorithm has finished
        SetFinish();
    }

    void DFS(GenericOperator rootChild)
    {
        int i = 0;
        Queue<GenericOperator> frontier = new Queue<GenericOperator>();
        GenericOperator currentOp;
        frontier.Enqueue(rootChild);
        while(frontier.Count > 0)
        {
            currentOp = frontier.Dequeue();
            if (currentOp != rootChild)
            {
                currentOp.GetIcon().transform.parent = rootChild.GetIcon().transform;
                i++;
            }
            foreach(var nextOp in currentOp.Children)
            {
                frontier.Enqueue(nextOp);
            }
        }
    }

    //returns equidistant points on sphere with number of points as input
    Vector3[] PointsOnSphere(int n)
    {
        List<Vector3> upts = new List<Vector3>();
        float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
        float off = 2.0f / n;
        float x = 0;
        float y = 0;
        float z = 0;
        float r = 0;
        float phi = 0;

        for (var k = 0; k < n; k++)
        {
            y = k * off - 1 + (off / 2);
            r = Mathf.Sqrt(1 - y * y);
            phi = k * inc;
            x = Mathf.Cos(phi) * r;
            z = Mathf.Sin(phi) * r;

            upts.Add(new Vector3(x, y, z));
        }
        Vector3[] pts = upts.ToArray();
        return pts;
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
            if (op.GetComponent<LineRenderer>() != null)
            {
                op.GetComponent<LineRenderer>().positionCount = 2;
                op.GetComponent<LineRenderer>().SetPosition(0, op.GetIcon().transform.position);
                op.GetComponent<LineRenderer>().SetPosition(1, op.Parents[0].GetIcon().transform.position);
            }
        }
    }
}
