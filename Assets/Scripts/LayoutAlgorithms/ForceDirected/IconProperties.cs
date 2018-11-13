using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconProperties : MonoBehaviour {
    
    public Vector3 acceleration;
    public Camera myCamera;
    public Transform child;
    private Vector3 _target;
	// Use this for initialization
	void Start () {
        myCamera = Camera.main;
        if(transform.GetChild(0).name != "Plane") child = transform.GetChild(0);
        else child = transform.GetChild(1);
    }
	
	// Update is called once per frame
	void Update () {
        var lookPos = myCamera.transform.position - transform.position;
        var rotation = Quaternion.LookRotation(lookPos);
        child.localRotation = rotation;
    }

    public void ApplyForce(Vector3 force)
    {
        acceleration += force;
    }
}
