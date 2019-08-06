using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class RecalculateValues : MonoBehaviour {
    public TextAsset alt1Data, alt2Data, chosenData, overallData;
    public float delta = 1;
    public bool calculate;
    public float edgeCrossWeight, nodeOverlapWeight, edgeLengthWeight, angResWeight, edgeCrossAngleWeight, min, max;
    public float prethEdgeCrossWeight, prethNodeOverlapWeight, prethEdgeLengthWeight, prethAngResWeight, prethEdgeCrossAngleWeight;
    public string publicPath;
    public int a, b;

    private string[] chosenLines, alt1Lines, alt2Lines, overallLines;
    private string[] chosenAttr, alt1Attr, alt2Attr, overallAttr;
    private List<string[]> rowData;
	// Use this for initialization
	void Start () {
        min = -2;
        max = 2;
        rowData = new List<string[]>();
        publicPath = "Pt0St0";

    }
	
	// Update is called once per frame
	void Update () {
		if(calculate)
        {
            calculate = false;
            chosenLines = chosenData.text.Split('\n');
            alt1Lines = alt1Data.text.Split('\n');
            alt2Lines = alt2Data.text.Split('\n');
            overallLines = overallData.text.Split('\n');
            delta = 1;
            rowData = new List<string[]>();
            string[] headerData = new string[10];
            headerData[0] = "UserID";
            headerData[1] = "StepNr";
            headerData[2] = "algorithm";
            headerData[3] = "task";
            headerData[4] = "edgeCrossingWeight";
            headerData[5] = "nodeOverlapWeight";
            headerData[6] = "edgeCrossAngleWeight";
            headerData[7] = "angularResWeight";
            headerData[8] = "edgeLengthWeight";
            headerData[9] = "cameraOffset";
            rowData.Add(headerData);
            prethAngResWeight = 1;
            prethEdgeCrossAngleWeight = 1;
            prethEdgeCrossWeight = 1;
            prethEdgeLengthWeight = 1;
            prethNodeOverlapWeight = 1;
            for (int i=1; i<21; i++)
            {
                edgeCrossWeight = 0;
                nodeOverlapWeight = 0;
                edgeCrossAngleWeight = 0;
                angResWeight = 0;
                edgeLengthWeight = 0;

                chosenAttr = chosenLines[i].Split(',');
                alt1Attr = alt1Lines[i].Split(',');
                alt2Attr = alt2Lines[i].Split(',');
                if (i == 1) overallAttr = new string[10] { "1", "1", "1", "1", "1", "1", "1", "1", "1", "1" };
                else overallAttr = overallLines[i - 1].Split(',');

                //podeli gi site so prethodnite weights za da se dobie vistinskata vrednost
                for(int j=2; j<7; j++)
                {
                    alt1Attr[j] = (float.Parse(alt1Attr[j]) / float.Parse(overallAttr[j + 2])).ToString();
                    alt2Attr[j] = (float.Parse(alt2Attr[j]) / float.Parse(overallAttr[j + 2])).ToString();
                    chosenAttr[j] = (float.Parse(chosenAttr[j]) / float.Parse(overallAttr[j + 2])).ToString();
                }

                #region soberi Razliki
                edgeCrossWeight += float.Parse(chosenAttr[2]) - float.Parse(alt1Attr[2]);
                edgeCrossWeight += float.Parse(chosenAttr[2]) - float.Parse(alt2Attr[2]);

                nodeOverlapWeight += float.Parse(chosenAttr[3]) - float.Parse(alt1Attr[3]);
                nodeOverlapWeight += float.Parse(chosenAttr[3]) - float.Parse(alt2Attr[3]);

                edgeCrossAngleWeight += float.Parse(chosenAttr[4]) - float.Parse(alt1Attr[4]);
                edgeCrossAngleWeight += float.Parse(chosenAttr[4]) - float.Parse(alt2Attr[4]);

                angResWeight += float.Parse(chosenAttr[5]) - float.Parse(alt1Attr[5]);
                angResWeight += float.Parse(chosenAttr[5]) - float.Parse(alt2Attr[5]);

                edgeLengthWeight += float.Parse(chosenAttr[6]) - float.Parse(alt1Attr[6]);
                edgeLengthWeight += float.Parse(chosenAttr[6]) - float.Parse(alt2Attr[6]);

                #endregion

                #region normaliziraj od 0-2
                edgeCrossWeight = Normalize(edgeCrossWeight);
                nodeOverlapWeight = Normalize(nodeOverlapWeight);
                edgeCrossAngleWeight = Normalize(edgeCrossAngleWeight);
                angResWeight = Normalize(angResWeight);
                edgeLengthWeight = Normalize(edgeLengthWeight);
                #endregion

                #region pomnozi So Prethodni weights
                edgeCrossWeight *= prethEdgeCrossWeight;
                nodeOverlapWeight *= prethNodeOverlapWeight;
                edgeCrossAngleWeight *= prethEdgeCrossAngleWeight;
                angResWeight *= prethAngResWeight;
                edgeLengthWeight *= prethEdgeLengthWeight;
                #endregion


                edgeCrossWeight = prethEdgeCrossWeight + ((edgeCrossWeight - prethEdgeCrossWeight) * delta);
                nodeOverlapWeight = prethNodeOverlapWeight + ((nodeOverlapWeight - prethNodeOverlapWeight) * delta);
                edgeCrossAngleWeight = prethEdgeCrossAngleWeight + ((edgeCrossAngleWeight - prethEdgeCrossAngleWeight) * delta);
                angResWeight = prethAngResWeight + ((angResWeight - prethAngResWeight) * delta);
                edgeLengthWeight = prethEdgeLengthWeight + ((edgeLengthWeight - prethEdgeLengthWeight) * delta);

                float sum = edgeCrossWeight + nodeOverlapWeight + edgeLengthWeight + angResWeight + edgeCrossAngleWeight;
                sum = 5 / sum;
                edgeCrossWeight *= sum;
                nodeOverlapWeight *= sum;
                edgeLengthWeight *= sum;
                angResWeight *= sum;
                edgeCrossAngleWeight *= sum;

                prethEdgeCrossWeight = edgeCrossWeight;
                prethNodeOverlapWeight = nodeOverlapWeight;
                prethEdgeCrossAngleWeight = edgeCrossAngleWeight;
                prethAngResWeight = angResWeight;
                prethEdgeLengthWeight = edgeLengthWeight;
                delta = delta * a / b;
                headerData = new string[10];
                headerData[0] = "0";
                headerData[1] = "0";
                headerData[2] = "ConeBurst";
                headerData[3] = "Task";
                headerData[4] = edgeCrossWeight.ToString();
                headerData[5] = nodeOverlapWeight.ToString();
                headerData[6] = edgeCrossAngleWeight.ToString();
                headerData[7] = angResWeight.ToString();
                headerData[8] = edgeLengthWeight.ToString();
                headerData[9] = "0";
                rowData.Add(headerData);
            }

            string path = "C:\\Kliment\\Master's Project\\VRVis\\Assets\\Resources\\Data to recalculate\\" + publicPath + "\\GENERAL DATA" + ".csv";
            SaveDataToFile(rowData, path);
        }
	}

    float Normalize(float value)
    {
        return ((value - min) / (max - min)) * 2;
    }

    void AddLogDataToFile()
    {

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
}
