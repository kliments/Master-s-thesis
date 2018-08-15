﻿/*
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

public class KMeansClusteringOperator : GenericOperator
{
	private List<Vector3> _input = new List<Vector3>(); //data input
	
	public static int _k = 2; //input parameter k, number of centroids/cluster
	
	private List<Vector3> _centroids = new List<Vector3>(); //list of centroids
	private float[,] _distanceMatrix; //matrix that saves the distances between centroids and data points
	private Vector3[,] _clusterMatrix; //matrix that saves centroids and the assigned data points
	private SimpleDatamodel _simpleDataModel = new SimpleDatamodel(); //data structure where the result gets saved in

	public static bool Convergence; //run until convergence?
	public static int RunForXLoops = 1; //run for X loops instead of convergence
	public static bool Clustering; //to be accessed in the scatterplot class and others in order to check if they're getting clustered data or not

	private int _hasRunFor; //has already looped X times
	
	public override void Start()
	{
		base.Start();	
		

		var dataItems = _rawInputData.GetDataItems();
		
		foreach (var dataItem in dataItems)
		{
			_input.Add(dataItem.GetfirstThreeNumericColsAsVector());
		}


		Init(_k,_input);
		SetOutputData(_simpleDataModel);
	}

	public override bool Process()
	{	
		return true;
	}

	public override bool ValidateIfOperatorPossibleForParents(GenericOperator parent)
	{
		return true;
	}
	
	//Initialize by creating random seed centroids and starting the method cascade
	public void Init(int k, List<Vector3> input)
	{
		Clustering = true;
		NumberOfLoops();
		
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
		for (var i = 0; i < _k; i++)
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
			for (var j = 0; j < _k; j++)
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

			for (var j = 1; j <= _k; j++)
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
		var centroidCheck = false;
		var convergenceCounter = 0;
		_centroids.Clear();
		
		for (var i = 1; i <= _k; i++)
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
					if (convergenceCounter == _k)
					{
						centroidCheck = true;
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
					centroidCheck = true;
					EncodeResultToSimpleDataModel();
				}
			}	
		}

		if (!centroidCheck)
		{
			//Start again with filling the matrices and assigning data points to the nearest (new) centroids
			_hasRunFor++;
			CalculateDistances();	
		}
	}

	//saves the final result matrix as dataAttributes/dataItems -> SimpleDataModel
	private void EncodeResultToSimpleDataModel()
	{
		for (var i = 1; i <= _k; i++)
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

	public void Restart()
	{
		Init(_k,_input);
	}

}
