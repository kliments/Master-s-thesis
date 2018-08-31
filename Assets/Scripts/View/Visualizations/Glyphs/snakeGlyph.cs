using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snakeGlyph : abstractGlyph
{

    public LineRenderer main;
    public LineRenderer arm1;
    public LineRenderer arm2;
    public LineRenderer leg1;
    public LineRenderer leg2;

    // Use this for initialization
    void Start()
    {
        //setValues(new float[5] {0.0f, 1.0f, 0.75f, 0.5f, 0.5f });
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

    // Use this for initialization
    public override void setValues(float[] Values) { 
        
        //Debug.Log(Values[1]);

        float mainRotateMin = 0.0f;
        float mainRotateMax = 180.0f;
        //float currentRotation = mainRotateMin + getFloatAtPos(Values, 0) * (mainRotateMax - mainRotateMin) - 90;
        //float currentSecondaryRotation = mainRotateMin + getFloatAtPos(Values, 5) * (mainRotateMax - mainRotateMin) - 90;

        float armMinRotation = 0.0f;
        float armMaxRotation = 90.0f;

        float legMinRotation = 0.0f;
        float legMaxRotation = -90.0f;

        Vector3 lineNoRotationPositionTop = new Vector3(0f, 0.1f, 0f);
        Vector3 lineNoRotationPositionBottom = new Vector3(0f, -0.1f, 0f);

        Vector3 lineRotation = new Vector3(mainRotateMin + (getFloatAtPos(Values, 5) * (mainRotateMax - mainRotateMin)),
            0.0f,
            mainRotateMin + (getFloatAtPos(Values, 0) * (mainRotateMax - mainRotateMin)));


        Vector3 topAncor = RotatePointAroundPivot(lineNoRotationPositionTop, new Vector3(0, 0, 0), lineRotation);
        Vector3 bottomAnchor = RotatePointAroundPivot(lineNoRotationPositionBottom, new Vector3(0, 0, 0), lineRotation);
        this.transform.Rotate(new Vector3(0.0f, 0.0f, 0.0f), Space.World);

        //this.transform.Rotate(new Vector3(currentSecondaryRotation, 0f, currentRotation), Space.World);

        //Vector3 topAncor = new Vector3(0, 0.1f, 0);
        //Vector3 bottomAnchor = new Vector3(0, -0.1f, 0);
        main.SetPositions(new Vector3[] { topAncor, bottomAnchor });

        //Vector3 arm1NoRotationPosition = new Vector3(0f, 0.3f, 0f);
        //Vector3 leg1NoRotationPosition = new Vector3(0f, -0.3f, 0f);
        Vector3 arm1NoRotationPosition = RotatePointAroundPivot(new Vector3(topAncor.x, topAncor.y + 0.2f, topAncor.z), topAncor, lineRotation);
        Vector3 leg1NoRotationPosition = RotatePointAroundPivot(new Vector3(bottomAnchor.x, bottomAnchor.y - 0.2f, bottomAnchor.z), bottomAnchor, lineRotation);


        Vector3 arm1Rotation = new Vector3(armMinRotation + getFloatAtPos(Values, 6) * (armMaxRotation - armMinRotation), 0.0f, armMinRotation + getFloatAtPos(Values, 1) * (armMaxRotation - armMinRotation));     //Positive is clockwise, negative counterclockwise
        Vector3 leg1Rotation = new Vector3(armMinRotation + getFloatAtPos(Values, 8) * (armMaxRotation - armMinRotation), 0.0f, armMinRotation + getFloatAtPos(Values, 3) * (armMaxRotation - armMinRotation));
        

        Vector3 arm1Pos = RotatePointAroundPivot(arm1NoRotationPosition, topAncor, arm1Rotation);//new Vector3(0, 0.5f, -0.5f);
        Vector3 leg1Pos = RotatePointAroundPivot(leg1NoRotationPosition, bottomAnchor, leg1Rotation);//new Vector3(0, -0.5f, -0.5f);
        
        Vector3 arm2NoRotationPosition = RotatePointAroundPivot(new Vector3(arm1Pos.x, arm1Pos.y + 0.2f, arm1Pos.z), arm1Pos, arm1Rotation + lineRotation);
        Vector3 leg2NoRotationPosition = RotatePointAroundPivot(new Vector3(leg1Pos.x, leg1Pos.y - 0.2f, leg1Pos.z), leg1Pos, leg1Rotation + lineRotation);

        Vector3 arm2Rotation = new Vector3(legMinRotation + getFloatAtPos(Values, 7) * (legMaxRotation - legMinRotation), 0.0f, legMinRotation + getFloatAtPos(Values, 2) * (legMaxRotation - legMinRotation));
        Vector3 leg2Rotation = new Vector3(legMinRotation + getFloatAtPos(Values, 9) * (legMaxRotation - legMinRotation), 0.0f, legMinRotation + getFloatAtPos(Values, 4) * (legMaxRotation - legMinRotation));

        Vector3 arm2Pos = RotatePointAroundPivot(arm2NoRotationPosition, arm1Pos, arm2Rotation);//new Vector3(0, 0.5f, 0.5f);
        Vector3 leg2Pos = RotatePointAroundPivot(leg2NoRotationPosition, leg1Pos, leg2Rotation);//new Vector3(0, -0.5f, 0.5f);

        arm1.SetPositions(new Vector3[] { topAncor, arm1Pos });
        arm2.SetPositions(new Vector3[] { arm1Pos, arm2Pos });
        leg1.SetPositions(new Vector3[] { bottomAnchor, leg1Pos });
        leg2.SetPositions(new Vector3[] { leg1Pos, leg2Pos });

        float mainWidth = 0.004f;
        float armWidth = 0.0025f;

        main.startWidth = mainWidth;
        main.endWidth = mainWidth;
        arm1.startWidth = armWidth;
        arm1.endWidth = armWidth;
        arm2.startWidth = armWidth;
        arm2.endWidth = armWidth;
        leg1.startWidth = armWidth;
        leg1.endWidth = armWidth;
        leg2.startWidth = armWidth;
        leg2.endWidth = armWidth;
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
