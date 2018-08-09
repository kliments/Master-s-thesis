using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterplotVisualization : GenericVisualization {
    private Renderer _pointRenderer;
    
    private Renderer _pointRenderer1;
    private Renderer _pointRenderer2;
    private Renderer _pointRenderer3;
    private Renderer _pointRenderer4;
    private Renderer _pointRenderer5;
    
//    private Material _pointMaterial; //commented because never used
    private GameObject _pointPrimitive;
    private Color _pointColor = Color.red;
    
    private Color _clusterColor1 = Color.blue;
    private Color _clusterColor2 = Color.cyan;
    private Color _clusterColor3 = Color.green;
    private Color _clusterColor4 = Color.magenta;
    private Color _clusterColor5 = Color.yellow;
    
//    private List<GameObject> _pointsList = new List<GameObject>();
    
    public override void CreateVisualization()
    {
        _pointPrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _pointPrimitive.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        _pointRenderer = _pointPrimitive.GetComponent<Renderer>();
        
        _pointRenderer1 = _pointPrimitive.GetComponent<Renderer>();
        _pointRenderer2 = _pointPrimitive.GetComponent<Renderer>();
        _pointRenderer3 = _pointPrimitive.GetComponent<Renderer>();
        _pointRenderer4 = _pointPrimitive.GetComponent<Renderer>();
        _pointRenderer5 = _pointPrimitive.GetComponent<Renderer>();
        
        _pointRenderer.material = new Material(Shader.Find("Specular"));
        
        _pointRenderer1.material = new Material(Shader.Find("Specular"));
        _pointRenderer2.material = new Material(Shader.Find("Specular"));
        _pointRenderer3.material = new Material(Shader.Find("Specular"));
        _pointRenderer4.material = new Material(Shader.Find("Specular"));
        _pointRenderer5.material = new Material(Shader.Find("Specular"));
        
        _pointRenderer.material.color = _pointColor;
        
        _pointRenderer1.material.color = _clusterColor1;
        _pointRenderer2.material.color = _clusterColor2;
        _pointRenderer3.material.color = _clusterColor3;
        _pointRenderer4.material.color = _clusterColor4;
        _pointRenderer5.material.color = _clusterColor5;

        if (GetOperator().GetRawInputData() == null)
            return;

        var dataPoints = ((SimpleDatamodel)GetOperator().GetRawInputData()).GetCoords();
        var dataPointsCluster = ((SimpleDatamodel)GetOperator().GetRawInputData()).GetCoordsAndCluster(); //new method for getting a vector4 instead of a vector3 (vector3 + cluster)

        var clusterList1 = new List<Vector4>();
        var clusterList2 = new List<Vector4>();
        var clusterList3 = new List<Vector4>();
        var clusterList4 = new List<Vector4>();
        var clusterList5 = new List<Vector4>();

        foreach (var dataPoint in dataPointsCluster)
        {
            var cluster = dataPoint.w;
            
            if (cluster == 1f) clusterList1.Add(dataPoint);
            if (cluster == 2f) clusterList2.Add(dataPoint);
            if (cluster == 3f) clusterList3.Add(dataPoint);
            if (cluster == 4f) clusterList4.Add(dataPoint);
            if (cluster == 5f) clusterList5.Add(dataPoint);
        } 

        var body = this.gameObject.GetComponent<Rigidbody>();
        if (body != null) { body.angularVelocity = Vector3.zero; }
        this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        var combine = new List<CombineInstance>();

        var vertexCount = 0;

        //this is the "standard" scatterplot code wrapped in an if-condition: only use, if there are no clusters in the data input
        if (clusterList1.Count < 1)
        {
            Debug.Log("keine cluster gefunden");
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
        else
        {
            Debug.Log("cluster gefunden");
            //use this if there are clusters detected
            for (var index = 0; index < clusterList1.Count; index++)
            {
                var datapoint = Instantiate(_pointPrimitive);
                datapoint.transform.parent = GetOperator().Visualization.gameObject.transform;
                datapoint.transform.localPosition = new Vector3(dataPoints[index].x - 0.5f, dataPoints[index].y - 0.5f, dataPoints[index].z - 0.5f);
            

                var dataPointMeshFilter = datapoint.GetComponent<MeshFilter>();

                if (vertexCount + dataPointMeshFilter.mesh.vertexCount > 65000 || index == clusterList1.Count - 1)
                {              
                    var submesh = new GameObject();
                    submesh.AddComponent<MeshFilter>();
                    submesh.AddComponent<MeshRenderer>();
                    submesh.GetComponent<Renderer>().sharedMaterial = _pointRenderer1.material;
                    submesh.GetComponent<Renderer>().materials[0] = _pointRenderer1.material;
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



}
