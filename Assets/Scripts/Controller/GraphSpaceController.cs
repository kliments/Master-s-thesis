using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;

public class GraphSpaceController : MonoBehaviour {
    private Vector3 _scale = new Vector3(0.03f, 0.06f, 0);
    private float _xOffset = -0.4f;
    private float _yOffsetUp = 0.2f;
    private float _yOffsetDown = -0.2f;

   
    public void InstallNewIcon(GenericOperator op)
    {
        //TODO nicht den placement counter vom neuen OP abgreifen, sondern vom gedrückten!
        if (op.PlacementCounter == 0 || op.PlacementCounter == 1)
        {
            op.GetIcon().gameObject.transform.parent = GameObject.Find("ControlWall").transform;
            op.GetIcon().gameObject.transform.localPosition = new Vector3(_xOffset, 0, -0.51f);
            op.GetIcon().gameObject.transform.localScale = _scale;
            _xOffset += 0.1f;
        }

        if (op.PlacementCounter == 2)
        {
            op.GetIcon().gameObject.transform.parent = GameObject.Find("ControlWall").transform;
            op.GetIcon().gameObject.transform.localPosition = new Vector3(_xOffset, _yOffsetUp, -0.51f);
            op.GetIcon().gameObject.transform.localScale = _scale;
            _xOffset += 0.1f;
        }

        if (op.PlacementCounter == 3)
        {
            op.GetIcon().gameObject.transform.parent = GameObject.Find("ControlWall").transform;
            op.GetIcon().gameObject.transform.localPosition = new Vector3(_xOffset, _yOffsetDown, -0.51f);
            op.GetIcon().gameObject.transform.localScale = _scale;
            _xOffset += 0.1f;
        }

        if (op.PlacementCounter > 3)
        {
            throw new Exception("kein platz mehr, sorry :(");
        }
        
    }
}
