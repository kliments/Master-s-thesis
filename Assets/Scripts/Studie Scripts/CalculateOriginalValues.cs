using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class CalculateOriginalValues : MonoBehaviour {
    public TextAsset alt1Data, alt2Data, chosenData, overallData, newOverallData;
    public string publicPath;
    public int participant, study;
    private string[] chosenLines, alt1Lines, alt2Lines, overallLines, newOverallLines;
    private string[] chosenAttr, alt1Attr, alt2Attr, overallAttr, newOverallAttr;
    private List<string[]> chosenRowData, alt1RowData, alt2RowData;

    public bool CALCULATE;
    // Use this for initialization
    void Start ()
    {
        participant = 0;
        study = 0;
    }
	
	// Update is called once per frame
	void Update () {
		if(CALCULATE)
        {
            CALCULATE = false;
            chosenLines = chosenData.text.Split('\n');
            alt1Lines = alt1Data.text.Split('\n');
            alt2Lines = alt2Data.text.Split('\n');
            overallLines = overallData.text.Split('\n');
            newOverallLines = newOverallData.text.Split('\n');

            chosenRowData = new List<string[]>();
            alt1RowData = new List<string[]>();
            alt2RowData = new List<string[]>();

            string[] headerData = new string[8];
            headerData[0] = "step";
            headerData[1] = "OverallGrade";
            headerData[2] = "edgeCrossValue";
            headerData[3] = "nodeOverlapValue";
            headerData[4] = "edgeCrossAngleValue";
            headerData[5] = "angularResValue";
            headerData[6] = "edgeLengthValue";
            headerData[7] = "passedTime";
            chosenRowData.Add(headerData);
            alt1RowData.Add(headerData);
            alt2RowData.Add(headerData);

            for (int i = 1; i < 21; i++)
            {
                chosenAttr = chosenLines[i].Split(',');
                alt1Attr = alt1Lines[i].Split(',');
                alt2Attr = alt2Lines[i].Split(',');
                if (i == 1)
                {
                    overallAttr = new string[10] { "1", "1", "1", "1", "1", "1", "1", "1", "1", "1" };
                    newOverallAttr = new string[10] { "1", "1", "1", "1", "1", "1", "1", "1", "1", "1" };
                }
                else
                {
                    overallAttr = overallLines[i - 1].Split(',');
                    newOverallAttr = newOverallLines[i - 1].Split(',');
                }

                //podeli gi site so prethodnite weights za da se dobie vistinskata vrednost
                for (int j = 2; j < 7; j++)
                {
                    alt1Attr[j] = ((float.Parse(alt1Attr[j]) / float.Parse(overallAttr[j + 2])) * float.Parse(newOverallAttr[j-2])).ToString();
                    alt2Attr[j] = ((float.Parse(alt2Attr[j]) / float.Parse(overallAttr[j + 2])) * float.Parse(newOverallAttr[j-2])).ToString();
                    chosenAttr[j] = ((float.Parse(chosenAttr[j]) / float.Parse(overallAttr[j + 2])) * float.Parse(newOverallAttr[j-2])).ToString();
                }
                headerData = new string[8];
                headerData[0] = (i-1).ToString();
                headerData[1] = ((float.Parse(chosenAttr[2]) + float.Parse(chosenAttr[3]) + float.Parse(chosenAttr[4]) + float.Parse(chosenAttr[5]) + float.Parse(chosenAttr[6]))/5).ToString();
                headerData[2] = chosenAttr[2];
                headerData[3] = chosenAttr[3];
                headerData[4] = chosenAttr[4];
                headerData[5] = chosenAttr[5];
                headerData[6] = chosenAttr[6];
                headerData[7] = chosenAttr[7];
                chosenRowData.Add(headerData);

                headerData = new string[8];
                headerData[0] = (i - 1).ToString();
                headerData[1] = ((float.Parse(alt1Attr[2]) + float.Parse(alt1Attr[3]) + float.Parse(alt1Attr[4]) + float.Parse(alt1Attr[5]) + float.Parse(alt1Attr[6])) / 5).ToString();
                headerData[2] = alt1Attr[2];
                headerData[3] = alt1Attr[3];
                headerData[4] = alt1Attr[4];
                headerData[5] = alt1Attr[5];
                headerData[6] = alt1Attr[6];
                headerData[7] = alt1Attr[7];
                alt1RowData.Add(headerData);

                headerData = new string[8];
                headerData[0] = (i - 1).ToString();
                headerData[1] = ((float.Parse(alt2Attr[2]) + float.Parse(alt2Attr[3]) + float.Parse(alt2Attr[4]) + float.Parse(alt2Attr[5]) + float.Parse(alt2Attr[6])) / 5).ToString();
                headerData[2] = alt2Attr[2];
                headerData[3] = alt2Attr[3];
                headerData[4] = alt2Attr[4];
                headerData[5] = alt2Attr[5];
                headerData[6] = alt2Attr[6];
                headerData[7] = alt2Attr[7];
                alt2RowData.Add(headerData);
            }

            publicPath = "Pt" + participant + "St" + study;
            string path = "C:\\Kliment\\Master's Project\\VRVis\\Assets\\Resources\\Data to recalculate\\" + publicPath + "\\OriginalChosenOption" + ".csv";
            SaveDataToFile(chosenRowData, path);

            path = "C:\\Kliment\\Master's Project\\VRVis\\Assets\\Resources\\Data to recalculate\\" + publicPath + "\\OriginalAlt1Option" + ".csv";
            SaveDataToFile(alt1RowData, path);

            path = "C:\\Kliment\\Master's Project\\VRVis\\Assets\\Resources\\Data to recalculate\\" + publicPath + "\\OriginalAlt2Option" + ".csv";
            SaveDataToFile(alt2RowData, path);

            study++;
            if (study == 4)
            {
                participant++;
                study = 0;
            }
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

        sb.Replace("\r", "");
        StreamWriter outStream = File.CreateText(path);
        outStream.WriteLine(sb);
        outStream.Close();
    }

    private class Header
    {
        public int step;
        public float overallGrade;
        public float edgeCr;
        public float nodeOv;
        public float edgeCrAng;
        public float angRes;
        public float edgeLen;
        public float passedTime;
    }
}
