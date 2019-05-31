using Assets.Scripts.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/*
 * Rotates a gameObject around the tree-graph
 * Calculates the number of node overlaps, edge crossings and angles between them
 * and saves it in QualityMetricViewport class for later calculation
 */

public class ViewPortOptimizer : MonoBehaviour, IMenueComponentListener {
    public Vector3 position;
    public bool globalScan, takeScreenshots, printPositions;
    public int nrOfViews, screenshotCounter, offset;
    public List<GeneralLayoutAlgorithm> algorithmsList;
    public GenericMenueComponent localScanListener, globalScanListener;
    public GameObject guidanceMarker, inputFieldWrapper, screenshotsContainer;
    public GameObject cameraRig;

    public List<List<QualityMetricViewPort>> globalObservationList;
    public List<QualityMetricViewPort> observationList, combinedObservationList, chosenViewpoints;
    //public QualityMetricViewPort temp;
    public QualityMetricSlider edgeCrossSlider, nodeOverlapSlider, edgeCrossAngSlider, angResSlider, edgeLengthSlider;
    public PreviewButtonsController previewButtons;

    private MultiDimensionalKMeansClustering multiDimKMeans;
    private Transform[] opIcons;
    private Transform oldParent, projectionPlane;
    private LayoutAlgorithm current;
    private Observer observer;
    private List<GameObject> guidanceMarkers;
    private List<string> imagesPaths;
    private int index = 0;
    private Vector3 furthestNodePos;
    // Lists of all metrics for calculating Mean and STD values
    private List<float> _edgeLengths, _angRes, _edgeCrossAngle;
    private List<float> _nodeOverlaps, _edgeCrossings;

    private float minEdgeCross, maxEdgeCross, minNodeOverlap, maxNodeOverlap, maxEdgeLength, minEdgeLength, delta;
    GeneralLayoutAlgorithm edgeAlg, nodeAlg;

    // counter for switching views
    private int counter = 0;

    private bool _toBreak;
	// Use this for initialization
	void Start () {
        observer = (Observer)FindObjectOfType(typeof(Observer));
        globalObservationList = new List<List<QualityMetricViewPort>>();
        observationList = new List<QualityMetricViewPort>();
        current = GetComponent<LayoutAlgorithm>();
        guidanceMarkers = new List<GameObject>();
        imagesPaths = new List<string>();
        multiDimKMeans = GetComponent<MultiDimensionalKMeansClustering>();
        delta = 1;
    }
	
