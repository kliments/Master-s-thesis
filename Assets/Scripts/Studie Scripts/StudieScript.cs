using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudieScript : MonoBehaviour {
    public bool start, layout, scan;

    //Config files for participants
    public TextAsset[] configFiles;

    //Game object that contains all the layout algorithms, and viewport optimizer scripts
    public GameObject controller;

    //Participant ID
    public int ptID;

    //Participant's config file
    private TextAsset configFile;

    //Row counter for config file
    private int rowCounter = 0;
    private string algorithm, task;
    private string[] lines, row;

    private GeneralLayoutAlgorithm[] algorithms;
    // Use this for initialization
    void Start () {
        configFile = configFiles[ptID];
        lines = configFile.text.Split('\n');
        algorithms = controller.GetComponents<GeneralLayoutAlgorithm>();
    }
	
	// Update is called once per frame
	void Update () {
        if(scan)
        {
            scan = false;
            ScanGraph();
            rowCounter++;
        }
        if(layout)
        {
            layout = false;
            LayoutGraph(algorithm);
            scan = true;
        }
        if (start)
        {
            start = false;
            if (rowCounter == 0)
            {
                LoadDataset(GenerateDataset());
            }
            row = lines[rowCounter].Split(',');
            algorithm = row[0];
            task = row[1];
            task = task.Replace("\r", "");
            layout = true;
        }
	}

    //Random dataset generator
    string GenerateDataset()
    {
        return GetComponent<GenerateRandomData>().GenerateData(ptID.ToString());
    }

    //Dataset loader
    void LoadDataset(string path)
    {
        SaveLoadData.LoadData(path);
    }

    //Layout algorithm
    void LayoutGraph(string algorithm)
    {
        foreach(var alg in algorithms)
        {
            if(alg.GetType().Name == algorithm)
            {
                alg.StartAlgorithm();
                break;
            }
        }
    }

    //Scan and compute viewpoints
    void ScanGraph()
    {
        controller.GetComponent<ViewPortOptimizer>().LocalScan();
    }
}
