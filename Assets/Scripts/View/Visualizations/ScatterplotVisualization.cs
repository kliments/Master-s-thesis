using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterplotVisualization : GenericVisualization {
    private Renderer _pointRenderer;
    private Material _pointMaterial;
    private GameObject _pointPrimitive;
    private Color _pointColor = Color.red;
    
    public override void CreateVisualization()
    {
        _pointPrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _pointPrimitive.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        _pointRenderer = _pointPrimitive.GetComponent<Renderer>();
        _pointRenderer.material = new Material(Shader.Find("Specular"));
        _pointRenderer.material.color = _pointColor;

        if (GetOperator().GetRawInputData() == null)
            return;

        var dataPoints = ((SimpleDatamodel)GetOperator().GetRawInputData()).GetCoords();

        



        var body = this.gameObject.GetComponent<Rigidbody>();
        if (body != null) { body.angularVelocity = Vector3.zero; }
        this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        var combine = new List<CombineInstance>();

        var vertexCount = 0;


        for (var index = 0; index < dataPoints.Count; index++)
        {
            var datapoint = Instantiate(_pointPrimitive);
            datapoint.transform.parent = GetOperator().Visualization.gameObject.transform;
            datapoint.transform.localPosition = new Vector3(dataPoints[index].x - 0.5f, dataPoints[index].y - 0.5f, dataPoints[index].z - 0.5f);

            var dataPointMeshFilter = datapoint.GetComponent<MeshFilter>();

            if (vertexCount + dataPointMeshFilter.mesh.vertexCount > 65000 || index == dataPoints.Count - 1)
            {
                var submesh = new GameObject();
                submesh.AddComponent<MeshFilter>();
                submesh.AddComponent<MeshRenderer>();
                submesh.GetComponent<Renderer>().sharedMaterial = _pointRenderer.material;
                submesh.GetComponent<Renderer>().materials[0] = _pointRenderer.material;
                submesh.name = "meshChunk";

                submesh.transform.parent = transform;
                submesh.transform.GetComponent<MeshFilter>().mesh = new Mesh();
                submesh.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine.ToArray());
                combine = new List<CombineInstance>();
                submesh.transform.parent = GetOperator().Visualization.gameObject.transform;

                vertexCount = 0;
            }

            vertexCount += dataPointMeshFilter.sharedMesh.vertexCount;
            var combinInstance = new CombineInstance();
            combinInstance.mesh = dataPointMeshFilter.sharedMesh;
            combinInstance.transform = dataPointMeshFilter.transform.localToWorldMatrix;
            combine.Add(combinInstance);
            
            GameObject.Destroy(datapoint);
        }
        GameObject.Destroy(_pointPrimitive);
    }



}
