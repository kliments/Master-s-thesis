using Assets.Scripts.Model;
using Model.Operators;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StudieScript : MonoBehaviour {
    public bool start, layout, scan, generateData;

    //boolean value that sets true when number of trials has exceeded
    public bool dontProceed;

    //value that determines whether is training session or logging
    public bool isTraining;

    //Config files for participants
    public TextAsset[] configFiles;

    //Quality metric sliders
    public QualityMetricSlider[] sliders;

    //Game object that contains all the layout algorithms
    public GameObject controller, graphParent, trainingText, selectedOperator;

    //Viewport optimizer
    public ViewPortOptimizer viewportOptimizer;
    //Participant ID
    public int ptID, testID;
    
    //Trial number
    public int trialNR;

    // Current algorithm and task
    public string algorithm, task;

    //Main directory address for current participant
    public string directory, studyTrialDirectory;
    public string dataPath;

    //buttons controller that keeps track of step
    public PreviewButtonsController _buttonsController;

    //Participant's config file
    private TextAsset configFile;
    
    //Multidimensional kMeans clustering algorithm
    private MultiDimensionalKMeansClustering _kMeans;

    //Row counter for config file
    private int _rowCounter = 0, _studyCounter = 0;
    private string[] _lines, _row;
    private string testData, studyData;
    private GeneralLayoutAlgorithm[] algorithms;
    private Observer observer;
    private List<GenericIcon> icons;
    // Use this for initialization
    void Start () {
        configFile = configFiles[ptID];
        _lines = configFile.text.Split('\n');
        algorithms = controller.GetComponents<GeneralLayoutAlgorithm>();
        _kMeans = (MultiDimensionalKMeansClustering)FindObjectOfType(typeof(MultiDimensionalKMeansClustering));
        viewportOptimizer = (ViewPortOptimizer)FindObjectOfType(typeof(ViewPortOptimizer));
        observer = (Observer)FindObjectOfType(typeof(Observer));
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_buttonsController.gameObject.activeSelf) _buttonsController.gameObject.SetActive(true);
            if (isTraining)
            {
                trainingText.SetActive(true);
                if (ptID > 15)
                {
                    start = false;
                    return;
                }
                ResetValues();
                _kMeans.ResetValues();
                if (_rowCounter == 0)
                {
                    CreateParticipantDirectory();
                    testData = GenerateDataset(task, isTraining);
                }
                LoadDataset(testData);
                if (_rowCounter > 3) return;

                _row = _lines[_rowCounter].Split(',');
                algorithm = _row[0];
                task = _row[1];
                task = task.Replace("\r", "");
                
                Invoke("LayoutGraph", 1);
                trialNR = _rowCounter;
                dontProceed = false;
            }
            else
            {
                trainingText.SetActive(false);
                if (ptID > 15)
                {
                    start = false;
                    return;
                }
                ResetValues();
                _kMeans.ResetValues();

                _row = _lines[_rowCounter].Split(',');
                algorithm = _row[0];
                task = _row[1];
                task = task.Replace("\r", "");

                if (_rowCounter == 0)
                {
                    CreateParticipantDirectory();
                }
                studyData = GenerateDataset(task, isTraining);
                LoadDataset(studyData);
                if (_rowCounter > 3)
                {
                    Debug.Log("STUDY HAS FINISHED!!");
                    return;
                }

                CreateStudyTrialDirectory();

                StartCoroutine(LayoutGraph(true, 1));
                _rowCounter++;
                trialNR = _rowCounter;
                dontProceed = false;
            }
        }
        if(generateData)
        {
            generateData = false;
            studyData = GetComponent<GenerateRandomData>().GenerateData(directory, testID.ToString(), task, isTraining);
            LoadDataset(studyData);
            StartCoroutine(LayoutGraph(false, 1));
        }
	}

    //Reseting values
    void ResetValues()
    {
        start = false;

        //reset weights back to 0
        foreach (var slider in sliders)
        {
            slider.qualityFactor = 1;
        }

        //reset study step
        _buttonsController.trialNr = 0;

        //reset graph rotation
        graphParent.transform.rotation = Quaternion.identity;
        graphParent.transform.localPosition = new Vector3(0, 2.5f, 0);
    }

    //Random dataset generator
    string GenerateDataset(string task, bool isTraining)
    {
        return GetComponent<GenerateRandomData>().GenerateData(directory,ptID.ToString(), task, isTraining);
    }

    //Dataset loader
    void LoadDataset(string path)
    {
        SaveLoadData.LoadData(path);
    }

    //Layout algorithm
    IEnumerator LayoutGraph(bool toScan, float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach(var alg in algorithms)
        {
            if(alg.GetType().Name == algorithm)
            {
                alg.StartAlgorithm();
                break;
            }
        }
        if(toScan) Invoke("ScanGraph", 1);
    }

    //Scan and compute viewpoints
    void ScanGraph()
    {
        viewportOptimizer.LocalScan();
        if (task == " Task1" && isTraining) SelectRandomlyTwoNodes();
        else if (task == " Task2" && isTraining) SelectAllOperators(typeof(DataloaderOperator));
    }

    //Each participant's directory
    void CreateParticipantDirectory()
    {
        directory = Directory.CreateDirectory(dataPath.ToString() + "Participant" + ptID.ToString()).FullName.ToString();
    }

    //Each trial's directory - in case of crash of the application, generate new directory
    private void CreateStudyTrialDirectory()
    {
        studyTrialDirectory = directory + "\\Study" + _studyCounter.ToString();
        //Directory for study trial (in case of crash of app, create new directory)
        while (Directory.Exists(studyTrialDirectory))
        {
            _studyCounter++;
            studyTrialDirectory = directory + "\\Study" + _studyCounter.ToString();
        }
        studyTrialDirectory = Directory.CreateDirectory(studyTrialDirectory).FullName;
    }
    public void ToggleTrainingSession()
    {
        if (isTraining)
        {
            isTraining = false;
        }
        else
        {
            if(_rowCounter % 2 == 0)
            {
                isTraining = true;
            }
        }
    }

    public void SelectRandomlyTwoNodes()
    {
        int random1, random2;
        IconProperties rn1, rn2;
        GameObject icon = new GameObject();
        bool toBreak = false;
        icons = new List<GenericIcon>();
        foreach (var op in observer.GetOperators())
        {
            icons.Add(op.GetIcon());
        }
        while(true)
        {
            toBreak = true;
            random1 = Random.Range(0, icons.Count);
            random2 = random1;
            rn1 = icons[random1].GetComponent<IconProperties>();
            rn2 = icons[random2].GetComponent<IconProperties>();
            while (random1 == random2)
            {
                random2 = Random.Range(0, icons.Count);
                rn2 = icons[random2].GetComponent<IconProperties>();
            }
            for(int i=0; i<3; i++)
            {
                //Raycast from each chosen viewpoint, to each randomly selected node to highlight
                //Choose only if visibility is higher than 50%
                foreach(Transform child in rn1.transform)
                {
                    if(child.gameObject.activeSelf)
                    {
                        icon = child.gameObject;
                        break;
                    }
                }
                if (NodeOverlappingCoef(icon.gameObject, viewportOptimizer.chosenViewpoints[i].cameraPosition) > 0.2f) toBreak = false;
            }
            for (int i = 0; i < 3; i++)
            {
                //Raycast from each chosen viewpoint, to each randomly selected node to highlight
                //Choose only if visibility is higher than 50%
                foreach (Transform child in rn2.transform)
                {
                    if (child.gameObject.activeSelf)
                    {
                        icon = child.gameObject;
                        break;
                    }
                }
                if (NodeOverlappingCoef(icon.gameObject, viewportOptimizer.chosenViewpoints[i].cameraPosition) > 0.2f) toBreak = false;
            }
            //check if both icons are visible more than 50%
            if (toBreak) break;

        }
        rn1.GetComponent<GenericIcon>().SelectThisIcon();
        rn2.GetComponent<GenericIcon>().SelectThisIcon();
    }

    public void SelectAllOperators(System.Type go)
    {
        foreach(var op in observer.GetOperators())
        {
            if(op.GetType() == go)
            {
                //Highlight the icon of the operator
                HighlightOperator(op);
            }
        }
    }

    void HighlightOperator(GenericOperator op)
    {
        Transform icon = op.GetIcon().transform;
        Transform child;
        if (icon.GetChild(0).name != "Plane") child = icon.GetChild(0);
        else child = icon.GetChild(1);
        GameObject selectedOp = Instantiate(selectedOperator, icon.position, new Quaternion(0,0,0,1), child);
        selectedOp.transform.localScale = new Vector3(1, 1, 100);
        selectedOp.tag = "SelectedIcon";
    }
    float NodeOverlappingCoef(GameObject currentIcon, Vector3 cameraPos)
    {
        int layerMask = LayerMask.GetMask("NodeOverlapping");
        float count = 0;
        Vector3 startXY, endX, endY, tempX, tempY, currentPoint, dir;
        RaycastHit hit;
        /*
         * Start and end points for interpolation between them to get the points to raycast
         */
        startXY = currentIcon.transform.TransformPoint(currentIcon.GetComponent<MeshFilter>().mesh.vertices[0]);
        endX = currentIcon.transform.TransformPoint(currentIcon.GetComponent<MeshFilter>().mesh.vertices[1]);
        endY = currentIcon.transform.TransformPoint(currentIcon.GetComponent<MeshFilter>().mesh.vertices[2]);
        for (int i = 0; i <= 4; i++)
        {
            tempX = Vector3.Lerp(startXY, endY, (float)i / 4);
            tempY = Vector3.Lerp(endX, currentIcon.transform.TransformPoint(currentIcon.GetComponent<MeshFilter>().mesh.vertices[3]), (float)i / 4);
            for (int j = 0; j <= 4; j++)
            {
                currentPoint = Vector3.Lerp(tempX, tempY, (float)j / 4);
                dir = currentPoint - cameraPos;
                if (Physics.Raycast(cameraPos, dir, out hit, 100, layerMask))
                {
                    if (hit.transform.gameObject != currentIcon)
                    {
                        count++;
                    }
                }
            }
        }
        return count /= 25;
    }
}
