﻿using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Rotates a gameObject around the tree-graph
 * Calculates the number of node overlaps, edge crossings and angles between them
 * and saves it in QualityMetricViewport class for later calculation
 */

public class ViewPortOptimizer : MonoBehaviour {
    public Vector3 position;
    public bool globalScan, scan, switchView, switchBackView;
    public Transform cam;
    public int nrOfViews;
    public List<GeneralLayoutAlgorithm> algorithmsList;

    public List<List<QualityMetricViewPort>> globalObservationList;
    public List<QualityMetricViewPort> observationList;
    public QualityMetricViewPort temp;
    public QualityMetricSlider edge, node, crossAngle, angRes, edgeLength;

    private Transform[] opIcons;
    private LayoutAlgorithm current;
    private Observer observer;
    // Lists of all metrics for calculating Mean and STD values
    private List<float> _edgeLengths, _angRes, _edgeCrossAngle;
    private List<float> _nodeOverlaps, _edgeCrossings;
    /* Mean and Standard Deviation values of 
     * quality metrics for calculating z score
     */
    /*private float _nodeOverlapMean, _nodeOverlapSTD;
    private float _edgeCrossMean, _edgeCrossSTD;
    private float _edgeCrossAngleMean, _edgeCrossAngleSTD;
    private float _angResMean, _angResSTD;
    private float _edgeLengthMean, _edgeLengthSTD;*/


    private float minEdgeCross, maxEdgeCross, minNodeOverlap, maxNodeOverlap;
    // Z scores of quality metrics
    //private float _ZnodeOverlap, _ZedgeCross, _ZedgeCrossAngle, _ZangRes, _ZedgeLength;

    // counter for switching views
    private int counter = -1;

    private bool _toBreak;
	// Use this for initialization
	void Start () {
        observer = (Observer)FindObjectOfType(typeof(Observer));
        globalObservationList = new List<List<QualityMetricViewPort>>();
        observationList = new List<QualityMetricViewPort>();
        current = GetComponent<LayoutAlgorithm>();
        _edgeLengths = new List<float>();
        _angRes = new List<float>();
        _edgeCrossAngle = new List<float>();
        _nodeOverlaps = new List<float>();
        _edgeCrossings = new List<float>();
	}
	
	// Update is called once per frame
	void Update () {
		if(scan)
        {
            scan = false;
            //set gameobject in the center of the graph
            transform.position = FindCenterOfGraph();
            //set camera to position of the furthest node + 1 meter
            transform.GetChild(0).localPosition = FindFurthestNode();
            cam.parent = transform;
            cam.localPosition = FindFurthestNode();
            cam.LookAt(transform);
            observationList = new List<QualityMetricViewPort>();
            ScanAndCalculate();
            cam.parent = null;
            cam.position = observationList[0].cameraPosition;
            cam.LookAt(transform);
            globalObservationList.Add(observationList);
        }
        /*if(globalScan)
        {
            globalScan = false;
            while(!current.currentLayout.AlgorithmHasFinished())
            {
                algorithmsList[0].StartAlgorithm();
            }
        }*/
        if(switchView)
        {
            switchView = false;
            counter++;
            if (counter == observationList.Count)
            {
                counter = observationList.Count - 1;
            }
            cam.position = observationList[counter].cameraPosition;
            cam.LookAt(transform);
            Debug.Log(observationList[counter].overallGrade + " node overlap " + observationList[counter].nrNodeOverlaps + " edge cross " + observationList[counter].nrEdgeCrossings + " edge cross angle " + observationList[counter].edgeCrossAngle + " angular res " + observationList[counter].angResRM + " edge length " + observationList[counter].edgeLength);
        }
        if(switchBackView)
        {
            switchBackView = false;
            counter--;
            if(counter < 0)
            {
                counter = 0;
            }
            cam.position = observationList[counter].cameraPosition;
            cam.LookAt(transform);
        }
	}

    /* Finds the center of the graph for rotation of the scanner object
     *  around the tree-graph
     */
    Vector3 FindCenterOfGraph()
    {
        Vector3 pos = new Vector3();
        foreach(var op in observer.GetOperators())
        {
            pos += op.GetIcon().transform.position;
        }
        pos /= observer.GetOperators().Count;
        return pos;
    }

    /* Finds the position of the furthest node and adds +1 meter 
     * in that direction for seeing the whole graph
     */
    Vector3 FindFurthestNode()
    {
        Vector3 pos = observer.GetOperators()[0].GetIcon().transform.position;
        foreach(var op in observer.GetOperators())
        {
            if (Vector3.Distance(transform.position, pos) < Vector3.Distance(transform.position, op.GetIcon().transform.position)) pos = op.GetIcon().transform.position;
        }
        pos = new Vector3(Vector3.Distance(transform.position, pos) + 2, 0, 0);
        return pos;
    }

