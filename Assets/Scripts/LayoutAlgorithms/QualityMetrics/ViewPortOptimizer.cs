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
    public GameObject guidanceMarker;

    public List<List<QualityMetricViewPort>> globalObservationList;
    public List<QualityMetricViewPort> observationList, combinedObservationList;
    public QualityMetricViewPort temp;
    public QualityMetricSlider edge, node, crossAngle, angRes, edgeLength;
    public CreatePreviewButtons previewButtons;

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

    private float minEdgeCross, maxEdgeCross, minNodeOverlap, maxNodeOverlap, maxEdgeLength, minEdgeLength;
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
    }
	
	// Update is called once per frame
	void LateUpdate () {
		if(takeScreenshots)
        {
            int random;
            switch (screenshotCounter)
            {
                case 0:                    
                    random = UnityEngine.Random.Range(0, 10);
                    Camera.main.transform.position = combinedObservationList[random].cameraPosition;
                    if (Camera.main.name == "Camera")
                    {
                        Camera.main.transform.LookAt(transform);
                        CentralizeCamera();
                    }

                    foreach (var icon in opIcons)
                    {
                        icon.transform.LookAt(Camera.main.transform);
                    }
                    
                    previewButtons.textures[screenshotCounter] = ScreenCapture.CaptureScreenshotAsTexture(1);
                    previewButtons.views[screenshotCounter] = combinedObservationList[random];
                    imagesPaths.Add("Screenshots/viewNr" + random.ToString());
                    screenshotCounter++;

                    Debug.Log("viewNr " + random + " EdgeCrossAngle: " + combinedObservationList[random].edgeCrossAngle + " AngResRM: " + combinedObservationList[random].angResRM
                        + " normalizedEdgeLength: " + combinedObservationList[random].normalizedEdgeLength + " normalizedNodeOverlaps: " + combinedObservationList[random].normalizedNodeOverlaps
                        + " normalizedEdgeCrossings: " + combinedObservationList[random].normalizedEdgeCrossings);
                    break;
                case 1:
                    random = UnityEngine.Random.Range(10, 50);
                    Camera.main.transform.position = combinedObservationList[random].cameraPosition;
                    if (Camera.main.name == "Camera")
                    {
                        Camera.main.transform.LookAt(transform);
                        CentralizeCamera();
                    }

                    foreach (var icon in opIcons)
                    {
                        icon.transform.LookAt(Camera.main.transform);
                    }

                    previewButtons.textures[screenshotCounter] = ScreenCapture.CaptureScreenshotAsTexture(1);
                    previewButtons.views[screenshotCounter] = combinedObservationList[random];
                    imagesPaths.Add("Screenshots/viewNr" + random.ToString());
                    screenshotCounter++;
                    
                    Debug.Log("viewNr " + random + " EdgeCrossAngle: " + combinedObservationList[random].edgeCrossAngle + " AngResRM: " + combinedObservationList[random].angResRM
                        + " normalizedEdgeLength: " + combinedObservationList[random].normalizedEdgeLength + " normalizedNodeOverlaps: " + combinedObservationList[random].normalizedNodeOverlaps
                        + " normalizedEdgeCrossings: " + combinedObservationList[random].normalizedEdgeCrossings);
                    break;
                case 2:
                    random = UnityEngine.Random.Range(50, 145);
                    Camera.main.transform.position = combinedObservationList[random].cameraPosition;
                    if (Camera.main.name == "Camera")
                    {
                        Camera.main.transform.LookAt(transform);
                        CentralizeCamera();
                    }

                    foreach (var icon in opIcons)
                    {
                        icon.transform.LookAt(Camera.main.transform);
                    }
                    
                    previewButtons.textures[screenshotCounter] = ScreenCapture.CaptureScreenshotAsTexture(1);
                    previewButtons.views[screenshotCounter] = combinedObservationList[random];
                    imagesPaths.Add("Screenshots/viewNr" + random.ToString());
                    screenshotCounter++;
                    takeScreenshots = false;

                    //once all the images are generated, create buttons with the images
                    previewButtons.CallForCreatingButtons(imagesPaths);

                    Debug.Log("viewNr " + random + " EdgeCrossAngle: " + combinedObservationList[random].edgeCrossAngle + " AngResRM: " + combinedObservationList[random].angResRM
                        + " normalizedEdgeLength: " + combinedObservationList[random].normalizedEdgeLength + " normalizedNodeOverlaps: " + combinedObservationList[random].normalizedNodeOverlaps
                        + " normalizedEdgeCrossings: " + combinedObservationList[random].normalizedEdgeCrossings);

                    Debug.Log("MinEdgeCross: " + minEdgeCross + " minNodeOverlap: " + minNodeOverlap + " minEdgeLength: " + minEdgeLength);
                    Debug.Log("MaxEdgeCross: " + maxEdgeCross + " MaxNodeOverlap: " + maxNodeOverlap + " maxEdgeLength: " + maxEdgeLength);
                    break;
            }
        }
        if(printPositions)
        {
            printPositions = false;
            CentralizeCamera();
            //Debug.Log(Camera.main.pixelWidth + "    " + Camera.main.pixelHeight);
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
            pos += op.GetIcon().GetComponent<IconProperties>().newPos;
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
        int localOffset;
        Vector3 pos = observer.GetOperators()[0].GetIcon().transform.position;
        foreach(var op in observer.GetOperators())
        {
            if (Vector3.Distance(transform.position, pos) < Vector3.Distance(transform.position, op.GetIcon().transform.position)) pos = op.GetIcon().transform.position;
        }
        //localOffset = FindOffset(offset);
        //Debug.Log(localOffset);
        //pos = new Vector3(transform.position.x + Vector3.Distance(transform.position, pos) + localOffset, transform.position.y, transform.position.z);
        pos = new Vector3(transform.position.x + Vector3.Distance(transform.position, pos), transform.position.y, transform.position.z);
        return pos;
    }

    int FindOffset(int offset)
    {
        Vector3 transformUp = new Vector3();
        Camera.main.transform.position = new Vector3(furthestNodePos.x + offset, transform.position.y, transform.position.z);
        for (int i = 0; i < 360; i += 20)
        {
            transform.eulerAngles = new Vector3(i, 0, 0);
            transformUp = transform.up;
            for (int j = 0; j < 360; j += 20)
            {
                transform.RotateAround(transform.position, transformUp, 20f);
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
        for(int i = 0; i<360; i+=20)
        {
            transform.eulerAngles = new Vector3(i, 0, 0);
            transformUp = transform.up;
            for (int j=0; j<360; j+=20)
            {
                _toBreak = false;
                transform.RotateAround(transform.position,transformUp, 20f);
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
                temp = new QualityMetricViewPort();
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
                temp.angResRM = GetComponent<NodeAngularResolution>().CalculateAngularResolution();
                temp.edgeLength = GetComponent<EdgeLength>().CalculateEdgeLength();
                temp.cameraPosition = Camera.main.transform.position;
                temp.algorithm = current.currentLayout;
                temp.index = index;
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
            LocalScan();
        }
        else
        {
            GlobalScan();
        }
        /*for(int i=0; i<5; i++)
        {
            Debug.Log(combinedObservationList[i].algorithm + " overal grade: " + combinedObservationList[i].overallGrade + " nodeOverlaps :" + combinedObservationList[i].normalizedNodeOverlaps + " edgeCross :" + combinedObservationList[i].normalizedEdgeCrossings + " edgeLength :" + combinedObservationList[i].normalizedEdgeLength + " angularRes :" + combinedObservationList[i].angResRM + " edgeCrossRes :" + combinedObservationList[i].edgeCrossAngle);
        }*/
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

    void OnDisable()
    {
        //delete screenshots if exist
        DirectoryInfo dir = new DirectoryInfo("C:/Kliment/Master's Project/VRVis/Assets/Resources/Screenshots/");
        FileInfo[] files = dir.GetFiles("*.*");
        if (files.Length != 0)
        {
            foreach(FileInfo file in files)
            {
                file.Delete();
            }
        }
    }

    private void LocalScan()
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
        //Camera.main.transform.position = FindFurthestNode();
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

        foreach (var view in observationList)
        {
            view.normalizedEdgeCrossings = NormalizedEdgeCrossings(view.nrEdgeCrossings);
            view.normalizedNodeOverlaps = NormalizedNodeOverlaps(view.nrNodeOverlaps);
            view.normalizedEdgeLength = NormalizedEdgeLengths(view.edgeLength);
            view.overallGrade = ((crossAngle.qualityFactor * view.edgeCrossAngle) + (angRes.qualityFactor * view.angResRM) + (edgeLength.qualityFactor * view.normalizedEdgeLength)
                + (node.qualityFactor * view.normalizedNodeOverlaps) + (edge.qualityFactor * view.normalizedEdgeCrossings))
                / (crossAngle.qualityFactor + edge.qualityFactor + node.qualityFactor + angRes.qualityFactor + edgeLength.qualityFactor);
        }

        SortList(combinedObservationList);

        StartCoroutine(Screenshot());
        projectionPlane.gameObject.SetActive(false);
        if (Camera.main.name == "Camera") Camera.main.transform.LookAt(transform);
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
            //set camera to position of the furthest node + 2 meters
            Camera.main.transform.position = FindFurthestNode();
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
                view.overallGrade = ((crossAngle.qualityFactor * view.edgeCrossAngle) + (angRes.qualityFactor * view.angResRM) + (edgeLength.qualityFactor * view.normalizedEdgeLength) + (node.qualityFactor * view.normalizedNodeOverlaps) + (edge.qualityFactor * view.normalizedEdgeCrossings)) / (crossAngle.qualityFactor + edge.qualityFactor + node.qualityFactor + angRes.qualityFactor + edgeLength.qualityFactor);
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
        takeScreenshots = true;
        previewButtons.textures = new Texture2D[3];
        previewButtons.views = new QualityMetricViewPort[3];
    }
}
