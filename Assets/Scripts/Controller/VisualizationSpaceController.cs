using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizationSpaceController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void installNewVisualization(GenericOperator op)
    {
        op.getVisualization().gameObject.transform.parent = gameObject.transform;
        op.getVisualization().gameObject.transform.localPosition = new Vector3(0, 0, -0.01f);
    }
}
