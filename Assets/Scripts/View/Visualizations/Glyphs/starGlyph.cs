using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class starGlyph : abstractGlyph
{

    public LineRenderer linePrefab;

    // Use this for initialization
    void Start () {
        //setValues(new float[5] { 0.2f, 1.0f, 0.75f, 0.5f, 0.5f});
    }

    private int getNrOfUndisabledValues(float[] actualValues)
    {
        int count = 0;
        for(int i = 0; i < actualValues.Length; i++)
        {
            if(actualValues[i] != -1)
            {
                count++;
            }
        }
        return count;
    }


    public override void setValues(float[] actualValues)
    {
        Vector3 center = new Vector3(0,0,0);
        float[] Values = new float[getNrOfUndisabledValues(actualValues)];
        int pos = 0;
        for(int i = 0; i < actualValues.Length; i++)
        {
            if(actualValues[i] != -1)
            {
                Values[pos] = actualValues[i];
                pos++;
            }
        }

        float rotSteps = 360.0f / Values.Length;
        float lineWidth = 0.004f;

        for (int i = 0; i < Values.Length; i++)
        {
            LineRenderer currentLine = Instantiate(linePrefab);
            currentLine.transform.parent = this.transform;
            currentLine.transform.localPosition = new Vector3(0,0,0);
            currentLine.transform.localScale = new Vector3(1,1,1);
            currentLine.transform.eulerAngles = new Vector3(0, 0, 0);
            currentLine.startWidth = lineWidth;
            currentLine.endWidth = lineWidth;
            Vector3 zeroRotationPoint = new Vector3(0, Values[i] * 0.5f, 0);
            Vector3 rot = new Vector3(0.0f, 0.0f, i * rotSteps);
            Vector3 linePos = RotatePointAroundPivot(zeroRotationPoint, center, rot);
            currentLine.SetPositions(new Vector3[] { center, linePos });
        }
        

        
        
    }

    //https://answers.unity.com/questions/532297/rotate-a-vector-around-a-certain-point.html
    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
