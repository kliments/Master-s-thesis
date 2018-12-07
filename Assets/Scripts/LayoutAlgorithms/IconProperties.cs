﻿using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconProperties : MonoBehaviour {

    #region ForceDirectedParameters
    public Vector3 acceleration;
    #endregion

    #region ConeTreeParameters
    // distance from parent node
    public float d;
    // radius of subtree
    public float r;
    // angle of halfsector
    public float a;
    // scaling factor
    public float c;
    // free angle
    public float f;

    //
    public float rx, rz;
    #endregion

    //original, old and new positions for smooth transition of nodes
    public Vector3 originalPos, oldPos, newPos;
    public bool repos;
    public float depth, normalizedDepth;
    public Camera myCamera;
    public Transform child;
    private Vector3 _target;

    private Vector3 lookPos;
    private Quaternion rotation;
	// Use this for initialization
	void Start () {
        myCamera = Camera.main;
        if(transform.GetChild(0).name != "Plane") child = transform.GetChild(0);
        else child = transform.GetChild(1);
    }
	
	// Update is called once per frame
	void Update () {
        _target = new Vector3(myCamera.transform.position.x, myCamera.transform.position.y, myCamera.transform.position.z);
        child.LookAt(_target);
        /*lookPos = myCamera.transform.position - transform.position;
        rotation = Quaternion.LookRotation(lookPos);
        rotation.x = 0;
        rotation.z = 0;
        child.localRotation = rotation;*/
    }

    public void ApplyForce(Vector3 force)
    {
        acceleration += force;
    }
}