	// Update is called once per frame
	void LateUpdate () {
		if(takeScreenshots)
        {
            switch (screenshotCounter)
            {
                case 0:
                    //disable controller game objects for not rendering
                    foreach(Transform child in cameraRig.transform)
                    {
                        if(child.gameObject.name.Contains("Controller"))
                        {
                            child.gameObject.SetActive(false);
                        }
                    }
                    Camera.main.transform.position = chosenViewpoints[screenshotCounter].cameraPosition;
                    Vector3 rotation = Camera.main.transform.eulerAngles;
                    Camera.main.transform.LookAt(transform);
                    CentralizeCamera();

                    foreach (var icon in opIcons)
                    {
                        icon.transform.LookAt(Camera.main.transform);
                    }

                    previewButtons.textures[screenshotCounter] = new Texture2D(2560, 1440);
                    previewButtons.textures[screenshotCounter].LoadImage(TakeScreenshot());
                    previewButtons.views[screenshotCounter] = chosenViewpoints[screenshotCounter];
                    imagesPaths.Add("Screenshots/viewNr" + chosenViewpoints[screenshotCounter].index.ToString());

                    /*Debug.Log("viewNr " + chosenViewpoints[screenshotCounter].index + " overallGrade " + chosenViewpoints[screenshotCounter].overallGrade + " EdgeCrossAngle: " + chosenViewpoints[screenshotCounter].edgeCrossAngle + " AngResRM: " + chosenViewpoints[screenshotCounter].angResRM
                        + " normalizedEdgeLength: " + chosenViewpoints[screenshotCounter].normalizedEdgeLength + " normalizedNodeOverlaps: " + chosenViewpoints[screenshotCounter].normalizedNodeOverlaps
                        + " normalizedEdgeCrossings: " + chosenViewpoints[screenshotCounter].normalizedEdgeCrossings);*/
                    Camera.main.transform.eulerAngles = rotation;
                    screenshotCounter++;
                    break;
                case 1:
                    Camera.main.transform.position = chosenViewpoints[screenshotCounter].cameraPosition;
                    rotation = Camera.main.transform.eulerAngles;
                    Camera.main.transform.LookAt(transform);
                    CentralizeCamera();

                    foreach (var icon in opIcons)
                    {
                        icon.transform.LookAt(Camera.main.transform);
                    }

                    previewButtons.textures[screenshotCounter] = new Texture2D(2560, 1440);
                    previewButtons.textures[screenshotCounter].LoadImage(TakeScreenshot());
                    previewButtons.views[screenshotCounter] = chosenViewpoints[screenshotCounter];
                    imagesPaths.Add("Screenshots/viewNr" + chosenViewpoints[screenshotCounter].index.ToString());

                    /*Debug.Log("viewNr " + chosenViewpoints[screenshotCounter].index + " overallGrade " + chosenViewpoints[screenshotCounter].overallGrade + " EdgeCrossAngle: " + chosenViewpoints[screenshotCounter].edgeCrossAngle + " AngResRM: " + chosenViewpoints[screenshotCounter].angResRM
                        + " normalizedEdgeLength: " + chosenViewpoints[screenshotCounter].normalizedEdgeLength + " normalizedNodeOverlaps: " + chosenViewpoints[screenshotCounter].normalizedNodeOverlaps
                        + " normalizedEdgeCrossings: " + chosenViewpoints[screenshotCounter].normalizedEdgeCrossings);*/
                    Camera.main.transform.eulerAngles = rotation;
                    screenshotCounter++;
                    break;
                case 2:
                    Camera.main.transform.position = chosenViewpoints[screenshotCounter].cameraPosition;
                    rotation = Camera.main.transform.eulerAngles;
                    Camera.main.transform.LookAt(transform);
                    CentralizeCamera();

                    foreach (var icon in opIcons)
                    {
                        icon.transform.LookAt(Camera.main.transform);
                    }

                    previewButtons.textures[screenshotCounter] = new Texture2D(2560, 1440);
                    previewButtons.textures[screenshotCounter].LoadImage(TakeScreenshot());
                    previewButtons.views[screenshotCounter] = chosenViewpoints[screenshotCounter];
                    imagesPaths.Add("Screenshots/viewNr" + chosenViewpoints[screenshotCounter].index.ToString());

                    //once all the images are generated, create buttons with the images
                    previewButtons.CallForCreatingButtons(imagesPaths, delta);
                    //reduce delta factor
                    delta = (delta * 9) / 10;

                    /*Debug.Log("viewNr " + chosenViewpoints[screenshotCounter].index + " overallGrade " + chosenViewpoints[screenshotCounter].overallGrade + " EdgeCrossAngle: " + chosenViewpoints[screenshotCounter].edgeCrossAngle + " AngResRM: " + chosenViewpoints[screenshotCounter].angResRM
                        + " normalizedEdgeLength: " + chosenViewpoints[screenshotCounter].normalizedEdgeLength + " normalizedNodeOverlaps: " + chosenViewpoints[screenshotCounter].normalizedNodeOverlaps
                        + " normalizedEdgeCrossings: " + chosenViewpoints[screenshotCounter].normalizedEdgeCrossings);

                    Debug.Log("MinEdgeCross: " + minEdgeCross + " minNodeOverlap: " + minNodeOverlap + " minEdgeLength: " + minEdgeLength);
                    Debug.Log("MaxEdgeCross: " + maxEdgeCross + " MaxNodeOverlap: " + maxNodeOverlap + " maxEdgeLength: " + maxEdgeLength);*/
                    
                    screenshotCounter=0;
                    takeScreenshots = false;

                    if (Camera.main.name == "Camera")
                    {
                        Camera.main.transform.LookAt(transform);
                    }
                    else
                    {
                        cameraRig.transform.position = combinedObservationList[0].cameraPosition;
                        cameraRig.transform.LookAt(transform);
                    }

                    //enable back controller game objects
                    foreach (Transform child in cameraRig.transform)
                    {
                        if (child.gameObject.name.Contains("Controller"))
                        {
                            child.gameObject.SetActive(true);
                        }
                    }
                    break;
            }
        }
        if(printPositions)
        {
            printPositions = false;
            Debug.Log("");
        }
	}

    /* Finds the center of the graph for rotation of the scanner object
     *  around the tree-graph
     */
    public Vector3 FindCenterOfGraph()
    {
        Vector3 pos = new Vector3();
        foreach(var op in observer.GetOperators())
        {
            //pos += op.GetIcon().GetComponent<IconProperties>().newPos;
            pos += op.GetIcon().GetComponent<IconProperties>().transform.position;
        }
        pos /= observer.GetOperators().Count;
        return pos;
    }

