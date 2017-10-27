using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterplotVisualization : GenericVisualization {
    private Renderer pointRenderer;
    private Material pointMaterial;
    private GameObject pointPrimitive;
    private Color pointColor = Color.red;
    
    public override void createVisualization()
    {
        pointPrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pointPrimitive.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        pointRenderer = pointPrimitive.GetComponent<Renderer>();
        pointRenderer.material = new Material(Shader.Find("Specular"));
        pointRenderer.material.color = pointColor;

        if (getOperator().getRawInputData() == null)
            return;

        List<Vector3> dataPoints = ((SimpleDatamodel)getOperator().getRawInputData()).getCoords();

        



        Rigidbody body = this.gameObject.GetComponent<Rigidbody>();
        if (body != null) { body.angularVelocity = Vector3.zero; }
        this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        List<CombineInstance> combine = new List<CombineInstance>();

        int vertexCount = 0;


        for (int index = 0; index < dataPoints.Count; index++)
        {
            GameObject datapoint = Instantiate(pointPrimitive);
            datapoint.transform.parent = getOperator().visualization.gameObject.transform;
            datapoint.transform.localPosition = new Vector3(dataPoints[index].x - 0.5f, dataPoints[index].y - 0.5f, dataPoints[index].z - 0.5f);

            MeshFilter dataPointMeshFilter = datapoint.GetComponent<MeshFilter>();

            if (vertexCount + dataPointMeshFilter.mesh.vertexCount > 65000 || index == dataPoints.Count - 1)
            {
                GameObject submesh = new GameObject();
                submesh.AddComponent<MeshFilter>();
                submesh.AddComponent<MeshRenderer>();
                submesh.GetComponent<Renderer>().sharedMaterial = pointRenderer.material;
                submesh.GetComponent<Renderer>().materials[0] = pointRenderer.material;
                submesh.name = "meshChunk";

                submesh.transform.parent = transform;
                submesh.transform.GetComponent<MeshFilter>().mesh = new Mesh();
                submesh.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine.ToArray());
                combine = new List<CombineInstance>();
                submesh.transform.parent = getOperator().visualization.gameObject.transform;

                vertexCount = 0;
            }

            vertexCount += dataPointMeshFilter.sharedMesh.vertexCount;
            CombineInstance combinInstance = new CombineInstance();
            combinInstance.mesh = dataPointMeshFilter.sharedMesh;
            combinInstance.transform = dataPointMeshFilter.transform.localToWorldMatrix;
            combine.Add(combinInstance);
            
            GameObject.Destroy(datapoint);
        }
        GameObject.Destroy(pointPrimitive);
    }



}
