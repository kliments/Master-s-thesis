using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StudieScript : MonoBehaviour {
    public bool start, layout, scan, log;

    //Config files for participants
    public TextAsset[] configFiles;

    //Quality metric sliders
    public QualityMetricSlider[] sliders;

    //Game object that contains all the layout algorithms, and viewport optimizer scripts
    public GameObject controller, graphParent;

    //Participant ID
    public int ptID;
    
    //Trial number
    public int trialNR;

    // Current algorithm and task
    public string algorithm, task;

    //Participant's config file
    private TextAsset configFile;

    //buttons controller that keeps track of step
    private PreviewButtonsController _buttonsController;

    //Main directory address for current participant
    public string directory, studyTrialDirectory;

    //Row counter for config file
    private int _rowCounter = 0, _studyCounter = 0;
    private string _dataPath;
    private string[] _lines, _row;

    private GeneralLayoutAlgorithm[] algorithms;
    // Use this for initialization
    void Start () {
        configFile = configFiles[ptID];
        _lines = configFile.text.Split('\n');
        _dataPath = Application.dataPath + "/Resources/LoggedData/";
        algorithms = controller.GetComponents<GeneralLayoutAlgorithm>();
        _buttonsController = (PreviewButtonsController)FindObjectOfType(typeof(PreviewButtonsController));
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(ptID > 7)
            {
                start = false;
                return;
            }
            ResetValues();

            if (_rowCounter == 0)
            {
                CreateParticipantDirectory();
                LoadDataset(GenerateDataset());
            }
            if (_rowCounter > 3) return;

            CreateStudyTrialDirectory();
            _row = _lines[_rowCounter].Split(',');
            algorithm = _row[0];
            task = _row[1];
            task = task.Replace("\r", "");

            Invoke("LayoutGraph", 1);
            _rowCounter++;
            trialNR = _rowCounter;

            log = true;
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
    string GenerateDataset()
    {
        return GetComponent<GenerateRandomData>().GenerateData(directory,ptID.ToString());
    }

    //Dataset loader
    void LoadDataset(string path)
    {
        SaveLoadData.LoadData(path);
    }

    //Layout algorithm
    void LayoutGraph()
    {
        foreach(var alg in algorithms)
        {
            if(alg.GetType().Name == algorithm)
            {
                alg.StartAlgorithm();
                break;
            }
        }
        Invoke("ScanGraph", 1);
    }

    //Scan and compute viewpoints
    void ScanGraph()
    {
        controller.GetComponent<ViewPortOptimizer>().LocalScan();
    }

    //Each participant's directory
    void CreateParticipantDirectory()
    {
        directory = Directory.CreateDirectory(_dataPath.ToString() + "Participant" + ptID.ToString()).FullName.ToString();
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
}