    // Repositiones the camera around the graph and calculates the quality metrics
    void ScanAndCalculate()
    {
        opIcons = new Transform[observer.GetOperators().Count];
        for(int i=0; i<opIcons.Length; i++)
        {
            if (observer.GetOperators()[i].GetIcon().transform.GetChild(0).name != "Plane") opIcons[i] = observer.GetOperators()[i].GetIcon().transform.GetChild(0);
            else opIcons[i] = observer.GetOperators()[i].GetIcon().transform.GetChild(1);
        }
        for(int i = 0; i<180; i+=10)
        {
            for(int j=0; j<360; j+=10)
            {
                _toBreak = false;
                transform.eulerAngles = new Vector3(i, 0, 0);
                transform.Rotate(Vector3.up, j);
                cam.transform.LookAt(transform);
                // first check if current position already exists
                foreach (var view in observationList)
                {
                    if (view.cameraPosition == cam.position)
                    {
                        _toBreak = true;
                        break;
                    }
                }
                if (_toBreak) continue;
                temp = new QualityMetricViewPort();

                //rotate icons towards camera before calculating node overlapping
                foreach(var icon in opIcons)
                {
                    icon.LookAt(cam);
                }
                temp.nrNodeOverlaps = GetComponent<NodeOverlapping>().CalculateNodeOverlapping();
                GetComponent<TwoDimensionalProjection>().ProjectTree();
                temp.nrEdgeCrossings = GetComponent<EdgeCrossingCounter>().CountEdgeCrossings();
                temp.edgeCrossAngle = GetComponent<EdgeCrossingCounter>().edgeCrossRM;
                temp.angResRM = GetComponent<NodeAngularResolution>().CalculateAngularResolution();
                temp.edgeLength = GetComponent<EdgeLength>().CalculateEdgeLength();
                GetComponent<TwoDimensionalProjection>().RestorePositions();
                temp.cameraPosition = cam.position;
                temp.algorithm = current.currentLayout;
                observationList.Add(temp);

                _nodeOverlaps.Add(temp.nrNodeOverlaps);
                _edgeCrossings.Add(temp.nrEdgeCrossings);
                _edgeCrossAngle.Add(temp.edgeCrossAngle);
                _angRes.Add(temp.angResRM);
                _edgeLengths.Add(temp.edgeLength);
            }
        }
        minNodeOverlap = MinimumNodeOverlap(observationList);
        maxNodeOverlap = MaximumNodeOverlap(observationList);
        minEdgeCross = MinimumEdgeCross(observationList);
        maxEdgeCross = MaximumEdgeCross(observationList);
        foreach(var currentView in observationList)
        {
            currentView.overallGrade = ((crossAngle.qualityFactor * currentView.edgeCrossAngle) + (angRes.qualityFactor * currentView.angResRM) + (node.qualityFactor * NormalizedNodeOverlaps(currentView.nrNodeOverlaps)) + (edge.qualityFactor * NormalizedEdgeCrossings(currentView.nrEdgeCrossings)) + (edgeLength.qualityFactor * currentView.edgeLength)) / (crossAngle.qualityFactor + edge.qualityFactor + node.qualityFactor + angRes.qualityFactor + edgeLength.qualityFactor);
        }

        observationList = SortList(observationList);
        nrOfViews = observationList.Count;
    }

    // Function for calculating Mean of values in list
    float MeanCalc(List<float> list)
    {
        float mean = 0;
        for (int i = 0; i < list.Count; i++)
        {
            mean += list[i];
        }
        mean /= list.Count;
        return mean;
    }

    // Function for calculating Standard Deviation of values in list
    float STDCalc(List<float> list, float mean)
    {
        float sum = 0;
        for (int i = 0; i < list.Count; i++)
        {
            sum += Mathf.Pow(list[i] - mean, 2);
        }
        sum /= list.Count;
        return Mathf.Sqrt(sum);
    }

    private List<QualityMetricViewPort> SortList(List<QualityMetricViewPort> list)
    {
        list.Sort((b, a) => a.overallGrade.CompareTo(b.overallGrade));
        return list;
    }

    private float MaximumEdgeLength(List<QualityMetricViewPort> list)
    {
        float max = 0;
        foreach(var item in list)
        {
            if (max < item.edgeLength) max = item.edgeLength;
        }
        return max;
    }
    private float MaximumEdgeCross(List<QualityMetricViewPort> list)
    {
        float max = 0;
        foreach (var item in list)
        {
            if (max < item.nrEdgeCrossings) max = item.nrEdgeCrossings;
        }
        return max;
    }
    private float MinimumEdgeCross(List<QualityMetricViewPort> list)
    {
        float min = 0;
        foreach (var item in list)
        {
            if (min > item.nrEdgeCrossings) min = item.nrEdgeCrossings;
        }
        return min;
    }
    private float MaximumNodeOverlap(List<QualityMetricViewPort> list)
    {
        float max = 0;
        foreach (var item in list)
        {
            if (max < item.nrNodeOverlaps) max = item.nrNodeOverlaps;
        }
        return max;
    }
    private float MinimumNodeOverlap(List<QualityMetricViewPort> list)
    {
        float min = 0;
        foreach (var item in list)
        {
            if (min > item.nrNodeOverlaps) min = item.nrNodeOverlaps;
        }
        return min;
    }
    private float MaximumEdgeAngle(List<QualityMetricViewPort> list)
    {
        float max = 0;
        foreach (var item in list)
        {
            if (max < item.edgeCrossAngle) max = item.edgeCrossAngle;
        }
        return max;
    }
    private float MaximumAngRes(List<QualityMetricViewPort> list)
    {
        float max = 0;
        foreach (var item in list)
        {
            if (max < item.angResRM) max = item.angResRM;
        }
        return max;
    }

    private float NormalizedEdgeCrossings(float edgeCross)
    {
        float cubeRoot = 1f / 3f;
        return 1 - ((Mathf.Pow(edgeCross, cubeRoot) - Mathf.Pow(minEdgeCross, cubeRoot)) / (Mathf.Pow(maxEdgeCross, cubeRoot) - Mathf.Pow(minEdgeCross, cubeRoot)));
    }
    private float NormalizedNodeOverlaps(float nodeOverlaps)
    {
        float cubeRoot = 1f / 3f;
        float result = ((Mathf.Pow(nodeOverlaps, cubeRoot) - Mathf.Pow(minNodeOverlap, cubeRoot)) / (Mathf.Pow(maxNodeOverlap, cubeRoot) - Mathf.Pow(minNodeOverlap, cubeRoot)));
        return 1 - result;
    }
}
