/*
   This operator calculates k-means clustering on a given dataset (Vector3[]).
   Generate starting seed centroids, that are randomly within the data.
   Calculate the distance between the centroids and every data point from the input.
   Assign every data point to its nearest centroid.
   Calculate the mean of all assigned data points for each centroid, that's the new centroid.
   Repeat distance calculations and assignments until convergence or a set number of times.
 */


using System;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class KMeansClusteringOperator : GenericOperator, IMenueComponentListener
{
	private List<Vector3> _input = new List<Vector3>(); //data input
	
//	[HideInInspector]
	public int K = 2; //input parameter k, number of centroids/cluster
	
	private List<Vector3> _centroids = new List<Vector3>(); //list of centroids
	private float[,] _distanceMatrix; //matrix that saves the distances between centroids and data points
	private Vector3[,] _clusterMatrix; //matrix that saves centroids and the assigned data points
	private SimpleDatamodel _simpleDataModel; //data structure where the result gets saved in

//	[HideInInspector]
	public bool Convergence; //run until convergence?
//	[HideInInspector]
	public int RunForXLoops = 10; //run for X loops instead of convergence
//	[HideInInspector]
//	public bool Clustering; //to be accessed in the scatterplot class and others in order to check if they're getting clustered data or not

	private int _hasRunFor; //has already looped X times
	private bool _centroidCheck;
	private bool _menuExists;
	
	private MenueScript _menu;

	public Mesh TestMesh;
	public Material TestMaterial;
	public int counter;
	public GameObject TestSphere;
	public GameObject TestCube;
	
	public override void Start()
	{
		base.Start();
				
		CreateMenu();

		TestSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//		TestCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		
		TestMesh = TestSphere.GetComponent<MeshFilter>().mesh;
//		TestCube.GetComponent<MeshFilter>().mesh = TestMesh;
		
		TestMaterial = new Material(Shader.Find("Specular"));
		TestMaterial.color = Color.red;
		TestSphere.GetComponent<Renderer>().material = TestMaterial;
		Destroy(TestSphere);
//		Destroy(TestCube);
		
		
//		NumberOfClusters();
		
		var dataItems = _rawInputData.GetDataItems();
		
		foreach (var dataItem in dataItems)
		{
			_input.Add(dataItem.GetfirstThreeNumericColsAsVector());
		}
		Debug.Log("Bevor ich auf den Button drücke: " + _input.Count);

//		Init(K,_input);
//		SetOutputData(_simpleDataModel);
		
		Debug.Log("kmeanoperator with ID " + Id + " just spawned.");
		
	}

	public override bool Process()
	{	
		return true;
	}

	public override bool ValidateIfOperatorPossibleForParents(GenericOperator parent)
	{
		return true;
	}

	private void Update()
	{
		if (Observer.selectedOperator != this && _menuExists)
		{
			DestroyMenu();
			Debug.Log("test");
		}
		else if (Observer.selectedOperator.Equals(this) && !_menuExists)
		{
			CreateMenu();
		}
		
		//TEST
		//TEST
		//TEST
		//TEST
		//TEST
		//TEST

		
		Graphics.DrawMesh(TestMesh, Vector3.zero, Quaternion.identity, TestMaterial, 0);
			


		//TEST
		//TEST
		//TEST
		//TEST
		//TEST
	}

	//Initialize by creating random seed centroids and starting the method cascade
	public void Init(int k, List<Vector3> input)
	{
		_simpleDataModel = new SimpleDatamodel();
//		Clustering = true;
		
//		NumberOfClusters();
//		NumberOfLoops();

		k = K;

		if (_distanceMatrix != null)
		{
			Array.Clear(_distanceMatrix,0,_distanceMatrix.Length);
			Array.Clear(_clusterMatrix,0,_clusterMatrix.Length);
		}
		
		_distanceMatrix = new float[k+1,input.Count+1];
		_clusterMatrix = new Vector3[k+1,input.Count+1];
		
		//get the range of the input data
		var minVector = Vector3.positiveInfinity; 
		var maxVector = Vector3.zero;
		
		for (var i = 0; i < input.Count; i++)
		{
			minVector = input[i].magnitude < minVector.magnitude ?  input[i] : minVector;
			maxVector = input[i].magnitude > maxVector.magnitude ?  input[i] : maxVector;
		}

		var xMin = minVector.x;
		var yMin = minVector.y;
		var zMin = minVector.z;
		var xMax = maxVector.x;
		var yMax = maxVector.y;
		var zMax = maxVector.z;

		//set the centroids randomly inside the range of the input (minmax)
		for (var i = 0; i < K; i++)
		{
			var randomX = Random.Range(xMin, xMax);
			var randomY = Random.Range(yMin, yMax);
			var randomZ = Random.Range(zMin, zMax);
			
			var tempRandom = new Vector3(randomX,randomY,randomZ);
			_centroids.Add(tempRandom);
		}
		
		CalculateDistances();
	}

	//calculate the distance between all data points and the cluster centroid seeds
	private void CalculateDistances()
	{
		for (var i = 0; i < _input.Count; i++)
		{
			for (var j = 0; j < K; j++)
			{
				var currentDistance = Vector3.Distance(_input[i], _centroids[j]);
				
				//fill distance values into matrix
				_distanceMatrix[j + 1, i + 1] = currentDistance;
			}
		}
		
		AssignDatapoints();
	}

	//Assign datapoints to the nearest centroid
	private void AssignDatapoints()
	{
		for (var i = 1; i <= _input.Count; i++)
		{
			var smallestDistance = float.PositiveInfinity;
			var index = -1;

			for (var j = 1; j <= K; j++)
			{
				if (_distanceMatrix[j, i] < smallestDistance)
				{
					smallestDistance = _distanceMatrix[j, i];
					index = j;
				}
			}
			_clusterMatrix[index, i] = _input[i-1];
		}
		
		NewCentroids();
	}

	//Calculate the new centroids as the mean of the currently nearest data points
	private void NewCentroids()
	{
		var convergenceCounter = 0;
		_centroids.Clear();
		
		for (var i = 1; i <= K; i++)
		{
			var centroidSum = Vector3.zero;
			var tempCounter = 0;
			var mean = Vector3.zero;
			
			for (var j = 1; j <= _input.Count; j++)
			{
		
				if (_clusterMatrix[i, j] != Vector3.zero)
				{
					tempCounter++;
					centroidSum += _clusterMatrix[i, j];	
				}
				
				if (j == _input.Count)
				{
					mean = centroidSum / tempCounter;
					_centroids.Add(mean);
				}	
			}

			if (Convergence)
			{
                Debug.Log("WOOOT"+i+ "|"+ _clusterMatrix[1, 0]);
				convergenceCounter++;
			    Debug.Log("DFUQ");
                //run until convergence is reached
                if (_clusterMatrix[i, 0] == mean)
				{
					
					Debug.Log(convergenceCounter);
					if (convergenceCounter == K)
					{
						_centroidCheck = true;
						EncodeResultToSimpleDataModel();
					}
				}
				else
				{
					break;
				}
			}
			else
			{
				_clusterMatrix[i, 0] = mean;
			
				//run until the counter is done instead of convergence
				if (_hasRunFor >= RunForXLoops)
				{
					_centroidCheck = true;
					EncodeResultToSimpleDataModel();
				}
			}	
		}

		if (!_centroidCheck)
		{
			//Start again with filling the matrices and assigning data points to the nearest (new) centroids
			_hasRunFor++;
			CalculateDistances();	
		}
	}

	//saves the final result matrix as dataAttributes/dataItems -> SimpleDataModel
	private void EncodeResultToSimpleDataModel()
	{
		for (var i = 1; i <= K; i++)
		{
			for (var j = 1; j <= _input.Count; j++)
			{
				if (_clusterMatrix[i, j] != Vector3.zero)
				{
					var dataItem = new DataItem();
				
					var x = new DataAttribute();
					var y = new DataAttribute();
					var z = new DataAttribute();
					var centroid = new DataAttribute();

					var valueX = _clusterMatrix[i, j].x;
					var valueY = _clusterMatrix[i, j].y;
					var valueZ = _clusterMatrix[i, j].z;
					var valueCentroid = (float)i;
				
					x.Init(j, "x", valueX + "", DataAttribute.Valuetype.ValFloat);
					y.Init(j, "y", valueY + "", DataAttribute.Valuetype.ValFloat);
					z.Init(j, "z", valueZ + "", DataAttribute.Valuetype.ValFloat);
					centroid.Init(0, "centroid", valueCentroid + "", DataAttribute.Valuetype.ValFloat);
				
					dataItem.Add(x);
					dataItem.Add(y);
					dataItem.Add(z);
					dataItem.Add(centroid);
										
					_simpleDataModel.Add(dataItem);
				}
			}
		}
	}
	
	//gets the UI string/int field
	private void NumberOfLoops()
	{
		var inputFieldText = GameObject.Find("NumberOfLoops").GetComponent<Text>();
		var test = Observer.selectedOperator.gameObject;

		if (!Convergence)
		{
			try
			{
				RunForXLoops = int.Parse(inputFieldText.text);
				Debug.Log("Ok Master, I will run " + RunForXLoops + " times!");
			}
			catch (Exception e)
			{
				Debug.Log("Please insert an integer into the input field.");
			}
		}
	}
	
	//gets the UI string/int field
	private void NumberOfClusters()
	{
		var inputFieldText = GameObject.Find("NumberOfClusters").GetComponent<Text>();
	
		try
		{
			K = int.Parse(inputFieldText.text);
			Debug.Log("Yo cluster me about " + K + " blops!");
		}
		catch (Exception e)
		{
			Debug.Log("I will use the Slider value for the cluster amount.");
		}
		
	}

	public void Restart()
	{
		Debug.Log("Nachdem ich auf den Button drücke: " + _input.Count);
		_centroidCheck = false;
		_hasRunFor = 0;
		
		Debug.Log("operator id: " + GetId());
		
		Init(K,_input);
		Debug.Log(_simpleDataModel.GetDataItems().Count);
		SetOutputData(_simpleDataModel);
		
	}

	public void CreateMenu()
	{
		_menu = FindObjectOfType<MenueScript>();
		
		if(_menu == null)
		{
			Debug.Log("Did not find a valid menue!");
			return;
		}
		
		_menu.AddDiscreteSlider("K", this);
		_menu.AddToggle("KMeanConvergence", this);	
		_menu.AddInputfield("NumberOfLoops", this);
		_menu.AddInputfield("NumberOfClusters", this);
		_menu.AddButton("KMeanStart", this);

		_menuExists = !_menuExists;
	}

	public void DestroyMenu()
	{
		_menu.Delete();
		_menuExists = !_menuExists;
	}

	public void menuChanged(GenericMenueComponent changedComponent)
	{
		if (_menu == null)
		{
			Debug.Log("Did not find a valid menue!");
			return;
		}

		string name = changedComponent.getName();
		

		//change if the KMean should run until convergence or not
		if (name.Equals("KMeanConvergence"))
		{
			Convergence = _menu.GetToggleValue(changedComponent.getId()); 
		}

		if (name.Equals("K"))
		{
			K = changedComponent.GetComponent<DiscreteSliderScript>().GetToggleSliderNumber();
//			Debug.Log("KKKKKKKK geändert: " + K);			
		}

		if (name.Equals("KMeanStart"))
		{
			Restart();
		}

		if (name.Equals("NumberOfLoops"))
		{
			var textObject = gameObject.transform.GetComponentInChildren<InputField>().text;
			RunForXLoops = int.Parse(textObject);	
			Debug.Log("LOOPY LOOPY LOOP");
		}
		
		if (name.Equals("NumberOfClusters"))
		{
			var textObject = gameObject.transform.GetComponentInChildren<InputField>().text;
			K = int.Parse(textObject);	
			Debug.Log("CLUSTERY MC CLUSTERSON");
		}
	}
}
