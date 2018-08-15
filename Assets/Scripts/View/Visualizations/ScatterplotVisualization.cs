using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScatterplotVisualization : GenericVisualization {
    private Renderer _pointRenderer;
    private Renderer _pointRenderer1;

    
//    private Material _pointMaterial; //commented because never used
    private GameObject _pointPrimitive;
    private GameObject _pointPrimitive1;
    private Color _pointColor = Color.red;
    private Color _pointColor1 = Color.red;
    
  
    
//    private List<GameObject> _pointsList = new List<GameObject>();
    
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
//        var dataPointsCluster = ((SimpleDatamodel)GetOperator().GetRawInputData()).GetCoordsAndCluster(); //new method for getting a vector4 instead of a vector3 (vector3 + cluster)

    

   


        Dictionary<float, List<DataItem>> dict = new Dictionary<float, List<DataItem>>();
        float id;
        var counter = 0;
        var clustered = false;

        if (KMeansClusteringOperator.Clustering)
        {
            foreach (DataItem datitem in ((SimpleDatamodel)GetOperator().GetRawInputData()).GetDataItems())
            {
                counter++;        
                id = (float) datitem.GetDataAttributeValuePairs()[3].GetValue();
                if(!dict.ContainsKey(id)) dict.Add(id,new List<DataItem>());
                dict[id].Add(datitem);

                if (id > 0 && counter == GetOperator().GetRawInputData().GetDataItems().Count)
                {
                    clustered = true;
                    CreateClusters(dict);
                }            
            }
        }

        if (!clustered)
        {
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

    private void CreateClusters(Dictionary<float, List<DataItem>> dictionary)
    {
        var choords = Vector3.zero;
        
        
        
   
        
        
        
        
        
        for (var i = 1f; i < dictionary.Count+1; i++)
        {
            var dataPoints = new List<Vector3>();
            var combine = new List<CombineInstance>();
            var vertexCount = 0;
            _pointColor1 = new Color(Random.Range(0,1), Random.Range(0,1), Random.Range(0,1));            
            _pointPrimitive1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _pointRenderer1 = _pointPrimitive1.GetComponent<Renderer>();

            
 
            _pointPrimitive1.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            _pointRenderer1.material = new Material(Shader.Find("Specular"));
            _pointRenderer1.material.color = _pointColor1;
        
         
            for (var j = 0; j < dictionary[i].Count; j++)
            {
                choords = dictionary[i][j].GetfirstThreeNumericColsAsVector();
                dataPoints.Add(choords);
            }
            
            for (var index = 0; index < dataPoints.Count; index++)
            {
                var datapoint = Instantiate(_pointPrimitive1);
                datapoint.transform.parent = GetOperator().Visualization.gameObject.transform;
                datapoint.transform.localPosition = new Vector3(dataPoints[index].x - 0.5f, dataPoints[index].y - 0.5f, dataPoints[index].z - 0.5f);  
    

                var dataPointMeshFilter = datapoint.GetComponent<MeshFilter>();

                if (vertexCount + dataPointMeshFilter.mesh.vertexCount > 65000 || index == dataPoints.Count - 1)
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
            GameObject.Destroy(_pointPrimitive1);   
        } 
    }
}