    /* Centers the camera  in the center of the graph
     * depending on gameobjects' position on screen
     */
     public void CentralizeCamera()
    {
        Vector3 pos = new Vector3();

        float minX = 500;
        float maxX = 0;
        float minY = 500;
        float maxY = 0;
        foreach (var icon in opIcons)
        {
            if (minX > Camera.main.WorldToScreenPoint(icon.position).x) minX = Camera.main.WorldToScreenPoint(icon.position).x;
            if (maxX < Camera.main.WorldToScreenPoint(icon.position).x) maxX = Camera.main.WorldToScreenPoint(icon.position).x;
            if (minY > Camera.main.WorldToScreenPoint(icon.position).y) minY = Camera.main.WorldToScreenPoint(icon.position).y;
            if (maxY < Camera.main.WorldToScreenPoint(icon.position).y) maxY = Camera.main.WorldToScreenPoint(icon.position).y;
        }
        pos.x = (minX + maxX) / 2;
        pos.y = (minY + maxY) / 2;
        pos.z = 1;
        Camera.main.transform.LookAt(Camera.main.ScreenToWorldPoint(pos));
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
        pos = new Vector3(transform.position.x + Vector3.Distance(transform.position, pos), transform.position.y, transform.position.z);
        return pos;
    }

    int FindOffset(int offset)
    {
        Vector3 transformUp = new Vector3();
        Camera.main.transform.position = new Vector3(furthestNodePos.x + offset, transform.position.y, transform.position.z);
        for (int i = 0; i < 360; i += 10)
        {
            transform.eulerAngles = new Vector3(i, 0, 0);
            transformUp = transform.up;
            for (int j = 0; j < 360; j += 10)
            {
                transform.RotateAround(transform.position, transformUp, 10f);
                Camera.main.transform.LookAt(transform);

                foreach (var icon in opIcons)
                {
                    if (Camera.main.WorldToViewportPoint(icon.position).x < 0 || Camera.main.WorldToViewportPoint(icon.position).x > 1
                     || Camera.main.WorldToViewportPoint(icon.position).y < 0 || Camera.main.WorldToViewportPoint(icon.position).y > 1)
                    {
                        offset++;
                        offset = FindOffset(offset);
                        return offset;
                    }
                }
            }
        }
        return offset;
    }

    // Repositiones the camera around the graph and calculates the quality metrics
    void ScanAndCalculate()
    {
        Vector3 transformUp = new Vector3();
        index = 0;
        GetComponent<TwoDimensionalProjection>().SetPlane();
        for(int i = 0; i<360; i+=10)
        {
            transform.eulerAngles = new Vector3(i, 0, 0);
            transformUp = transform.up;
            for (int j=0; j<360; j+=10)
            {
                _toBreak = false;
                transform.RotateAround(transform.position,transformUp, 10f);
                Camera.main.transform.LookAt(transform);
                // first check if current position already exists
                foreach (var view in observationList)
                {
                    if (Vector3.Distance(view.cameraPosition, Camera.main.transform.position) < 0.01f)
                    {
                        _toBreak = true;
                        break;
                    }
                }
                if (_toBreak) continue;

                QualityMetricViewPort temp = new QualityMetricViewPort();
                temp.listPos = new Vector3[observer.GetOperators().Count];
                //rotate icons towards camera before calculating node overlapping
                for(int icon = 0; icon<opIcons.Length; icon++)
                {
                    opIcons[icon].LookAt(Camera.main.transform);
                    temp.listPos[icon] = opIcons[icon].transform.position;
                }
                temp.nrNodeOverlaps = GetComponent<NodeOverlapping>().CalculateNodeOverlapping();
                GetComponent<TwoDimensionalProjection>().ProjectTree();
                temp.nrEdgeCrossings = GetComponent<EdgeCrossingCounter>().CountEdgeCrossings();
                temp.edgeCrossAngle = GetComponent<EdgeCrossingCounter>().edgeCrossRM;
                temp.angRes = GetComponent<NodeAngularResolution>().CalculateAngularResolution();
                temp.edgeLength = GetComponent<EdgeLength>().CalculateEdgeLength();
                temp.cameraPosition = Camera.main.transform.position;
                temp.algorithm = current.currentLayout;
                temp.index = index;

                //set initial values
                temp.initialNodeOverlaps = temp.nrNodeOverlaps;
                temp.initialEdgeCrossings = temp.nrEdgeCrossings;
                temp.initialEdgeCrossAngle = temp.edgeCrossAngle;
                temp.initialAngRes = temp.angRes;
                temp.initialEdgeLength = temp.edgeLength;

                GetComponent<TwoDimensionalProjection>().RestorePositions();
                observationList.Add(temp);
                combinedObservationList.Add(temp);
                index++;
            }
        }
        nrOfViews = observationList.Count;
    }

    void GetIcons()
    {
        opIcons = new Transform[observer.GetOperators().Count];
        for (int i = 0; i < opIcons.Length; i++)
        {
            if (observer.GetOperators()[i].GetIcon().transform.GetChild(0).name != "Plane") opIcons[i] = observer.GetOperators()[i].GetIcon().transform.GetChild(0);
            else opIcons[i] = observer.GetOperators()[i].GetIcon().transform.GetChild(1);
        }
    }
    private List<QualityMetricViewPort> SortList(List<QualityMetricViewPort> list)
    {
        list.Sort((b, a) => a.overallGrade.CompareTo(b.overallGrade));
        return list;
    }

