using Assets.Scripts.Model;
using UnityEngine;

public class ConeTreeAlgorithm : MonoBehaviour {
    public Observer observer;
    public bool runConeTree;

    private float _minRadius = 0.5f;
    private Vector3 _anchor;
    private GenericOperator root;
    private float[] _timeStamps;
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
            observer.NormalizeTimeStamps();
            Layout(root, _anchor.x, _anchor.z);
        }
	}

    private void Layout(GenericOperator root, float x, float z)
    {
        FirstWalk(root);
        //SecondWalk(root, x, z, 1f, 0f);
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
        //if(node.Children.Count > 0) s -= (s / node.Children.Count);
        AdjustChildren(np, s);
        //SetRadius(np);
        SetRadius(node, np);
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
    /*private void SetRadius(IconProperties np)
    {
        np.r = Mathf.Max(np.d, _minRadius) + 2 * np.d;
    }*/

    //Setting the radius of a node
    private void SetRadius(GenericOperator nodeN, IconProperties np)
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
    }

    // Computation of the absolute x and z coordinates for each node
    /*private void SecondWalk(GenericOperator nodeN, float x, float z, float l, float t)
    {
        IconProperties np = nodeN.GetIcon().GetComponent<IconProperties>();
        float y = 5*(1 + (1 / np.depth));
        Vector3 pos = new Vector3(x, y, z);
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
    }*/

    // Computation of the absolute x and z coordinates for each node
    private void SecondWalk(GenericOperator nodeN, float bx, float bz, float l, float t)
    {
        IconProperties np = nodeN.GetIcon().GetComponent<IconProperties>();
        double y = 2 - nodeN.normalizedTimeStamp;
        float cosT = Mathf.Cos(t);
        float sinT = Mathf.Sin(t);
        float nx = bx + l * (np.rx * cosT - np.rz * sinT);
        float nz = bz + l * (np.rx * sinT + np.rz * cosT);
        Vector3 pos = new Vector3(nx, (float)y, bz); ;
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
    }
}
