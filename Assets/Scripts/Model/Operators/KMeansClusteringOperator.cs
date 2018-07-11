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

public class KMeansClusteringOperator : GenericOperator
{
	private List<Vector3> _centroids = new List<Vector3>();
	private float[,] _distanceMatrix;
	private Vector3[,] _clusterMatrix;
	
//	private Vector3[] SampleInput = new Vector3[10];
	private List<Vector3> SampleInput = new List<Vector3>();
	
//	private float _arrayMin;
//	private float _arrayMax;
	private Vector3 _maxVector;
	
	private int _testCounter = 1;
	public static int _k; //input parameter k, number of centroids/cluster
	
	
	// Use this for initialization
	void Start ()
	{
//		for (var i = 0; i < 9; i++)
//		{
//			SampleInput[i] = new Vector3(Random.Range(0f, 10f),Random.Range(0f, 10f), Random.Range(0f,10f));
//		}
//		var dataModel = new SimpleDatamodel();
//		SampleInput = dataModel.GetCoords();
//
//		Init(2,SampleInput);
	}

	public override bool Process()
	{
//		var dataModel = new SimpleDatamodel();
		Debug.Log(_rawInputData.GetDataItems()[1]);
		
//		var input = ((SimpleDatamodel)GetOpera)
		//initializes k-means-clustering with _k = number of clusters and input = List<Vector3>
//		Init(_k,input);
		
		//dataModel.addFloatMatrixColwise(Hier meine Vektoren??);
//		SetOutputData(dataModel);
		
		return true;
	}

	public override bool ValidateIfOperatorPossibleForParents(GenericOperator parent)
	{
		return true;
	}

	
	
	
	// Update is called once per frame
	void Update () {
		
//		if (Input.GetKeyDown(KeyCode.Space))
//		{
////			Debug.Log("start");
//			Init(2,SampleInput);
//		}
		
	}
	
	
	
	//Initialize by creating random seed centroids and starting the method cascade
	private void Init(int k, List<Vector3> input)
	{
		_k = k;
		_distanceMatrix = new float[k+1,input.Count+1];
		_clusterMatrix = new Vector3[k+1,input.Count+1];
		
		//get minmax of the input data
//		_arrayMin = Mathf.Min(input);
//		_arrayMax = Mathf.Max(input);
		
		
		//get the range of the input data
		Vector3 minVector = Vector3.positiveInfinity; 
		Vector3 maxVector = Vector3.zero;
		
		for (var i = 0; i < input.Count; i++)
		{
			minVector = input[i].magnitude < minVector.magnitude ?  input[i] : minVector;
			maxVector = input[i].magnitude > maxVector.magnitude ?  input[i] : maxVector;
		}

		_maxVector = maxVector;

		var xMin = minVector.x;
		var yMin = minVector.y;
		var zMin = minVector.z;
		var xMax = maxVector.x;
		var yMax = maxVector.y;
		var zMax = maxVector.z;

		//set the centroids randomly inside the range of the input (minmax)
		for (var i = 0; i < _k; i++)
		{
			var randomX = Random.Range(xMin, xMax);
			var randomY = Random.Range(yMin, yMax);
			var randomZ = Random.Range(zMin, zMax);
			
			var tempRandom = new Vector3(randomX,randomY,randomZ);
			_centroids.Add(tempRandom);
//			Debug.Log("random seeds: " + tempRandom);
		}
		
		FillMatrix(k);
	}

	//Fill the distanceMatrix with the input data, centroids and distance calculations
	private void FillMatrix(int countCentroids)
	{
		//put the data points into the matrix' first row, leave out the very first cell [1,1]
//		for (var i = 0; i < SampleInput.Length; i++)
//		{
//			_distanceMatrix[0, i+1] = SampleInput[i];
//		}
//		
//		//put the centroids into the matrix' first column, leave out the very first cell [1,1]
//		for (var i = 0; i < _k; i++)
//		{
//			_distanceMatrix[i+1, 0] = _centroids[i];
////			Debug.Log(_distanceMatrix[i+1, 1]);
//		}
		
//		Debug.Log("centroids in matrix: " + _distanceMatrix[1,0] + " " + _distanceMatrix[2,0] + " " + _distanceMatrix[3,0]);
	
		//calculate the distance between all data points and the cluster centroid seeds
		for (var i = 0; i < SampleInput.Count; i++)
		{
			for (var j = 0; j < _k; j++)
			{
//				var difference = SampleInput[i] - _centroids[j];
//				currentDistance = (Vector3)Math.Sqrt(Math.Pow(difference, 2));
				var currentDistance = Vector3.Distance(SampleInput[i], _centroids[j]);
				
				//fill distance values into matrix
				_distanceMatrix[j + 1, i + 1] = currentDistance;
//				Debug.Log(currentDistance);
			}
		}
		
		AssignDatapoints();
	}

	//Sort datapoints according to the nearest centroid
	private void AssignDatapoints()
	{
		for (var i = 1; i <= SampleInput.Count; i++)
		{
			var smallestDistance = float.PositiveInfinity;
			var index = -1;

			for (var j = 1; j <= _k; j++)
			{
				if (_distanceMatrix[j, i] < smallestDistance)
				{
					smallestDistance = _distanceMatrix[j, i];
					index = j;
				}
			}
			_clusterMatrix[index, i] = SampleInput[i-1];
//			Debug.Log(_clusterMatrix[1, i] + " " + _clusterMatrix[2, i] + " " + _clusterMatrix[3, i]);
		}
		
		NewCentroids();
	}

	//Calculate the new centroids as the mean of currently nearest data points
	private void NewCentroids()
	{
		var centroidCheck = false;
		_centroids.Clear();
		
		for (var i = 1; i <= _k; i++)
		{
			var centroidSum = Vector3.zero;
			var tempCounter = 0;
			var mean = Vector3.zero;
			
			for (var j = 1; j <= SampleInput.Count; j++)
			{
		
				if (_clusterMatrix[i, j] != Vector3.zero)
				{
					tempCounter++;
					centroidSum += _clusterMatrix[i, j];	
				}
				
				if (j == SampleInput.Count)
				{
					mean = centroidSum / tempCounter;
					_centroids.Add(mean);
//					Debug.Log(mean);
				}
				
			}

//			Debug.Log("Inhalt Clustermatrix: " + _clusterMatrix[i, 0] + " || " + _clusterMatrix[i, 1] + " " +  _clusterMatrix[i, 2] + " " + _clusterMatrix[i, 3] + " " + _clusterMatrix[i, 4] + " " + _clusterMatrix[i, 5] + " " + _clusterMatrix[i, 6] + " " + _clusterMatrix[i, 7] + " " + _clusterMatrix[i, 8] + " " + _clusterMatrix[i, 9] + " " + _clusterMatrix[i, 10]);
            			
			//run until the counter is done instead of convergence
			if (_testCounter > 9)
			{
				centroidCheck = true;
			}
			
			//run until convergence is reached
//			if (_clusterMatrix[i, 0] == centroidSum)
//			{
//				centroidCheck = true;
//			}
			
			
			_clusterMatrix[i, 0] = mean;
//			_distanceMatrix[i, 0] = mean;
//			Debug.Log(mean);
			
		}
		
		Debug.Log("runs: " + _testCounter + "  and centroids: " +_clusterMatrix[1,0] + " " + _clusterMatrix[2,0]);

		if (!centroidCheck)
		{
			//Start again with filling the matrices and assigning data points to the nearest (new) centroids
			_testCounter++;
			FillMatrix(_k);
		}
	}	
}
