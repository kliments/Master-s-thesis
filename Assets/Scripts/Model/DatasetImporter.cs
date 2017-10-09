using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class DatasetImporter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static string[][] readCSV(string nameOfDatafile, bool hasheader, string delimiter)
    {
        // TODO implement header functionality
        string pathToData = createFilePath(nameOfDatafile);
        if (File.Exists(pathToData))
        {
            int start = 0;
            if (hasheader) { start++; };

            string[] fileContent = System.IO.File.ReadAllLines(pathToData);
            string[][] fileContentSplit = new string[fileContent.Length - start][];
            int amountOfCols = fileContent[0].Split(delimiter.ToCharArray()).Length;
            
            for (int i = start; i < fileContent.Length; i++)
            {
                fileContentSplit[i - start] = trimStringArray(fileContent[i].Split(delimiter.ToCharArray()));
                if (fileContentSplit[i - start].Length != amountOfCols) { throw new FileLoadException("Can not load " + pathToData + ". Row " + i + " does not contain the same amount of columns than the first row(" + amountOfCols + ")."); };
            }
            return fileContentSplit;
        }
        else
        {
            throw new FileLoadException("Did not find file '" + pathToData + "'.");
        }
    }

    private static string[] trimStringArray(string[] toTrim)
    {
        for (int i = 0; i < toTrim.Length; i++)
        {
            toTrim[i] = toTrim[i].Trim();
        }
        return toTrim;
    }

    public static float[][] readNumericCSV(string nameOfDatafile, bool hasheader, string delimiter)
    {
        string[][] tmp = readCSV(nameOfDatafile, hasheader, delimiter);
        return makeNumeric(tmp, nameOfDatafile);
    }

    public static string[] getHeader(string nameOfDatafile, string delimiter)
    {
        string pathToData = createFilePath(nameOfDatafile);
        if (File.Exists(pathToData))
        {
            StreamReader reader = new StreamReader(pathToData);
            string fileContent = reader.ReadLine();
            reader.Close();
            string[] fileContentSplit = fileContent.Split(delimiter.ToCharArray());
            return trimStringArray(fileContentSplit);
        }
        else
        {
            throw new FileLoadException("Did not find file '" + pathToData + "'.");
        }
    }

    private static string createFilePath(string nameOfDatafile)
    {
        return Application.streamingAssetsPath + "/Datasets/" + nameOfDatafile; ;
    }

    private static float[][] makeNumeric(string[][] toCheck, string dataFileName)
    {
        float[][] numericData = new float[toCheck.Length][];
        for (int i = 0; i < toCheck.Length; i++)
        {
            numericData[i] = new float[toCheck[i].Length];
            for (int j = 0; j < toCheck[i].Length; j++)
            {
                try
                {
                    numericData[i][j] = float.Parse(toCheck[i][j]);
                }
                catch (System.Exception e)
                {
                    throw new FileLoadException("Found non numeric value in datafile '" + dataFileName + "'. Invalid row has index " + i + ". Invalid content is: '" + toCheck[i][j] + "'." + System.Environment.NewLine + "Original Exception: " + System.Environment.NewLine + e);
                }
            }
        }
        return numericData;
    }
}
