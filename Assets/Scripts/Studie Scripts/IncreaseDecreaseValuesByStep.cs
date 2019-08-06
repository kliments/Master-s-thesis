using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class IncreaseDecreaseValuesByStep : MonoBehaviour {
    public TextAsset alt1Data, alt2Data, chosenData;
    public string publicPath;
    public bool calculate;
    public int participant, study;
    public float edgeCrossingWeight, nodeOverlapWeight, edgeCrossingAngleWeight, angularResolutionWeight, edgeLengthWeight;
    private string[] chosenLines, alt1Lines, alt2Lines;
    private string[] chosenAttr, alt1Attr, alt2Attr;
    private List<string[]> chosenRawData, chosenNormalizedData;
    // Use this for initialization
    void Start () {
        participant = 0;
        study = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (calculate)
        {
            calculate = false;
            chosenLines = chosenData.text.Split('\n');
            alt1Lines = alt1Data.text.Split('\n');
            alt2Lines = alt2Data.text.Split('\n');

            chosenRawData = new List<string[]>();
            chosenNormalizedData = new List<string[]>();

            string[] chosenAdjustedData = new string[5];
            chosenAdjustedData[0] = "edge crossing weight";
            chosenAdjustedData[1] = "node overlap weight";
            chosenAdjustedData[2] = "edge crossing angle weight";
            chosenAdjustedData[3] = "angular resolution weight";
            chosenAdjustedData[4] = "edge length weight";

            chosenRawData.Add(chosenAdjustedData);
            chosenNormalizedData.Add(chosenAdjustedData);

            edgeCrossingWeight = 1;
            nodeOverlapWeight = 1;
            edgeCrossingAngleWeight = 1;
            angularResolutionWeight = 1;
            edgeLengthWeight = 1;

            float sum = 0;

            for (int i = 1; i < 21; i++)
            {
                chosenAttr = chosenLines[i].Split(',');
                alt1Attr = alt1Lines[i].Split(',');
                alt2Attr = alt2Lines[i].Split(',');

                if (float.Parse(chosenAttr[2]) > float.Parse(alt1Attr[2]) && float.Parse(chosenAttr[2]) > float.Parse(alt2Attr[2])) edgeCrossingWeight += 0.05f;
                else if (float.Parse(chosenAttr[2]) < float.Parse(alt1Attr[2]) && float.Parse(chosenAttr[2]) < float.Parse(alt2Attr[2])) edgeCrossingWeight -= 0.05f;

                if (float.Parse(chosenAttr[3]) > float.Parse(alt1Attr[3]) && float.Parse(chosenAttr[3]) > float.Parse(alt2Attr[3])) nodeOverlapWeight += 0.05f;
                else if (float.Parse(chosenAttr[3]) < float.Parse(alt1Attr[3]) && float.Parse(chosenAttr[3]) < float.Parse(alt2Attr[3])) nodeOverlapWeight -= 0.05f;

                if (float.Parse(chosenAttr[4]) > float.Parse(alt1Attr[4]) && float.Parse(chosenAttr[4]) > float.Parse(alt2Attr[4])) edgeCrossingAngleWeight += 0.05f;
                else if (float.Parse(chosenAttr[4]) < float.Parse(alt1Attr[4]) && float.Parse(chosenAttr[4]) < float.Parse(alt2Attr[4])) edgeCrossingAngleWeight -= 0.05f;

                if (float.Parse(chosenAttr[5]) > float.Parse(alt1Attr[5]) && float.Parse(chosenAttr[5]) > float.Parse(alt2Attr[5])) angularResolutionWeight += 0.05f;
                else if (float.Parse(chosenAttr[5]) < float.Parse(alt1Attr[5]) && float.Parse(chosenAttr[5]) < float.Parse(alt2Attr[5])) angularResolutionWeight -= 0.05f;

                if (float.Parse(chosenAttr[6]) > float.Parse(alt1Attr[6]) && float.Parse(chosenAttr[6]) > float.Parse(alt2Attr[6])) edgeLengthWeight += 0.05f;
                else if (float.Parse(chosenAttr[6]) < float.Parse(alt1Attr[6]) && float.Parse(chosenAttr[6]) < float.Parse(alt2Attr[6])) edgeLengthWeight -= 0.05f;

                sum = float.Parse(chosenAttr[2]) + float.Parse(chosenAttr[3]) + float.Parse(chosenAttr[4]) + float.Parse(chosenAttr[5]) + float.Parse(chosenAttr[6]);
                sum /= 5;

                chosenAdjustedData = new string[5];
                chosenAdjustedData[0] = edgeCrossingWeight.ToString();
                chosenAdjustedData[1] = nodeOverlapWeight.ToString();
                chosenAdjustedData[2] = edgeCrossingAngleWeight.ToString();
                chosenAdjustedData[3] = angularResolutionWeight.ToString();
                chosenAdjustedData[4] = edgeLengthWeight.ToString();
                chosenRawData.Add(chosenAdjustedData);
                
                chosenAdjustedData = new string[5];
                chosenAdjustedData[0] = (edgeCrossingWeight * sum).ToString();
                chosenAdjustedData[1] = (nodeOverlapWeight * sum).ToString();
                chosenAdjustedData[2] = (edgeCrossingAngleWeight * sum).ToString();
                chosenAdjustedData[3] = (angularResolutionWeight * sum).ToString();
                chosenAdjustedData[4] = (edgeLengthWeight * sum).ToString();
                chosenNormalizedData.Add(chosenAdjustedData);
            }
            publicPath = "Pt" + participant + "St" + study;
            string path = "C:\\Kliment\\Master's Project\\VRVis\\Assets\\Resources\\Data to recalculate\\" + publicPath + "\\RawIncreaseOrDecrease" + ".csv";
            SaveDataToFile(chosenRawData, path);
            path = "C:\\Kliment\\Master's Project\\VRVis\\Assets\\Resources\\Data to recalculate\\" + publicPath + "\\NormalizedIncreaseOrDecrease" + ".csv";
            SaveDataToFile(chosenNormalizedData, path);
            study++;
            if(study==4)
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

    private class Values
    {
        public float edgeCrWeight;
        public float nodeOvWeight;
        public float edgeCrAngWeight;
        public float angResWeight;
        public float edgeLenWeight;
    }
}
