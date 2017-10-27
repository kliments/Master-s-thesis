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
        if (gameObject.transform.childCount > 0)
        {
            foreach (GenericVisualization vis in transform.GetComponentsInChildren<GenericVisualization>())
            {
                vis.gameObject.transform.parent = vis.getOperator().gameObject.transform;
                vis.gameObject.SetActive(false);
            }
        }
        op.getVisualization().gameObject.SetActive(true);
        op.getVisualization().gameObject.transform.parent = gameObject.transform;
        //op.getVisualization().gameObject.transform.localPosition = new Vector3();
    }
}
