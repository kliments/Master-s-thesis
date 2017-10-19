﻿using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterplotOperator : GenericOperator
{
    private Renderer pointRenderer;
    private Material pointMaterial;
    private GameObject pointPrimitive;
    private Color pointColor = Color.red;

    void Start()
    {
        pointPrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pointPrimitive.transform.localScale = new Vector3(0.01f,0.01f,0.01f);
        pointRenderer = pointPrimitive.GetComponent<Renderer>();
        pointRenderer.material = new Material(Shader.Find("Specular"));
        pointRenderer.material.color = pointColor;
    }

    public override bool process()
    {
        if(getRawInputData() == null)
            return false;

        List<Vector3> dataPoints = ((SimpleDatamodel) getRawInputData()).getCoords();
        
        for (int i = 0; i < dataPoints.Count; i++)
        {
            GameObject dat = Instantiate(pointPrimitive);
            dat.transform.parent = transform;
            dat.transform.localPosition = dataPoints[i];
        }

        return true;
    }
    
}
