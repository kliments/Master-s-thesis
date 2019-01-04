using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDirectedConeTree : GeneralLayoutAlgorithm {
    public bool start, reposition, putOldRot;
    public Transform defaultParent;
    private Observer observer;
    private ConeTreeAlgorithm coneTreeAlg;
    private GenericOperator _root;
    private Vector3 _anchor;
	// Use this for initialization
	void Start () {
        coneTreeAlg = (ConeTreeAlgorithm)FindObjectOfType(typeof(ConeTreeAlgorithm));
        observer = (Observer)FindObjectOfType(typeof(Observer));
	}
	
	// Update is called once per frame
	void Update () {
		if(start)
        {
            start = false;
            StartAlgorithm();
            reposition = true;
        }
    }

    public override void StartAlgorithm()
    {
        //get root and anchor of tree
        _root = observer.GetOperators()[0];
        _anchor = _root.GetIcon().transform.position;

        Vector3 rootPos = _root.GetIcon().transform.position;
        float dist = 0;
        //Normalize time stamps between 0 and 2 for Y position
        observer.NormalizeTimeStamps();
        //Normalize depth between 0-2
        coneTreeAlg.NormalizeDepth();
        //First walk of cone tree algorithm
        coneTreeAlg.FirstWalk(_root);
        //Second walk of cone tree algorithm
        coneTreeAlg.SecondWalk(_root, _anchor.x, _anchor.z, 0.5f, 0);
        //set the new Cone Tree position of each node
        foreach(var op in observer.GetOperators())
        {
            op.GetIcon().transform.position = op.GetIcon().GetComponent<IconProperties>().newPos;
        }

        /*iterate over root's children to set as transform parent to
          all their children, grandchildren and so on */
        foreach (var rootChild in _root.Children)
        {
            DFS(rootChild);
            rootChild.GetIcon().GetComponent<IconProperties>().oldPos = rootChild.GetIcon().transform.position;
            //get the biggest distance, to position all root's children on sphere with radius of the distance
            if (Vector3.Distance(_root.GetIcon().transform.position, rootChild.GetIcon().transform.position) > dist) dist = Vector3.Distance(_root.GetIcon().transform.position, rootChild.GetIcon().transform.position);
        }
        //set location of root to zero for proper calculation of points of sphere
        _root.GetIcon().transform.position = Vector3.zero;
        Vector3[] rootChildrenPos = PointsOnSphere(_root.Children.Count);
        int i = 0;
        //Place root's children on a sphere with radius of the biggest distance
        foreach(var pos in rootChildrenPos)
        {
            _root.Children[i].GetIcon().transform.parent = _root.GetIcon().transform;
            _root.Children[i].GetIcon().transform.position = pos * dist;
            i++;
        }

        //put back root to its old position
        _root.GetIcon().transform.position = rootPos;
        
        //rotate root's children in direction of root to child, save root's children's children location and rotate back
        foreach(var child in _root.Children)
        {
            child.GetIcon().GetComponent<IconProperties>().newPos = child.GetIcon().transform.position;
            child.GetIcon().GetComponent<IconProperties>().repos = true;

            var oldRot = child.GetIcon().transform.rotation;
            var dir = _root.GetIcon().transform.position - child.GetIcon().transform.position;
            Quaternion rotation = Quaternion.FromToRotation(child.GetIcon().transform.up, dir);
            child.GetIcon().transform.rotation = rotation;

            //save position of grand children for animated positioning
            foreach(Transform grandChild in child.GetIcon().transform)
            {
                if (grandChild.GetComponent<IconProperties>() == null) continue;
                grandChild.GetComponent<IconProperties>().newPos = grandChild.transform.position;
                grandChild.GetComponent<IconProperties>().repos = true;
            }
            //set back position
            child.GetIcon().transform.position = child.GetIcon().GetComponent<IconProperties>().oldPos;
            //set back rotation
            child.GetIcon().transform.rotation = Quaternion.identity;
            //unparent from root's transform
            child.GetIcon().transform.parent = defaultParent;
            //unparent grandchildren from children transform
            for(int c= child.GetIcon().transform.childCount - 1; c>=0; c--)
            {
                if (child.GetIcon().transform.GetChild(c).GetComponent<IconProperties>() != null) child.GetIcon().transform.GetChild(c).transform.parent = defaultParent;
            }
            /*foreach(Transform grandChild in child.GetIcon().transform)
            {
                if (grandChild.GetComponent<IconProperties>() == null) continue;
                grandChild.parent = defaultParent;
            }*/
        }


        //set current algorithm to ForceDirectedConeTree
        GetComponent<LayoutAlgorithm>().currentLayout = this;
        GetComponent<TwoDimensionalProjection>().SetPlane();
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
}