    private List<QualityMetricViewPort> SortListEdgeCrossings(List<QualityMetricViewPort> list)
    {
        List<QualityMetricViewPort> returnList = new List<QualityMetricViewPort>();
        foreach (var obj in list) returnList.Add(obj);
        returnList.Sort((b, a) => a.normalizedEdgeCrossings.CompareTo(b.normalizedEdgeCrossings));
        return returnList;
    }

    private List<QualityMetricViewPort> SortListNodeOverlaps(List<QualityMetricViewPort> list)
    {
        List<QualityMetricViewPort> returnList = new List<QualityMetricViewPort>();
        foreach (var obj in list) returnList.Add(obj);
        returnList.Sort((b, a) => a.normalizedNodeOverlaps.CompareTo(b.normalizedNodeOverlaps));
        return returnList;
    }

    private List<QualityMetricViewPort> SortListAngRes(List<QualityMetricViewPort> list)
    {
        List<QualityMetricViewPort> returnList = new List<QualityMetricViewPort>();
        foreach (var obj in list) returnList.Add(obj);
        returnList.Sort((b, a) => a.angRes.CompareTo(b.angRes));
        return returnList;
    }

    private List<QualityMetricViewPort> SortListEdgeLength(List<QualityMetricViewPort> list)
    {
        List<QualityMetricViewPort> returnList = new List<QualityMetricViewPort>();
        foreach (var obj in list) returnList.Add(obj);
        returnList.Sort((b, a) => a.normalizedEdgeLength.CompareTo(b.normalizedEdgeLength));
        return returnList;
    }

    private List<QualityMetricViewPort> SortListEdgeCrossAngle(List<QualityMetricViewPort> list)
    {
        List<QualityMetricViewPort> returnList = new List<QualityMetricViewPort>();
        foreach (var obj in list) returnList.Add(obj);
        returnList.Sort((b, a) => a.edgeCrossAngle.CompareTo(b.edgeCrossAngle));
        return returnList;
    }

