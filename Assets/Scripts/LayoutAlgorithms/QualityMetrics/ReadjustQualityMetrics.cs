using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ReadjustQualityMetrics : MonoBehaviour, IMenueComponentListener {
    public List<GameObject> other;
    public bool readjust;
    public GenericMenueComponent listener;
    public float delta, timePassedOnView;
    public Transform rightController;
    public Transform leftController;

    private SteamVR_TrackedObject _rightTrackedObj, _leftTrackedObject;
    private SteamVR_Controller.Device _rightDevice, _leftDevice;
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
    private float edgeCrossingWeight, nodeOverlapWeight, edgeLenWeight, angResWeight, edgeCrossResWeight, min, max, sum;
    private int _studyCounter;
    // Use this for initialization
    void Start ()
    {
        mainCamera = Camera.main;
        if (mainCamera.name == "Camera (eye)")
        {
            _rightTrackedObj = rightController.GetComponent<SteamVR_TrackedObject>();
            _rightDevice = SteamVR_Controller.Input((int)_rightTrackedObj.index);
            _leftTrackedObject = leftController.GetComponent<SteamVR_TrackedObject>();
            _leftDevice = SteamVR_Controller.Input((int)_leftTrackedObject.index);
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
                if(_hit.collider.gameObject == transform.GetChild(0).gameObject && Input.GetMouseButtonDown(1))
                {
                    _rotate.GraphRotation(GetComponent<QualityMetricViewPort>().cameraPosition);
                    _buttonsController.currentView = GetComponent<QualityMetricViewPort>();
                }
            }
        }
        else if(mainCamera.name == "Camera (eye)")
        {
            ray = new Ray(rightController.position, rightController.forward);
            if (Physics.Raycast(ray, out _hit, 100))
            {
                if (_hit.collider.gameObject == transform.GetChild(0).gameObject && _rightDevice.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
                {
                    _rotate.GraphRotation(GetComponent<QualityMetricViewPort>().cameraPosition);
                    _buttonsController.currentView = GetComponent<QualityMetricViewPort>();
                }
            }
        }
        if(_buttonsController.currentView == GetComponent<QualityMetricViewPort>())
        {
            timePassedOnView += Time.deltaTime;
        }
    }

    public void ReadjustMetrics()
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
            edgeCrossingWeight += (_qualityMetricsValues.normalizedEdgeCrossings - temp.normalizedEdgeCrossings);
            nodeOverlapWeight += (_qualityMetricsValues.normalizedNodeOverlaps - temp.normalizedNodeOverlaps);
            edgeLenWeight += (_qualityMetricsValues.normalizedEdgeLength - temp.normalizedEdgeLength);
            angResWeight += (_qualityMetricsValues.angRes - temp.angRes);
            edgeCrossResWeight += (_qualityMetricsValues.edgeCrossAngle - temp.edgeCrossAngle);
        }

        //normalize on scale between (-2,2)
        edgeCrossingWeight = Normalize(edgeCrossingWeight);
        nodeOverlapWeight = Normalize(nodeOverlapWeight);
        edgeLenWeight = Normalize(edgeLenWeight);
        angResWeight = Normalize(angResWeight);
        edgeCrossResWeight = Normalize(edgeCrossResWeight);

        //multiply by delta factor
        edgeCrossingWeight *= delta;
        nodeOverlapWeight *= delta;
        edgeLenWeight *= delta;
        angResWeight *= delta;
        edgeCrossResWeight *= delta;

        //multiply to current values of quality metric weights
        /*edgeCrossingWeight = edgeCrossingWeight * _viewport.edgeCrossSlider.qualityFactor;
        nodeOverlapWeight = nodeOverlapWeight * _viewport.nodeOverlapSlider.qualityFactor;
        edgeLenWeight = edgeLenWeight * _viewport.edgeLengthSlider.qualityFactor;
        angResWeight = angResWeight * _viewport.angResSlider.qualityFactor;
        edgeCrossResWeight = edgeCrossResWeight * _viewport.edgeCrossAngSlider.qualityFactor;*/
        edgeCrossingWeight = (_viewport.edgeCrossSlider.qualityFactor + edgeCrossingWeight) / (1 + delta);
        nodeOverlapWeight = (_viewport.nodeOverlapSlider.qualityFactor + nodeOverlapWeight) / (1 + delta);
        edgeLenWeight = (_viewport.edgeLengthSlider.qualityFactor + edgeLenWeight) / (1 + delta);
        angResWeight = (_viewport.angResSlider.qualityFactor + angResWeight) / (1 + delta);
        edgeCrossResWeight = (_viewport.edgeCrossAngSlider.qualityFactor + edgeCrossResWeight) / (1 + delta);

        //normalize new values so their sum is 5
        sum = edgeCrossingWeight + nodeOverlapWeight + edgeLenWeight + angResWeight + edgeCrossResWeight;
        sum = 5 / sum;
        edgeCrossingWeight *= sum;
        nodeOverlapWeight *= sum;
        edgeLenWeight *= sum;
        angResWeight *= sum;
        edgeCrossResWeight *= sum;

        //check if any of the values are higher than 2 and re-normalize back
        CheckAndRenormalize();

        //update current values
        _viewport.edgeCrossSlider.qualityFactor = edgeCrossingWeight;
        _viewport.nodeOverlapSlider.qualityFactor = nodeOverlapWeight;
        _viewport.edgeLengthSlider.qualityFactor = edgeLenWeight;
        _viewport.angResSlider.qualityFactor = angResWeight;
        _viewport.edgeCrossAngSlider.qualityFactor = edgeCrossResWeight;

        //update sliders
        _viewport.edgeCrossSlider.MoveSlideFromValue(edgeCrossingWeight);
        _viewport.nodeOverlapSlider.MoveSlideFromValue(nodeOverlapWeight);
        _viewport.edgeLengthSlider.MoveSlideFromValue(edgeLenWeight);
        _viewport.angResSlider.MoveSlideFromValue(angResWeight);
        _viewport.edgeCrossAngSlider.MoveSlideFromValue(edgeCrossResWeight);

        //Update values of quality metrics from their weights in each view
        _viewport.ReadjustQualityMetricValues();
        //Create new screenshots from new clusters
        _viewport.GenerateClustersAndCreateScreenshots();
    }

    float Normalize(float value)
    {
        return ((value - min) / (max - min)) * 2;
    }

    public void menueChanged(GenericMenueComponent changedComponent)
    {
        
    }
    //All The logging is done here!
    public void AddLogDataToFile()
    {
        _buttonsController.trialNr++;
        //Do not log if it is training session
        if (_studyScript.isTraining) return;
        //Study step directory - each step create new directory where logged data and screenshots are saved
        _studyStepDirectory = _studyScript.studyTrialDirectory + "\\StudyStep" + _buttonsController.trialNr.ToString();
        _studyStepDirectory = Directory.CreateDirectory(_studyStepDirectory).FullName;

        //General data for current step
        Header header = new Header() {
            stepNumber = _buttonsController.trialNr,
            algorithm = _studyScript.algorithm,
            task = _studyScript.task,
            edgeCrWeight = _viewport.edgeCrossSlider.qualityFactor,
            nodeOvWeight = _viewport.nodeOverlapSlider.qualityFactor,
            edgeCrAngWeight = _viewport.edgeCrossAngSlider.qualityFactor,
            angResWeight = _viewport.angResSlider.qualityFactor,
            edgeLenWeight = _viewport.edgeLengthSlider.qualityFactor,
            cameraOffset = _viewport.offset
        };
        string headerStr = JsonUtility.ToJson(header);
        
        //Save general data to file
        _file = _studyStepDirectory + "\\GeneralData" + _buttonsController.trialNr.ToString() + ".json";
        File.WriteAllText(_file, headerStr);

        Option chosenOpt = new Option()
        {
            option = "chosenOption",
            overallGrade = _qualityMetricsValues.overallGrade,
            edgeCrossGrade = _qualityMetricsValues.normalizedEdgeCrossings,
            nodeOverlapGrade = _qualityMetricsValues.normalizedNodeOverlaps,
            edgeCrossAngGrade = _qualityMetricsValues.edgeCrossAngle,
            angResGrade = _qualityMetricsValues.angRes,
            edgeLengthGrade = _qualityMetricsValues.normalizedEdgeLength,
            passedTime = timePassedOnView
        };
        string chosenOptStr = JsonUtility.ToJson(chosenOpt);
        
        //Save chosen choice to file
        _file = _studyStepDirectory + "\\ChosenChoice" + _buttonsController.trialNr.ToString() + ".json";
        File.WriteAllText(_file, chosenOptStr);

        //reset passed time to 0
        timePassedOnView = 0;
        int counter = 0;
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if (transform.parent.GetChild(i) != transform)
            {
                counter++;
                //Other alternatives
                Option alternative = new Option()
                {
                    option = "alt" + counter.ToString(),
                    overallGrade = transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().overallGrade,
                    edgeCrossGrade = transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().normalizedEdgeCrossings,
                    nodeOverlapGrade = transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().normalizedNodeOverlaps,
                    edgeCrossAngGrade = transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().edgeCrossAngle,
                    angResGrade = transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().angRes,
                    edgeLengthGrade = transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().normalizedEdgeLength,
                    passedTime = transform.parent.GetChild(i).GetComponent<ReadjustQualityMetrics>().timePassedOnView
                };
                string altStr = JsonUtility.ToJson(alternative);

                //Save alternative choice to file
                _file = _studyStepDirectory + "\\Alternative" + counter.ToString() + "Step" + _buttonsController.trialNr.ToString() + ".json";
                File.WriteAllText(_file, altStr);

                //reset timePassed value
                transform.parent.GetChild(i).GetComponent<ReadjustQualityMetrics>().timePassedOnView = 0;
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

    void CheckAndRenormalize()
    {
        float maxValue = 2;
        if (edgeCrossingWeight > maxValue) maxValue = edgeCrossingWeight;
        if (nodeOverlapWeight > maxValue) maxValue = nodeOverlapWeight;
        if (edgeLenWeight > maxValue) maxValue = edgeLenWeight;
        if (angResWeight > maxValue) maxValue = angResWeight;
        if (edgeCrossResWeight > maxValue) maxValue = edgeCrossResWeight;

        edgeCrossingWeight *= 2 / maxValue;
        nodeOverlapWeight *= 2 / maxValue;
        edgeLenWeight *= 2 / maxValue;
        angResWeight *= 2 / maxValue;
        edgeCrossResWeight *= 2 / maxValue;
    }

    [System.Serializable]
    private class Header
    {
        public int stepNumber;
        public string algorithm;
        public string task;
        public float edgeCrWeight;
        public float nodeOvWeight;
        public float edgeCrAngWeight;
        public float angResWeight;
        public float edgeLenWeight;
        public int cameraOffset;
    }
    private class Option
    {
        public string option;
        public float overallGrade;
        public float edgeCrossGrade;
        public float nodeOverlapGrade;
        public float edgeCrossAngGrade;
        public float angResGrade;
        public float edgeLengthGrade;
        public float passedTime;
    }
    
}
