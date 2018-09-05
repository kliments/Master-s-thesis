using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Assets.Scripts.Model;
using Model.Operators;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScatterplotVisualization : GenericVisualization {
    private Renderer _pointRenderer;
    private Renderer _pointRendererCluster;
//    private Material _pointMaterial; //commented because never used
    private GameObject _pointPrimitive;
    private GameObject _pointPrimitiveCluster;
    private Color _pointColor = Color.red;
    private Color _pointColorCluster;
    private Texture2D _colorRamp;
    private KMeansClusteringOperator _kmeansClusteringOperator;

    private void Start()
    {
        var viz = GameObject.Find("Visualization");
        viz.AddComponent<ScatterplotInteractionController>();
        viz.AddComponent<BoxCollider>();
    }

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

        // Constructs a dictionary/lookup with the cluster ID value (float) from the previous kmeansclustering (if any) as key and a list of the corresponding data points as the value
        Dictionary<float, List<DataItem>> dict = new Dictionary<float, List<DataItem>>();
        float id;
        var counter = 0;
        var clustered = false;

        // TRY to get the clustering operator and clustering.
        try
        {
            _kmeansClusteringOperator = Observer.selectedOperator.GetComponent<KMeansClusteringOperator>();
            Debug.Log("GGGAAAAAAAAAAHHHHH " + Observer.selectedOperator.name);
            
            if (_kmeansClusteringOperator.Clustering)
            {
                Debug.Log("dann hier");
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
        }
        catch (Exception e)
        {
            Debug.Log("no clustering has been done.");
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
        Vector3 choords;
        // get the rgb gradient image here, it's a field in the ColorRamp component which is attached to the VisualizationSpace GameObject
        _colorRamp = GameObject.Find("VisualizationSpace").GetComponent<ColorRamp>().RGBGradient;
        
        for (var i = 1f; i <= dictionary.Count; i++)
        {
            var dataPoints = new List<Vector3>();
            var combine = new List<CombineInstance>();
            var vertexCount = 0;

            // get how many clusters there are, calculate evenly spaced positions in a one dimensional axis get the color value from the color ramp (rgb gradient)
            var numCluster = dictionary.Count;
            var colorPoint = (545 / numCluster) / 2;           
            var clusterColor = _colorRamp.GetPixel((int)((i - 1) * (545 / numCluster) + colorPoint), 250);

            _pointColorCluster = clusterColor;           
            _pointPrimitiveCluster = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _pointRendererCluster = _pointPrimitiveCluster.GetComponent<Renderer>();
            _pointPrimitiveCluster.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            _pointRendererCluster.material = new Material(Shader.Find("Specular"));
            _pointRendererCluster.material.color = _pointColorCluster;        
         
            // construct a new list of Vector3s for the combine routine so that each cluster is combined to one submesh ideally
            for (var j = 0; j < dictionary[i].Count; j++)
            {
                choords = dictionary[i][j].GetfirstThreeNumericColsAsVector();
                dataPoints.Add(choords);
            }
            
            for (var index = 0; index < dataPoints.Count; index++)
            {
                var datapoint = Instantiate(_pointPrimitiveCluster);
                datapoint.transform.parent = GetOperator().Visualization.gameObject.transform;
                datapoint.transform.localPosition = new Vector3(dataPoints[index].x - 0.5f, dataPoints[index].y - 0.5f, dataPoints[index].z - 0.5f);  
    

                var dataPointMeshFilter = datapoint.GetComponent<MeshFilter>();

                if (vertexCount + dataPointMeshFilter.mesh.vertexCount > 65000 || index == dataPoints.Count - 1)
                {              
                    var submesh = new GameObject();
                    submesh.AddComponent<MeshFilter>();
                    submesh.AddComponent<MeshRenderer>();
                    submesh.GetComponent<Renderer>().sharedMaterial = _pointRendererCluster.material;
                    submesh.GetComponent<Renderer>().materials[0] = _pointRendererCluster.material;
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
    
                Destroy(datapoint);
            }
            
            Destroy(_pointPrimitiveCluster);   
        } 
    }
}
