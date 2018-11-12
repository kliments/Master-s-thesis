using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconProperties : MonoBehaviour {
    
    public Vector3 acceleration;
    public GenericOperator op;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ApplyForce(Vector3 force)
    {
        acceleration += force;
    }
}
