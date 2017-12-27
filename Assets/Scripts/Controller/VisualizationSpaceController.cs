using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizationSpaceController : MonoBehaviour {

	// Use this for initialization
	private void Start () {
		
	}
	
	// Update is called once per frame
	private void Update () {
		
	}

    public void InstallNewVisualization(GenericOperator op)
    {
        if (gameObject.transform.childCount > 0)
        {
            foreach (var vis in transform.GetComponentsInChildren<GenericVisualization>())
            {
                vis.gameObject.transform.parent = vis.GetOperator().gameObject.transform;
                vis.gameObject.SetActive(false);
            }
        }
        op.GetVisualization().gameObject.SetActive(true);
        op.GetVisualization().gameObject.transform.parent = gameObject.transform;
        //op.getVisualization().gameObject.transform.localPosition = new Vector3();
    }
}
