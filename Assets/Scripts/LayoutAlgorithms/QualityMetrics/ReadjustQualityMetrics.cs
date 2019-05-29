using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ReadjustQualityMetrics : MonoBehaviour, IMenueComponentListener {
    public List<GameObject> other;
    public bool readjust;
    public GenericMenueComponent listener;
    public float delta;
    public Transform rightController;

    private SteamVR_TrackedObject _trackedObj;
    private SteamVR_Controller.Device _device;
    private QualityMetricViewPort _qualityMetricsValues;
    private ViewPortOptimizer _viewport;
    private InstantRotationOfGraph _rotate;
    private StudieScript _studyScript;
    private PreviewButtonsController _buttonsController;
    private RaycastHit _hit;
    private Ray ray;
    private Camera mainCamera;
    private Vector3 smallSize, bigSize, vrSize, backPos, frontPos;
    private string _dataPath, _studyStepDirectory, _file;
    private float edgeCrossingWeight, nodeOverlapWeight, edgeLenWeight, angResWeight, edgeCrossResWeight, min, max, sum, sumNoDelta, edgeCrossNoDelta, nodeOverlapNoDelta, edgeLenNoDelta, angResNoDelta, edgeCrossAngNoDelta;
    private int _studyCounter;
    // Use this for initialization
    void Start ()
    {
        mainCamera = Camera.main;
        if (mainCamera.name == "Camera (eye)")
        {
            _trackedObj = rightController.GetComponent<SteamVR_TrackedObject>();
            _device = SteamVR_Controller.Input((int)_trackedObj.index);
        }
        _viewport = (ViewPortOptimizer)FindObjectOfType(typeof(ViewPortOptimizer));
        _rotate = (InstantRotationOfGraph)FindObjectOfType(typeof(InstantRotationOfGraph));
        _studyScript = (StudieScript)FindObjectOfType(typeof(StudieScript));
        _buttonsController = (PreviewButtonsController)FindObjectOfType(typeof(PreviewButtonsController));

        other = new List<GameObject>();
        _qualityMetricsValues = GetComponent<QualityMetricViewPort>();
        foreach (Transform child in transform.parent)
        {
            if (child == transform) continue;
            other.Add(child.gameObject);
        }
        min = -2;
        max = 2;
        smallSize = new Vector3(0.08f, 0.08f, 0.08f);
        bigSize = new Vector3(0.16f, 0.16f, 0.16f);
        vrSize = new Vector3(0.64f, 0.64f, 0.64f);
        backPos = transform.localPosition;
        frontPos = backPos + new Vector3(0, 0, -50);
        
    }
	
	// Update is called once per frame
	void Update () {
        if(mainCamera.name == "Camera")
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out _hit, 100))
            {
                if(_hit.collider.gameObject == transform.GetChild(0).gameObject && transform.localScale != bigSize)
                {
                    transform.localScale = bigSize;
                    transform.localPosition = frontPos;
                }
                else if(_hit.collider.gameObject != transform.GetChild(0).gameObject && transform.localScale == bigSize)
                {
                    transform.localScale = smallSize;
                    transform.localPosition = backPos;
                }

                if(_hit.collider.gameObject == transform.GetChild(0).gameObject && Input.GetMouseButtonDown(1))
                {
                    _rotate.GraphRotation(GetComponent<QualityMetricViewPort>().cameraPosition);
                }
            }
            else
            {
                if(transform.localScale == bigSize)
                {
                    transform.localScale = smallSize;
                    transform.localPosition = backPos;
                }
            }
        }
        else if(mainCamera.name == "Camera (eye)")
        {
            ray = new Ray(rightController.position, rightController.forward);
            if (Physics.Raycast(ray, out _hit, 100))
            {
                if (_hit.collider.gameObject == transform.GetChild(0).gameObject && _device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
                {
                    _rotate.GraphRotation(GetComponent<QualityMetricViewPort>().cameraPosition);
                }
            }
        }
    }

    void ReadjustMetrics()
    {
        QualityMetricViewPort temp = new QualityMetricViewPort();
        edgeCrossingWeight = 0;
        nodeOverlapWeight = 0;
        edgeLenWeight = 0;
        angResWeight = 0;
        edgeCrossResWeight = 0;
        //calculate summed up differences
        foreach(var obj in other)
        {
            temp = obj.GetComponent<QualityMetricViewPort>();
            edgeCrossingWeight += (_qualityMetricsValues.normalizedEdgeCrossings - temp.normalizedEdgeCrossings) * delta;
            edgeCrossNoDelta += (_qualityMetricsValues.normalizedEdgeCrossings - temp.normalizedEdgeCrossings);

            nodeOverlapWeight += (_qualityMetricsValues.normalizedNodeOverlaps - temp.normalizedNodeOverlaps) * delta;
            nodeOverlapNoDelta += (_qualityMetricsValues.normalizedNodeOverlaps - temp.normalizedNodeOverlaps);

            edgeLenWeight += (_qualityMetricsValues.normalizedEdgeLength - temp.normalizedEdgeLength) * delta;
            edgeLenNoDelta += (_qualityMetricsValues.normalizedEdgeLength - temp.normalizedEdgeLength);

            angResWeight += (_qualityMetricsValues.angResRM - temp.angResRM) * delta;
            angResNoDelta += (_qualityMetricsValues.angResRM - temp.angResRM);

            edgeCrossResWeight += (_qualityMetricsValues.edgeCrossAngle - temp.edgeCrossAngle) * delta;
            edgeCrossAngNoDelta += (_qualityMetricsValues.edgeCrossAngle - temp.edgeCrossAngle);
        }

        //normalize on scale between 0-2
        edgeCrossingWeight = Normalize(edgeCrossingWeight);
        edgeCrossNoDelta = Normalize(edgeCrossNoDelta);

        nodeOverlapWeight = Normalize(nodeOverlapWeight);
        nodeOverlapNoDelta = Normalize(nodeOverlapNoDelta);

        edgeLenWeight = Normalize(edgeLenWeight);
        edgeLenNoDelta = Normalize(edgeLenNoDelta);

        angResWeight = Normalize(angResWeight);
        angResNoDelta = Normalize(angResNoDelta);

        edgeCrossResWeight = Normalize(edgeCrossResWeight);
        edgeCrossAngNoDelta = Normalize(edgeCrossAngNoDelta);

        //multiply to current values of quality metric weights
        edgeCrossingWeight = edgeCrossingWeight * _viewport.edge.qualityFactor;
        edgeCrossNoDelta = edgeCrossNoDelta * _viewport.edge.qualityFactor;

        nodeOverlapWeight = nodeOverlapWeight * _viewport.node.qualityFactor;
        nodeOverlapNoDelta = nodeOverlapNoDelta * _viewport.node.qualityFactor;

        edgeLenWeight = edgeLenWeight * _viewport.edgeLength.qualityFactor;
        edgeLenNoDelta = edgeLenNoDelta * _viewport.edgeLength.qualityFactor;

        angResWeight = angResWeight * _viewport.angRes.qualityFactor;
        angResNoDelta = angResNoDelta * _viewport.angRes.qualityFactor;

        edgeCrossResWeight = edgeCrossResWeight * _viewport.edgeCrossRes.qualityFactor;
        edgeCrossAngNoDelta = edgeCrossAngNoDelta * _viewport.edgeCrossRes.qualityFactor;

        //normalize new values so their sum is 5
        sum = edgeCrossingWeight + nodeOverlapWeight + edgeLenWeight + angResWeight + edgeCrossResWeight;
        sumNoDelta = edgeCrossNoDelta + nodeOverlapNoDelta + edgeLenNoDelta + angResNoDelta + edgeCrossAngNoDelta;

        sum = 5 / sum;
        sumNoDelta = 5 / sumNoDelta;

        edgeCrossingWeight *= sum;
        edgeCrossNoDelta *= sum;

        nodeOverlapWeight *= sum;
        nodeOverlapNoDelta *= sum;

        edgeLenWeight *= sum;
        edgeLenNoDelta *= sum;

        angResWeight *= sum;
        angResNoDelta *= sum;

        edgeCrossResWeight *= sum;
        edgeCrossAngNoDelta *= sum;
        //update current values
        _viewport.edge.qualityFactor = edgeCrossingWeight;
        _viewport.node.qualityFactor = nodeOverlapWeight;
        _viewport.edgeLength.qualityFactor = edgeLenWeight;
        _viewport.angRes.qualityFactor = angResWeight;
        _viewport.edgeCrossRes.qualityFactor = edgeCrossResWeight;

        //update sliders
        _viewport.edge.MoveSlideFromValue(edgeCrossingWeight);
        _viewport.node.MoveSlideFromValue(nodeOverlapWeight);
        _viewport.edgeLength.MoveSlideFromValue(edgeLenWeight);
        _viewport.angRes.MoveSlideFromValue(angResWeight);
        _viewport.edgeCrossRes.MoveSlideFromValue(edgeCrossResWeight);

        //Rescan tree again
        _viewport.LocalScan();
    }

    float Normalize(float value)
    {
        return ((value - min) / (max - min)) * 2;
    }

    public void menueChanged(GenericMenueComponent changedComponent)
    {
        //Set rotation of graph back to 0
        _rotate.SetBackToZero();

        ReadjustMetrics();
        AddLogDataToFile();
    }
    //All The logging is done here!
    void AddLogDataToFile()
    {
        //Study step directory - each step create new directory where logged data and screenshots are saved
        _studyStepDirectory = _studyScript.studyTrialDirectory + "\\StudyStep" + _buttonsController.trialNr.ToString();
        _studyStepDirectory = Directory.CreateDirectory(_studyStepDirectory).FullName;

        //log data in file
        _file = _studyStepDirectory + "\\StudyData" + _buttonsController.trialNr.ToString() + ".txt";
        using (StreamWriter sw = File.CreateText(_file))
        {
            sw.WriteLine("Step number " + _buttonsController.trialNr.ToString());
            sw.WriteLine("Current algorithm: " + _studyScript.algorithm);
            sw.WriteLine("Current task: " + _studyScript.task);
            sw.WriteLine("");
            sw.WriteLine("Edge crossings current weight: " + _viewport.edge.qualityFactor.ToString());
            sw.WriteLine("Node overlapping current weight: " + _viewport.node.qualityFactor.ToString());
            sw.WriteLine("Edge crossings angle current weight: " + _viewport.edgeCrossRes.qualityFactor.ToString());
            sw.WriteLine("Angular resolution current weight: " + _viewport.angRes.qualityFactor.ToString());
            sw.WriteLine("Edge length current weight: " + _viewport.edgeLength.qualityFactor.ToString());
            sw.WriteLine("");
            sw.WriteLine("Camera offset " + _viewport.offset.ToString());

            //For each option log its parameters
            //First save chosen option
            sw.WriteLine("");
            sw.WriteLine("");
            sw.WriteLine("");
            sw.WriteLine("CHOSEN OPTION");
            sw.WriteLine("");
            sw.WriteLine("Overall grade: " + _qualityMetricsValues.overallGrade.ToString());
            sw.WriteLine("Edge crossings grade: " + _qualityMetricsValues.normalizedEdgeCrossings);
            sw.WriteLine("Node overlapping grade: " + _qualityMetricsValues.normalizedNodeOverlaps);
            sw.WriteLine("Edge crossing angle grade: " + _qualityMetricsValues.edgeCrossAngle);
            sw.WriteLine("Angular resolution grade: " + _qualityMetricsValues.angResRM);
            sw.WriteLine("Edge length grade: " + _qualityMetricsValues.normalizedEdgeLength);
            
            for(int i=0; i<transform.parent.childCount; i++)
            {
                if(transform.parent.GetChild(i) != transform)
                {
                    //Other alternatives
                    sw.WriteLine("");
                    sw.WriteLine("");
                    sw.WriteLine("");
                    sw.WriteLine("OTHER ALTERNATIVE");
                    sw.WriteLine("");
                    sw.WriteLine("Overall grade: " + transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().overallGrade.ToString());
                    sw.WriteLine("Edge crossings grade: " + transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().normalizedEdgeCrossings);
                    sw.WriteLine("Node overlapping grade: " + transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().normalizedNodeOverlaps);
                    sw.WriteLine("Edge crossing angle grade: " + transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().edgeCrossAngle);
                    sw.WriteLine("Angular resolution grade: " + transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().angResRM);
                    sw.WriteLine("Edge length grade: " + transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().normalizedEdgeLength);
                }
            }
        }
        // Save screenshots
        for(int i=0; i<transform.parent.childCount; i++)
        {
            //Chosen option
            if(transform.parent.GetChild(i) == transform)
            {
                SaveTextureToFile((Texture2D)GetComponent<RawImage>().texture, _studyStepDirectory + "\\ChosenOption.png");
            }
            //Alternative option
            else
            {
                SaveTextureToFile((Texture2D)transform.parent.GetChild(i).gameObject.GetComponent<RawImage>().texture, _studyStepDirectory + "\\Alternative" + i.ToString() + ".png");
            }
        }
        _buttonsController.trialNr++;
    }

    public void CloseAllMenus()
    {

    }
    void OnEnable()
    {
        if(!listener.getListeners().Contains(this)) listener.addListener(this);
    }

    void SaveTextureToFile(Texture2D texture, string path)
    {
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
    }
}