    private float MaximumEdgeCross(List<QualityMetricViewPort> list, bool getIndex)
    {
        int tempIndex = 0;
        float tempEdgeCross = 0;
        foreach (var item in list)
        {
            if (tempEdgeCross < item.nrEdgeCrossings)
            {
                tempEdgeCross = item.nrEdgeCrossings;
                if (getIndex) index = tempIndex;
            }
            tempIndex++;
        }
        return tempEdgeCross;
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
    private float MaximumEdgeLength(List<QualityMetricViewPort> list)
    {
        float max = 0;
        foreach (var item in list)
        {
            if (max < item.edgeLength) max = item.edgeLength;
        }
        return max;
    }

    private float MinimumEdgeCross(List<QualityMetricViewPort> list)
    {
        float min = maxEdgeCross;
        foreach (var item in list)
        {
            if (min > item.nrEdgeCrossings) min = item.nrEdgeCrossings;
        }
        return min;
    }

    private float MinimumNodeOverlap(List<QualityMetricViewPort> list)
    {
        float min = maxNodeOverlap;
        foreach (var item in list)
        {
            if (min > item.nrNodeOverlaps) min = item.nrNodeOverlaps;
        }
        return min;
    }
    
    private float MinimumEdgeLength(List<QualityMetricViewPort> list)
    {
        float min = maxEdgeLength;
        foreach (var item in list)
        {
            if (min > item.edgeLength) min = item.edgeLength;
        }
        return min;
    }

    private float NormalizedEdgeCrossings(float edgeCross)
    {
        float cubeRoot = 1f / 3f;
        if (minEdgeCross == maxEdgeCross) return 1;
        return 1 - ((Mathf.Pow(edgeCross, cubeRoot) - Mathf.Pow(minEdgeCross, cubeRoot)) / (Mathf.Pow(maxEdgeCross, cubeRoot) - Mathf.Pow(minEdgeCross, cubeRoot)));
    }
    private float NormalizedNodeOverlaps(float nodeOverlaps)
    {
        float cubeRoot = 1f / 3f;
        if (minNodeOverlap == maxNodeOverlap) return 1;
        float result = ((Mathf.Pow(nodeOverlaps, cubeRoot) - Mathf.Pow(minNodeOverlap, cubeRoot)) / (Mathf.Pow(maxNodeOverlap, cubeRoot) - Mathf.Pow(minNodeOverlap, cubeRoot)));
        return 1 - result;
    }
    private float NormalizedEdgeLengths(float edgeLengths)
    {
        float cubeRoot = 1f / 3f;
        if (minEdgeLength == maxEdgeLength) return 1;
        float result = ((Mathf.Pow(edgeLengths, cubeRoot) - Mathf.Pow(minEdgeLength, cubeRoot)) / (Mathf.Pow(maxEdgeLength, cubeRoot) - Mathf.Pow(minEdgeLength, cubeRoot)));
        return 1 - result;
    }

    public void menueChanged(GenericMenueComponent changedComponent)
    {
        if (changedComponent == localScanListener)
        {
            //reset delta value
            delta = 1f;
            LocalScan();
        }
        else
        {
            GlobalScan();
        }
        for (float i = 0; i < 5; i++)
        {
            GameObject marker = Instantiate(guidanceMarker);
            marker.transform.position = combinedObservationList[(int)i].cameraPosition;
            marker.transform.LookAt(Camera.main.transform);
            marker.GetComponent<MeshRenderer>().material.color = new Color(1, i / 5f, i / 5f);
            marker.GetComponent<ViewportMarkerButtonComponent>().viewPort = combinedObservationList[(int)i];
            marker.GetComponent<ViewportMarkerButtonComponent>().optimizer = this;
            guidanceMarkers.Add(marker);
        }
    }

    void OnEnable()
    {
        localScanListener.addListener(this);
        globalScanListener.addListener(this);
    }

    public void LocalScan()
    {
        globalScan = false;
        screenshotCounter = 0;
        imagesPaths = new List<string>();
        for (int i = guidanceMarkers.Count - 1; i >= 0; i--)
        {
            Destroy(guidanceMarkers[i]);
        }
        guidanceMarkers = new List<GameObject>();

        GetIcons();
        combinedObservationList = new List<QualityMetricViewPort>();
        chosenViewpoints = new List<QualityMetricViewPort>();
        maxEdgeCross = 0;
        maxNodeOverlap = 0;
        maxEdgeLength = 0;
        //set gameobject in the center of the graph
        transform.position = FindCenterOfGraph();
        //set camera to position of the furthest node + 2 meters
        oldParent = Camera.main.transform.parent;
        Camera.main.transform.parent = transform;
        furthestNodePos = FindFurthestNode();
        offset = FindOffset(3);

        transform.eulerAngles = new Vector3(0, 0, 0);
        Camera.main.transform.position = new Vector3(furthestNodePos.x + offset, transform.position.y, transform.position.z);
        Camera.main.transform.LookAt(transform);
        projectionPlane = Camera.main.transform.GetChild(0);
        projectionPlane.gameObject.SetActive(true);
        observationList = new List<QualityMetricViewPort>();
        ScanAndCalculate();
        Camera.main.transform.parent = oldParent;
        globalObservationList.Add(observationList);
        if (maxEdgeCross < MaximumEdgeCross(observationList, false))
        {
            maxEdgeCross = MaximumEdgeCross(observationList, false);
        }
        if (maxNodeOverlap < MaximumNodeOverlap(observationList))
        {
            maxNodeOverlap = MaximumNodeOverlap(observationList);
        }
        if (maxEdgeLength < MaximumEdgeLength(observationList))
        {
            maxEdgeLength = MaximumEdgeLength(observationList);
        }

        minEdgeCross = MinimumEdgeCross(observationList);
        minNodeOverlap = MinimumNodeOverlap(observationList);
        minEdgeLength = MinimumEdgeLength(observationList);

        ReadjustQualityMetricValues();

        GenerateClustersAndCreateScreenshots();

        projectionPlane.gameObject.SetActive(false);
    }

    //Readjust quality metric values with updated weights
    public void ReadjustQualityMetricValues()
    {
        if (maxEdgeCross < MaximumEdgeCross(observationList, false))
        {
            maxEdgeCross = MaximumEdgeCross(observationList, false);
        }
        if (maxNodeOverlap < MaximumNodeOverlap(observationList))
        {
            maxNodeOverlap = MaximumNodeOverlap(observationList);
        }
        if (maxEdgeLength < MaximumEdgeLength(observationList))
        {
            maxEdgeLength = MaximumEdgeLength(observationList);
        }

        foreach (var view in observationList)
        {
            view.normalizedEdgeCrossings = NormalizedEdgeCrossings(view.initialEdgeCrossings);
            view.normalizedNodeOverlaps = NormalizedNodeOverlaps(view.initialNodeOverlaps);
            view.edgeCrossAngle = view.initialEdgeCrossAngle;
            view.angRes = view.initialAngRes;
            view.normalizedEdgeLength = NormalizedEdgeLengths(view.initialEdgeLength);
            //multiply values by their weights
            view.normalizedNodeOverlaps *= nodeOverlapSlider.qualityFactor;
            view.normalizedEdgeCrossings *= edgeCrossSlider.qualityFactor;
            view.edgeCrossAngle *= edgeCrossAngSlider.qualityFactor;
            view.angRes *= angResSlider.qualityFactor;
            view.normalizedEdgeLength *= edgeLengthSlider.qualityFactor;
            /*view.overallGrade = ((edgeCrossAngSlider.qualityFactor * view.edgeCrossAngle) + (angResSlider.qualityFactor * view.angRes) + (edgeLengthSlider.qualityFactor * view.normalizedEdgeLength)
                + (nodeOverlapSlider.qualityFactor * view.normalizedNodeOverlaps) + (edgeCrossSlider.qualityFactor * view.normalizedEdgeCrossings))
                / (edgeCrossAngSlider.qualityFactor + edgeCrossSlider.qualityFactor + nodeOverlapSlider.qualityFactor + angResSlider.qualityFactor + edgeLengthSlider.qualityFactor);*/
            view.overallGrade = (view.normalizedEdgeCrossings + view.normalizedEdgeLength + view.normalizedNodeOverlaps + view.edgeCrossAngle + view.angRes)
                / (edgeCrossAngSlider.qualityFactor + edgeCrossSlider.qualityFactor + nodeOverlapSlider.qualityFactor + angResSlider.qualityFactor + edgeLengthSlider.qualityFactor);
        }
    }

    public void GenerateClustersAndCreateScreenshots()
    {
        //Cluster views using KMeans algorithm dependent on quality metrics
        chosenViewpoints = multiDimKMeans.Clusters(observationList);
        StartCoroutine(Screenshot());
        //input field wrapper set false, screenshots set true
        inputFieldWrapper.SetActive(false);
        screenshotsContainer.SetActive(true);
    }

    private void GlobalScan()
    {
        for (int i = guidanceMarkers.Count - 1; i >= 0; i--)
        {
            Destroy(guidanceMarkers[i]);
        }
        guidanceMarkers = new List<GameObject>();
        GetIcons();
        combinedObservationList = new List<QualityMetricViewPort>();
        maxEdgeCross = 0;
        maxNodeOverlap = 0;
        maxEdgeLength = 0;
        index = 0;
        globalScan = true;
        globalObservationList = new List<List<QualityMetricViewPort>>();
        projectionPlane = Camera.main.transform.GetChild(0);
        projectionPlane.gameObject.SetActive(true);
        oldParent = Camera.main.transform.parent;
        foreach (var algorithm in algorithmsList)
        {
            algorithm.PreScanCalculation();
            //set gameobject in the center of the graph
            transform.position = FindCenterOfGraph();
            Camera.main.transform.parent = transform;
            furthestNodePos = FindFurthestNode();
            offset = FindOffset(3);
            transform.eulerAngles = new Vector3(0, 0, 0);
            Camera.main.transform.position = new Vector3(furthestNodePos.x + offset, transform.position.y, transform.position.z);
            Camera.main.transform.LookAt(transform);
            observationList = new List<QualityMetricViewPort>();
            ScanAndCalculate();
            Camera.main.transform.parent = oldParent;
            globalObservationList.Add(observationList);
        }
        foreach (var list in globalObservationList)
        {
            if (maxEdgeCross < MaximumEdgeCross(list, false))
            {
                maxEdgeCross = MaximumEdgeCross(list, false);
                //listIndex++;
            }
            if (maxNodeOverlap < MaximumNodeOverlap(list))
            {
                maxNodeOverlap = MaximumNodeOverlap(list);
            }
            if (maxEdgeLength < MaximumEdgeLength(list))
            {
                maxEdgeLength = MaximumEdgeLength(list);
            }
        }
        foreach (var list in globalObservationList)
        {
            foreach (var view in list)
            {
                view.normalizedEdgeCrossings = NormalizedEdgeCrossings(view.nrEdgeCrossings);
                view.normalizedNodeOverlaps = NormalizedNodeOverlaps(view.nrNodeOverlaps);
                view.normalizedEdgeLength = NormalizedEdgeLengths(view.edgeLength);
                view.overallGrade = ((edgeCrossAngSlider.qualityFactor * view.edgeCrossAngle) + (angResSlider.qualityFactor * view.angRes) + (edgeLengthSlider.qualityFactor * view.normalizedEdgeLength) + (nodeOverlapSlider.qualityFactor * view.normalizedNodeOverlaps) + (edgeCrossSlider.qualityFactor * view.normalizedEdgeCrossings)) / (edgeCrossAngSlider.qualityFactor + edgeCrossSlider.qualityFactor + nodeOverlapSlider.qualityFactor + angResSlider.qualityFactor + edgeLengthSlider.qualityFactor);
            }
        }
        SortList(combinedObservationList);
        if (current.currentLayout.GetType().Equals(typeof(RDTAlgorithm))) CalculateRDT();
        else current.currentLayout.PlaceEdges();

        transform.position = FindCenterOfGraph();
        Camera.main.transform.position = combinedObservationList[counter].cameraPosition;
        Camera.main.transform.parent = oldParent;
        projectionPlane.gameObject.SetActive(false);
        if (Camera.main.name == "Camera") Camera.main.transform.LookAt(transform);
    }

    public void CloseAllMenus()
    {
        for(int i=guidanceMarkers.Count -1; i >= 0; i--)
        {
            Destroy(guidanceMarkers[i]);
        }
    }

    public void CalculateRDT()
    {
        int x = 0;
        for (int op = 0; op < observer.GetOperators().Count; op++)
        {
            if (observer.GetOperators()[op].Children != null)
            {
                if (observer.GetOperators()[op].Children.Count > 0)
                {
                    Vector3 avgPt = new Vector3();
                    Vector3 refPt = new Vector3();
                    Vector3 sum = new Vector3();
                    for (int i = 0; i < observer.GetOperators()[op].Children.Count; i++)
                    {
                        sum += observer.GetOperators()[op].Children[i].GetIcon().GetComponent<IconProperties>().newPos;
                        x++;
                    }
                    avgPt = sum / observer.GetOperators()[op].Children.Count;
                    avgPt.x = observer.GetOperators()[op].GetIcon().GetComponent<IconProperties>().newPos.x;
                    avgPt.z = observer.GetOperators()[op].GetIcon().GetComponent<IconProperties>().newPos.z;
                    refPt = Vector3.Lerp(observer.GetOperators()[op].GetIcon().GetComponent<IconProperties>().newPos, avgPt, 0.5f);
                    observer.GetOperators()[op].GetIcon().GetComponent<IconProperties>().refPoint = refPt;
                    for (int i = 0; i < observer.GetOperators()[op].Children.Count; i++)
                    {
                        observer.GetOperators()[op].Children[i].GetComponent<LineRenderer>().positionCount = 3;
                        observer.GetOperators()[op].Children[i].GetComponent<LineRenderer>().SetPosition(0, observer.GetOperators()[op].GetIcon().GetComponent<IconProperties>().newPos);
                        observer.GetOperators()[op].Children[i].GetComponent<LineRenderer>().SetPosition(1, refPt);
                        observer.GetOperators()[op].Children[i].GetComponent<LineRenderer>().SetPosition(2, observer.GetOperators()[op].Children[i].GetIcon().GetComponent<IconProperties>().newPos);
                    }
                }
            }
        }
    }

    IEnumerator Screenshot()
    {
        yield return 0;
        screenshotCounter = 0;
        imagesPaths = new List<string>();
        previewButtons.textures = new Texture2D[3];
        previewButtons.views = new QualityMetricViewPort[3];
        takeScreenshots = true;
    }

    Byte[] TakeScreenshot()
    {
        int resWidthN = 2560;
        int resHeightN = 1440;
        RenderTexture rt = new RenderTexture(resWidthN, resHeightN, 24);
        Camera.main.targetTexture = rt;
        

        Texture2D screenShot = new Texture2D(resWidthN, resHeightN, TextureFormat.RGBA32, false);
        Camera.main.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidthN, resHeightN), 0, 0);
        screenShot.Apply();
        Camera.main.targetTexture = null;
        RenderTexture.active = null;

