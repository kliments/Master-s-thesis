using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGlyph : abstractGlyph
{
    public LineRenderer myLine;

    public override void setValues(float[] Values)
    {
        
        
        float mainRotateMin = 0.0f;
        float mainRotateMax = 180.0f;

        Vector3 lineNoRotationPositionTop = new Vector3(0f, 0.5f, 0f);
        Vector3 lineNoRotationPositionBottom = new Vector3(0f, -0.5f, 0f);

        Vector3 lineRotation = new Vector3(
            mainRotateMin + (getFloatAtPos(Values, 5) * (mainRotateMax - mainRotateMin)),
            0.0f,   
            mainRotateMin + (getFloatAtPos(Values, 0) * (mainRotateMax - mainRotateMin)));// mainRotateMin + (getFloatAtPos(Values, 5) * (mainRotateMax - mainRotateMin)));
        

        Vector3 topPos = RotatePointAroundPivot(lineNoRotationPositionTop, new Vector3(0,0,0), lineRotation);
        Vector3 bottomPos = RotatePointAroundPivot(lineNoRotationPositionBottom, new Vector3(0, 0, 0), lineRotation);

        //float currentRotation = mainRotateMin + getFloatAtPos(Values, 0) * (mainRotateMax - mainRotateMin) - 90;
        //float currentSecondaryRotation = mainRotateMin + getFloatAtPos(Values, 5) * (mainRotateMax - mainRotateMin) - 90;

        this.transform.Rotate(new Vector3(0f, 0f, 0f), Space.World);
        
        myLine.SetPositions(new Vector3[] { topPos, bottomPos });

        float mainWidth = 0.004f;
        myLine.startWidth = mainWidth;
        myLine.endWidth = mainWidth;
    }

    private float getFloatAtPos(float[] Values, int pos)
    {
        if (Values[pos] >= 0)
        {
            return Values[pos];
        }
        else       // If value is disabled return fitting standard value.
        {
            return 0.0f;
        }
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }
    
}
