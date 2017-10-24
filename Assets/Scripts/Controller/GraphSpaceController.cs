using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;

public class GraphSpaceController : MonoBehaviour {
    private Vector3 scale = new Vector3(0.03f, 0.06f, 0);
    private float xOffset = -0.4f;

   
    public void installNewIcon(GenericOperator op)
    {
        op.getIcon().gameObject.transform.parent = GameObject.Find("ControlWall").transform;
        op.getIcon().gameObject.transform.localPosition = new Vector3(xOffset, 0, -0.51f);
        op.getIcon().gameObject.transform.localScale = scale;
        xOffset += 0.1f;
    }
}
