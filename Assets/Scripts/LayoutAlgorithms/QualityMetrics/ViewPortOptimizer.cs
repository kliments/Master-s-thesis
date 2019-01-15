using Assets.Scripts.Model;
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
    public bool scan, switchView, switchBackView;
    public Transform cam;
    public int nrOfViews;

    private Observer observer;
    public List<QualityMetricViewPort> observationList;
    public QualityMetricViewPort temp;

    // Lists of all metrics for calculating Mean and STD values
    private List<float> _edgeLengths, _angRes, _minAngles;
    private List<float> _nodeOverlaps, _edgeCrossings;
    /* Mean and Standard Deviation values of 
     * quality metrics for calculating z score
     */
    private float _nodeOverlapMean, _nodeOverlapSTD;
    private float _edgeCrossMean, _edgeCrossSTD;
    private float _minAngleMean, _minAngleSTD;
    private float _angResMean, _angResSTD;
    private float _edgeLengthMean, _edgeLengthSTD;

    // Z scores of quality metrics
    private float _ZnodeOverlap, _ZedgeCross, _ZminAngle, _ZangRes, _ZedgeLength;

    // counter for switching views
    private int counter = -1;

    private bool _toBreak;
	// Use this for initialization
	void Start () {
        observer = (Observer)FindObjectOfType(typeof(Observer));
        observationList = new List<QualityMetricViewPort>();

        _edgeLengths = new List<float>();
        _angRes = new List<float>();
        _minAngles = new List<float>();
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
        }
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
            Debug.Log(observationList[counter].overallGrade);
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
        for(int i = 0; i<180; i+=5)
        {
            for(int j=0; j<360; j+=5)
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
                NodeOverlapping node = new NodeOverlapping();
                temp = new QualityMetricViewPort();
                temp.nrNodeOverlaps = GetComponent<NodeOverlapping>().CalculateNodeOverlapping();
                GetComponent<TwoDimensionalProjection>().ProjectTree();
                temp.nrEdgeCrossings = GetComponent<EdgeCrossingCounter>().CountEdgeCrossings();
                temp.minimumAngle = GetComponent<EdgeCrossingCounter>().minimumAngle;
                temp.minAngRes = GetComponent<NodeAngularResolution>().CalculateAngularResolution();
                temp.edgeLength = GetComponent<EdgeLength>().CalculateEdgeLength();
                GetComponent<TwoDimensionalProjection>().RestorePositions();
                temp.cameraPosition = cam.position;
                observationList.Add(temp);

                _nodeOverlaps.Add(temp.nrNodeOverlaps);
                _edgeCrossings.Add(temp.nrEdgeCrossings);
                _minAngles.Add(temp.minimumAngle);
                _angRes.Add(temp.minAngRes);
                _edgeLengths.Add(temp.edgeLength);
            }
        }

        //Calculates the mean of each quality metric
        _nodeOverlapMean = MeanCalc(_nodeOverlaps);
        _edgeCrossMean = MeanCalc(_edgeCrossings);
        _minAngleMean = MeanCalc(_minAngles);
        _angResMean = MeanCalc(_angRes);
        _edgeLengthMean = MeanCalc(_edgeLengths);

        //Calculates the Standard Deviation of each quality metric
        _nodeOverlapSTD = STDCalc(_nodeOverlaps, _nodeOverlapMean);
        _edgeCrossSTD = STDCalc(_edgeCrossings, _edgeCrossMean);
        _minAngleSTD = STDCalc(_minAngles, _minAngleMean);
        _angResSTD = STDCalc(_angRes, _angResMean);
        _edgeLengthSTD = STDCalc(_edgeLengths, _edgeLengthMean);

        foreach(var currentView in observationList)
        {
            _ZnodeOverlap = ((float)currentView.nrNodeOverlaps - _nodeOverlapMean) / _nodeOverlapSTD;
            _ZedgeCross = ((float)currentView.nrEdgeCrossings - _edgeCrossMean) / _edgeCrossSTD;
            _ZminAngle = (currentView.minimumAngle - _minAngleMean) / _minAngleSTD;
            _ZangRes = (currentView.minAngRes - _angResMean) / _angResSTD;
            _ZedgeLength = (currentView.edgeLength - _edgeLengthMean) / _edgeLengthSTD;
            currentView.overallGrade = _ZminAngle + _ZangRes - _ZnodeOverlap - _ZedgeCross - _ZedgeLength;
            currentView._ZnodeOverlap = _ZnodeOverlap;
            currentView._ZedgeCross = _ZedgeCross;
            currentView._ZedgeLength = _ZedgeLength;
            currentView._ZangRes = _ZangRes;
            currentView._ZminAngle = _ZminAngle;
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
}
