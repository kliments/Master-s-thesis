using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NumericDatamodel : MonoBehaviour {

    public string nameOfDatafile;
    public bool hasheader;
    public string delimiter;

    public float[][] myData;
    public string[] myHeaders;

    // Use this for initialization
    void Start () {
        if (nameOfDatafile.EndsWith(".csv"))
        {
            myData = DatasetImporter.readNumericCSV(nameOfDatafile, hasheader, delimiter);
            myHeaders = DatasetImporter.getHeader(nameOfDatafile, delimiter);
            return;
        }

        throw new FileLoadException("Unsupported data format of file '" + nameOfDatafile + "'. Can not load data into datamodel.");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
