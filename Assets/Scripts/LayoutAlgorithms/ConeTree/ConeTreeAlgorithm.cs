using Assets.Scripts.Model;
using UnityEngine;

public class ConeTreeAlgorithm : MonoBehaviour {
    public Observer observer;
    public bool runConeTree;

    private int _minRadius = 1;
    private Vector3 _anchor;
    private GenericOperator root;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(runConeTree)
        {
            root = observer.GetOperators()[0];
            _anchor = root.GetIcon().transform.position;
            runConeTree = false;
            Layout(root, _anchor.x, _anchor.z);
        }
	}

    private void Layout(GenericOperator root, float x, float z)
    {
        FirstWalk(root);
        SecondWalk(root, x, z, 1f, 0f);
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
        AdjustChildren(np, s);
        SetRadius(np);
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

    // Computation of the absolute x and z coordinates for each node
    private void SecondWalk(GenericOperator nodeN, float x, float z, float l, float t)
    {
        IconProperties np = nodeN.GetIcon().GetComponent<IconProperties>();
        Vector3 pos = new Vector3(x, 2, z);
        l = np.depth / (np.depth + 1);

        nodeN.GetIcon().transform.position = pos;
        float dd = l * np.d;
        float p = t + Mathf.PI;
        float freeSpace = (nodeN.Children.Count == 0 ? 0 : np.f / (nodeN.Children.Count + 1));
        float previous = 0;

        foreach(var child in nodeN.Children)
        {
            IconProperties cp = child.GetIcon().GetComponent<IconProperties>();
            float aa = np.c * cp.a;
            float rr = np.d * Mathf.Tan(aa) / (1 - Mathf.Tan(aa));
            p += previous + aa + freeSpace;
            float xx = (l * rr + dd) * Mathf.Cos(p);
            float zz = (l * rr + dd) * Mathf.Sin(p);
            previous = aa;
            SecondWalk(child, x + xx, z + zz, l * np.c, p);
        }
    }
}
