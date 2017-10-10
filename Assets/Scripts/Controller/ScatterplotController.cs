using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterplotController : MonoBehaviour {

    public NumericDatamodel myDataModel;
    public GameObject datapoint_prefab;

	// Use this for initialization
	void Start () {
        createDatapoints();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void createDatapoints()
    {
        float[][] myData = myDataModel.getNormalizedData(false, false, false);  // Gets normalized data. Filtered Data is not removed (first false), normalization is done without the use of filtered rows (second false) and normalization is done per column and not globaly (third false).
        for(int i = 0; i < myData.Length; i++)
        {
            GameObject datapoint = Instantiate(datapoint_prefab);
            datapoint.transform.parent = this.transform;
            datapoint.transform.localPosition = new Vector3(myData[i][0], myData[i][1], myData[i][2]);
        }
    }
}
