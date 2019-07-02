using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ConvertJsonToCsv : MonoBehaviour {
    public List<TextAsset> jsonFiles;
    public bool load, save;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(load)
        {
            load = false;
            List<string[]> allData = new List<string[]>();
            foreach(var file in jsonFiles)
            {
                string text = file.ToString();
                Option option = JsonUtility.FromJson<Option>(text);
                string[] optionData = new string[8];
                optionData[0] = option.option;
                optionData[1] = option.overallGrade.ToString();
                optionData[2] = option.edgeCrossGrade.ToString();
                optionData[3] = option.nodeOverlapGrade.ToString();
                optionData[4] = option.edgeCrossAngGrade.ToString();
                optionData[5] = option.angResGrade.ToString();
                optionData[6] = option.edgeLengthGrade.ToString();
                optionData[7] = option.passedTime.ToString();
                allData.Add(optionData);
            }
            SaveDataToFile(allData, "C://Kliment//Master's Project//VRVis//LoggedData//combined.csv");

        }
        if(save)
        {
            save = false;
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

    [Serializable]
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