        byte[] bytes = screenShot.EncodeToPNG();
        /*string filename = ScreenShotName(resWidthN, resHeightN, name);

        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("Took screenshot to: {0}", filename));*/
        return bytes;
    }
    public string ScreenShotName(int width, int height, string name)
    {

        string strPath = "";
        string path = "C:/Kliment/Master's Project/VRVis/Assets/Resources";
        strPath = string.Format("{0}/screen_{1}x{2}_{3}.png",
                             path,
                             width, height,name);

        return strPath;
    }
    List<Vector2> FindDisparityVectors(List<QualityMetricViewPort> list)
    {
        List<Vector2> disparityList = new List<Vector2>();
        foreach(var view in list)
        {
            Vector2 disparity = new Vector2();
            disparity.x = CalculateDisparity(view);
            disparity.y = view.index;
            disparityList.Add(disparity);
        }
        return SortByDisparity(disparityList);
    }

    List<Vector2> SortByDisparity(List<Vector2> disparityList)
    {
        disparityList.Sort((b, a) => a.x.CompareTo(b.x));
        return disparityList;
    }

    QualityMetricViewPort SortDisparityByEdgeCrossing(List<Vector2> disparityList)
    {
        QualityMetricViewPort edgeCrossMaxDisparity = combinedObservationList[0];
        float edgeCross = combinedObservationList[(int)disparityList[0].y].normalizedEdgeCrossings;
        int index = 0;
        for(int i=0; i<disparityList.Count/2; i++)
        {
            if (edgeCross < combinedObservationList[(int)disparityList[i].y].normalizedEdgeCrossings)
            {
                edgeCross = combinedObservationList[(int)disparityList[i].y].normalizedEdgeCrossings;
                index = (int)disparityList[i].y;
                edgeCrossMaxDisparity = combinedObservationList[(int)disparityList[i].y];
            }
        }
        return edgeCrossMaxDisparity;
    }

    QualityMetricViewPort SortDisparityByNodeOverlaps(List<Vector2> disparityList)
    {
        QualityMetricViewPort nodeOverlapsMaxDisparity = combinedObservationList[0];
        float nodeOverlaps = combinedObservationList[(int)disparityList[0].y].normalizedNodeOverlaps;
        int index = 0;
        for (int i = 0; i < disparityList.Count / 2; i++)
        {
            if (nodeOverlaps < combinedObservationList[(int)disparityList[i].y].normalizedNodeOverlaps)
            {
                nodeOverlaps = combinedObservationList[(int)disparityList[i].y].normalizedNodeOverlaps;
                index = (int)disparityList[i].y;
                nodeOverlapsMaxDisparity = combinedObservationList[(int)disparityList[i].y];
            }
        }
        return nodeOverlapsMaxDisparity;
    }
    QualityMetricViewPort SortDisparityByEdgeLength(List<Vector2> disparityList)
    {
        QualityMetricViewPort edgeLengthMaxDisparity = combinedObservationList[0];
        float edgeLength = combinedObservationList[(int)disparityList[0].y].normalizedEdgeLength;
        int index = 0;
        for (int i = 0; i < disparityList.Count / 2; i++)
        {
            if (edgeLength < combinedObservationList[(int)disparityList[i].y].normalizedEdgeLength)
            {
                edgeLength = combinedObservationList[(int)disparityList[i].y].normalizedEdgeLength;
                index = (int)disparityList[i].y;
                edgeLengthMaxDisparity = combinedObservationList[(int)disparityList[i].y];
            }
        }
        return edgeLengthMaxDisparity;
    }
    QualityMetricViewPort SortDisparityByAngularResolution(List<Vector2> disparityList)
    {
        QualityMetricViewPort angResMaxDisparity = combinedObservationList[0];
        float angRes = combinedObservationList[(int)disparityList[0].y].angRes;
        int index = 0;
        for (int i = 0; i < disparityList.Count / 2; i++)
        {
            if (angRes < combinedObservationList[(int)disparityList[i].y].angRes)
            {
                angRes = combinedObservationList[(int)disparityList[i].y].angRes;
                index = (int)disparityList[i].y;
                angResMaxDisparity = combinedObservationList[(int)disparityList[i].y];
            }
        }
        return angResMaxDisparity;
    }
    QualityMetricViewPort SortDisparityByEdgeCrossAngle(List<Vector2> disparityList)
    {
        QualityMetricViewPort edgeCrossAngleMaxDisparity = combinedObservationList[0];
        float edgeCrossAngle = combinedObservationList[(int)disparityList[0].y].edgeCrossAngle;
        int index = 0;
        for (int i = 0; i < disparityList.Count / 2; i++)
        {
            if (edgeCrossAngle < combinedObservationList[(int)disparityList[i].y].edgeCrossAngle)
            {
                edgeCrossAngle = combinedObservationList[(int)disparityList[i].y].edgeCrossAngle;
                index = (int)disparityList[i].y;
                edgeCrossAngleMaxDisparity = combinedObservationList[(int)disparityList[i].y];
            }
        }
        return edgeCrossAngleMaxDisparity;
    }

    float CalculateDisparity(QualityMetricViewPort view)
    {
        float disparity = 0;
        disparity += Mathf.Abs(view.normalizedEdgeCrossings - view.normalizedNodeOverlaps);
        disparity += Mathf.Abs(view.normalizedEdgeCrossings - view.normalizedEdgeLength);
        disparity += Mathf.Abs(view.normalizedEdgeCrossings - view.angRes);
        disparity += Mathf.Abs(view.normalizedEdgeCrossings - view.edgeCrossAngle);
        disparity += Mathf.Abs(view.normalizedNodeOverlaps - view.normalizedEdgeLength);
        disparity += Mathf.Abs(view.normalizedNodeOverlaps - view.angRes);
        disparity += Mathf.Abs(view.normalizedNodeOverlaps - view.edgeCrossAngle);
        disparity += Mathf.Abs(view.normalizedEdgeLength - view.angRes);
        disparity += Mathf.Abs(view.normalizedEdgeLength - view.edgeCrossAngle);
        disparity += Mathf.Abs(view.angRes - view.edgeCrossAngle);
        return disparity;
    }
}
