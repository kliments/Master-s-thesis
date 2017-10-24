using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;

public class GraphSpaceController : MonoBehaviour {



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void installNewIcon(GenericOperator op)
    {
        op.getIcon().gameObject.transform.parent = gameObject.transform;
        op.getIcon().gameObject.transform.localPosition = new Vector3(0,0,-0.01f);
    }
}
