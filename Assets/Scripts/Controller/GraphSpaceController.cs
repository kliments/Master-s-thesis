using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;

public class GraphSpaceController : MonoBehaviour {
    private Vector3 _scale = new Vector3(0.03f, 0.06f, 0);
    private float _xOffset = -0.4f;

   
    public void InstallNewIcon(GenericOperator op)
    {
        op.GetIcon().gameObject.transform.parent = GameObject.Find("ControlWall").transform;
        op.GetIcon().gameObject.transform.localPosition = new Vector3(_xOffset, 0, -0.51f);
        op.GetIcon().gameObject.transform.localScale = _scale;
        _xOffset += 0.1f;
    }
}
