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
    private int _numberOfDimensions = 3; //per default get the first 3 dimensions of the dataItem as Vector3 coordinates 

    private void Start()
    {
        var viz = GameObject.Find("Visualization");
        viz.AddComponent<ScatterplotInteractionController>();
        viz.AddComponent<BoxCollider>();
    }

    public void SetNumberOfDimensions(int number)
    {
        _numberOfDimensions = number;
    }
    
    public int GetNumberOfDimensions()
    {
        return _numberOfDimensions;
    }

    public override void CreateVisualization()
    {
        _pointPrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _pointPrimitive.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        _pointRenderer = _pointPrimitive.GetComponent<Renderer>(); 
        _pointRenderer.material = new Material(Shader.Find("Specular"));        
        _pointRenderer.material.color = _pointColor;      
        _colorRamp = GameObject.Find("VisualizationSpace").GetComponent<ColorRamp>().RGBGradient;  // get the rgb gradient image here, it's a field in the ColorRamp component which is attached to the VisualizationSpace GameObject
        

        if (GetOperator().GetRawInputData() == null)
            return;

        var dataPoints = ((SimpleDatamodel)GetOperator().GetRawInputData()).GetVariableCoords(_numberOfDimensions);
//        var dataPointsCluster = ((SimpleDatamodel)GetOperator().GetRawInputData()).GetCoordsAndCluster(); //new method for getting a vector4 instead of a vector3 (vector3 + cluster)

//        //Constructs a dictionary/lookup with the cluster ID value (float) from the previous kmeansclustering (if any) as key and a list of the corresponding data points as the value
//        Dictionary<float, List<DataItem>> dict = new Dictionary<float, List<DataItem>>();
//        float id;
        var counter = 0;
        var clustered = false;

        var attributeValuePairs = GetOperator().GetRawInputData().GetDataItems()[0].GetDataAttributeValuePairs();
        var dataItems = GetOperator().GetRawInputData().GetDataItems();
            
        var body = gameObject.GetComponent<Rigidbody>();
        if (body != null) { body.angularVelocity = Vector3.zero; }
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        var combine = new List<CombineInstance>();
        var vertexCount = 0;

        for (var index = 0; index < dataPoints.Count; index++)
        {
            var datapoint = Instantiate(_pointPrimitive);
            datapoint.transform.parent = GetOperator().Visualization.gameObject.transform;

            var hasMoreThanThreeDimensions = false;

            switch (_numberOfDimensions)
            {
                case 1:
                    datapoint.transform.localPosition = new Vector3(dataPoints[index][0] - 0.5f, -0.5f, -0.5f);                 
                    break;
                case 2:
                    datapoint.transform.localPosition = new Vector3(dataPoints[index][0] - 0.5f, dataPoints[index][1] - 0.5f, -0.5f);
                    break;
                case 3:
                    datapoint.transform.localPosition = new Vector3(dataPoints[index][0] - 0.5f, dataPoints[index][1] - 0.5f, dataPoints[index][2] - 0.5f);
                    break;
                default:
                    datapoint.transform.localPosition = new Vector3(dataPoints[index][0] - 0.5f, dataPoints[index][1] - 0.5f, dataPoints[index][2] - 0.5f);
                    hasMoreThanThreeDimensions = true;
                    break;
            }

            if (hasMoreThanThreeDimensions)
            {
                switch (_numberOfDimensions)
                {
                    case 4:
                                         
                        break;                   
                }
            }
            
            
            
            
            
            
            // get how many clusters there are, calculate evenly spaced positions in a one dimensional axis get the color value from the color ramp (rgb gradient)
            var numCluster = dataPoints.Count;
            var colorPoint = (545 / numCluster) / 2;           
            var clusterColor = _colorRamp.GetPixel(index * (545 / numCluster) + colorPoint, 250);

            _pointColorCluster = clusterColor;           
            _pointPrimitiveCluster = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _pointRendererCluster = _pointPrimitiveCluster.GetComponent<Renderer>();
            _pointPrimitiveCluster.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            _pointRendererCluster.material = new Material(Shader.Find("Specular"));
            _pointRendererCluster.material.color = _pointColorCluster;        

            
            
            
            
            
            
            
            
//            datapoint.transform.localPosition = new Vector3(dataPoints[index].x - 0.5f, dataPoints[index].y - 0.5f, dataPoints[index].z - 0.5f);  
    

            var dataPointMeshFilter = datapoint.GetComponent<MeshFilter>();

            if (vertexCount + dataPointMeshFilter.mesh.vertexCount > 65000 || index == dataPoints.Count - 1)
            {              
                var submesh = new GameObject();
                submesh.AddComponent<MeshFilter>();
                submesh.AddComponent<MeshRenderer>();
//                submesh.GetComponent<Renderer>().sharedMaterial = _pointRenderer.material;
                submesh.GetComponent<Renderer>().sharedMaterial = _pointRendererCluster.material;
//                submesh.GetComponent<Renderer>().materials[0] = _pointRenderer.material;
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
    
            GameObject.Destroy(datapoint);
        }
        
        GameObject.Destroy(_pointPrimitive); 
          
        
        
        
        
        
        
        /*
        foreach (DataItem dataItem in dataItems)
        { 
            counter++;
            Debug.Log("bis hier hin");
            try
            {
                id = (float) dataItem.GetDataAttributeValuePairs()[3].GetValue();
                if (!dict.ContainsKey(id)) dict.Add(id, new List<DataItem>());
                dict[id].Add(dataItem);
            }
            catch (Exception e)
            {
                Debug.Log("jo");
            }
           
            Debug.Log("aber nicht bis hier");
            if (counter == GetOperator().GetRawInputData().GetDataItems().Count)
            {
                CreateClusters(dict);
            }            
        }
        */

        
       


       
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
