using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        //normalize on scale between (0,2)
        edgeCrossingWeight = Normalize(edgeCrossingWeight);
        nodeOverlapWeight = Normalize(nodeOverlapWeight);
        edgeLenWeight = Normalize(edgeLenWeight);
        angResWeight = Normalize(angResWeight);
        edgeCrossResWeight = Normalize(edgeCrossResWeight);
        
        //multiply to current values of quality metric weights
        edgeCrossingWeight *= _viewport.edgeCrossSlider.qualityFactor;
        nodeOverlapWeight *= _viewport.nodeOverlapSlider.qualityFactor;
        edgeLenWeight *= _viewport.edgeLengthSlider.qualityFactor;
        angResWeight *= _viewport.angResSlider.qualityFactor;
        edgeCrossResWeight *= _viewport.edgeCrossAngSlider.qualityFactor;
        
        //renormalize new values so their sum is 5
        sum = edgeCrossingWeight + nodeOverlapWeight + edgeLenWeight + angResWeight + edgeCrossResWeight;
        sum = 5 / sum;
        edgeCrossingWeight *= sum;
        nodeOverlapWeight *= sum;
        edgeLenWeight *= sum;
        angResWeight *= sum;
        edgeCrossResWeight *= sum;

        //check if the difference with the previous values is more than 0.05 and normalize
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
        string allData = "";
        _buttonsController.trialNr++;
        //Do not log if it is training session
        if (_studyScript.isTraining) return;
        //Study step directory - each step create new directory where logged data and screenshots are saved
        _studyStepDirectory = _studyScript.studyTrialDirectory + "\\StudyStep" + _buttonsController.trialNr.ToString();
        _studyStepDirectory = Directory.CreateDirectory(_studyStepDirectory).FullName;

        // Save screenshots
        for (int i=0; i<transform.parent.childCount; i++)
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
        //adds header data to list
        string[] headerData = new string[10];
        headerData[0] = _studyScript.ptID.ToString();
        headerData[1] = _buttonsController.trialNr.ToString();
        headerData[2] = _studyScript.algorithm;
        headerData[3] = _studyScript.task;
        headerData[4] = _viewport.edgeCrossSlider.qualityFactor.ToString();
        headerData[5] = _viewport.nodeOverlapSlider.qualityFactor.ToString();
        headerData[6] = _viewport.edgeCrossAngSlider.qualityFactor.ToString();
        headerData[7] = _viewport.angResSlider.qualityFactor.ToString();
        headerData[8] = _viewport.edgeLengthSlider.qualityFactor.ToString();
        headerData[9] = _viewport.offset.ToString();
        _buttonsController.rowHeaderData.Add(headerData);

        //adds chosen option data to list
        string[] optionData = new string[8];
        optionData[0] = "chosenOption";
        optionData[1] = _qualityMetricsValues.overallGrade.ToString();
        optionData[2] = _qualityMetricsValues.normalizedEdgeCrossings.ToString();
        optionData[3] = _qualityMetricsValues.normalizedNodeOverlaps.ToString();
        optionData[4] = _qualityMetricsValues.edgeCrossAngle.ToString();
        optionData[5] = _qualityMetricsValues.angRes.ToString();
        optionData[6] = _qualityMetricsValues.normalizedEdgeLength.ToString();
        optionData[7] = timePassedOnView.ToString();
        _buttonsController.rowChosenViewData.Add(optionData);

        timePassedOnView = 0;

        int counter = 0;
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if (transform.parent.GetChild(i) != transform)
            {
                counter++;
                if (counter == 1)
                {
                    optionData = new string[8];
                    optionData[0] = "alt" + counter.ToString();
                    optionData[1] = transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().overallGrade.ToString();
                    optionData[2] = transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().normalizedEdgeCrossings.ToString();
                    optionData[3] = transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().normalizedNodeOverlaps.ToString();
                    optionData[4] = transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().edgeCrossAngle.ToString();
                    optionData[5] = transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().angRes.ToString();
                    optionData[6] = transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().normalizedEdgeLength.ToString();
                    optionData[7] = transform.parent.GetChild(i).GetComponent<ReadjustQualityMetrics>().timePassedOnView.ToString();
                    _buttonsController.rowAlt1Data.Add(optionData);
                    transform.parent.GetChild(i).GetComponent<ReadjustQualityMetrics>().timePassedOnView = 0;
                }
                else
                {
                    optionData = new string[8];
                    optionData[0] = "alt" + counter.ToString();
                    optionData[1] = transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().overallGrade.ToString();
                    optionData[2] = transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().normalizedEdgeCrossings.ToString();
                    optionData[3] = transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().normalizedNodeOverlaps.ToString();
                    optionData[4] = transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().edgeCrossAngle.ToString();
                    optionData[5] = transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().angRes.ToString();
                    optionData[6] = transform.parent.GetChild(i).GetComponent<QualityMetricViewPort>().normalizedEdgeLength.ToString();
                    optionData[7] = transform.parent.GetChild(i).GetComponent<ReadjustQualityMetrics>().timePassedOnView.ToString();
                    _buttonsController.rowAlt2Data.Add(optionData);

                    transform.parent.GetChild(i).GetComponent<ReadjustQualityMetrics>().timePassedOnView = 0;
                }
            }
        }
        //save header data to file/or overwrite if file exists
        string path = _studyStepDirectory + "\\GeneralData" + _buttonsController.trialNr.ToString() + ".csv";
        SaveDataToFile(_buttonsController.rowHeaderData, path);
        //save chosen data to file/or overwrite if file exists
        path = _studyStepDirectory + "\\ChosenOptionData" + _buttonsController.trialNr.ToString() + ".csv";
        SaveDataToFile(_buttonsController.rowChosenViewData, path);
        //save alternative 1 data to file/or overwrite if file exists
        path = _studyStepDirectory + "\\AlternativeOptionData1" + _buttonsController.trialNr.ToString() + ".csv";
        SaveDataToFile(_buttonsController.rowAlt1Data, path);
        //save alternative 2 data to file/or overwrite if file exists
        path = _studyStepDirectory + "\\AlternativeOptionData2" + _buttonsController.trialNr.ToString() + ".csv";
        SaveDataToFile(_buttonsController.rowAlt2Data,path);
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
        float maxValue = 0;
        float minValue = 1;
        float dif1 = edgeCrossingWeight - _viewport.edgeCrossSlider.qualityFactor;
        float dif2 = nodeOverlapWeight - _viewport.nodeOverlapSlider.qualityFactor;
        float dif3 = edgeLenWeight - _viewport.edgeLengthSlider.qualityFactor;
        float dif4 = angResWeight - _viewport.angResSlider.qualityFactor;
        float dif5 = edgeCrossResWeight - _viewport.edgeCrossAngSlider.qualityFactor;
        if (dif1> 0.05f || dif2 > 0.05f || dif3 > 0.05f || dif4 > 0.05f || dif5 > 0.05f ||
            dif1 < 0.05f || dif2 < 0.05f || dif3 < 0.05f || dif4 < 0.05f || dif5 < 0.05f)
        {
            if (maxValue < dif1) maxValue = dif1;
            if (maxValue < dif2) maxValue = dif2;
            if (maxValue < dif3) maxValue = dif3;
            if (maxValue < dif4) maxValue = dif4;
            if (maxValue < dif5) maxValue = dif5;

            if (minValue > dif1) minValue = dif1;
            if (minValue > dif2) minValue = dif2;
            if (minValue > dif3) minValue = dif3;
            if (minValue > dif4) minValue = dif4;
            if (minValue > dif5) minValue = dif5;

            //normalize on scale between 0-0.05
            edgeCrossingWeight = _viewport.edgeCrossSlider.qualityFactor + ((0.1f) * ((dif1 - minValue) / (maxValue - minValue)) - 0.05f);
            nodeOverlapWeight = _viewport.nodeOverlapSlider.qualityFactor + ((0.1f) * ((dif2 - minValue) / (maxValue - minValue)) - 0.05f);
            edgeLenWeight = _viewport.edgeLengthSlider.qualityFactor + ((0.1f) * ((dif3 - minValue) / (maxValue - minValue)) - 0.05f);
            angResWeight = _viewport.angResSlider.qualityFactor + ((0.1f) * ((dif4 - minValue) / (maxValue - minValue)) - 0.05f);
            edgeCrossResWeight = _viewport.edgeCrossAngSlider.qualityFactor + ((0.1f) * ((dif5 - minValue) / (maxValue - minValue)) - 0.05f);
            
            //multiply by delta factor
            edgeCrossingWeight *= delta;
            nodeOverlapWeight *= delta;
            edgeLenWeight *= delta;
            angResWeight *= delta;
            edgeCrossResWeight *= delta;

            //renormalize new values so their sum is 5
            sum = edgeCrossingWeight + nodeOverlapWeight + edgeLenWeight + angResWeight + edgeCrossResWeight;
            sum = 5 / sum;
            edgeCrossingWeight *= sum;
            nodeOverlapWeight *= sum;
            edgeLenWeight *= sum;
            angResWeight *= sum;
            edgeCrossResWeight *= sum;
            
        }
    }

    void SaveDataToFile(List<string[]> rowData, string path)
    {
        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));

        StreamWriter outStream = File.CreateText(path);
        outStream.WriteLine(sb);
        outStream.Close();
    }

    [System.Serializable]
    class AllData
    {
        public Header headerData;
        public Option chosen;
        public Option alt1;
        public Option alt2;
    }
    private class Header
    {
        public int userID;
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
