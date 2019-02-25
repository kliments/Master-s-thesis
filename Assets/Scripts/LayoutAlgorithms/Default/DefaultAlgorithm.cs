using Assets.Scripts.Model;
using Model.Operators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAlgorithm : GeneralLayoutAlgorithm {

    public List<Vector3> positions;
    private Transform tempTransform;
	// Use this for initialization
	void Start () {
        observer = (Observer)FindObjectOfType(typeof(Observer));
        tempTransform = new GameObject().transform;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void StartAlgorithm()
    {
        //don't start if another algorithm is in process
        if (!GetComponent<LayoutAlgorithm>().currentLayout.AlgorithmHasFinished()) return;
        //set flag that this algorithm is running
        SetStart();

        if (observer == null) observer = (Observer)FindObjectOfType(typeof(Observer));
        //set 2 lines, in case previous algorithm had changed it to 3 (ex. RDT)
        if (GetComponent<LayoutAlgorithm>().currentLayout != this)
        {
            foreach (var op in observer.GetOperators())
            {
                if (op.Parents != null)
                {
                    if (op.Parents.Count != 0)
                    {
                        op.GetComponent<LineRenderer>().positionCount = 2;
                        op.GetComponent<LineRenderer>().SetPositions(new Vector3[] { op.Parents[0].GetIcon().transform.position, op.GetIcon().transform.position });
                    }
                }
            }
        }
        float counter = 1;
        Vector3 inverseTempPosition = new Vector3();
        Vector3 tempTransformPos = new Vector3();
        GetComponent<LayoutAlgorithm>().currentLayout = this;
        foreach(var op in observer.GetOperators())
        {
            if (op.Parents != null && op.Parents.Count > 0)
            {
                //shift next node to the right
                inverseTempPosition = op.GetIcon().transform.parent.InverseTransformPoint(op.Parents[0].GetIcon().GetComponent<IconProperties>().newPos);
                op.GetIcon().GetComponent<IconProperties>().newPos = op.GetIcon().transform.parent.TransformPoint(counter, inverseTempPosition.y, 0);
                if (!op.GetType().Equals((typeof(NewOperator))))
                {
                    if (!op.Parents[0].GetType().Equals(typeof(SplitDatasetOperator))) counter++;
                    else counter += 0.5f;
                }
                //shift above
                if (op.Parents[0].Children != null)
                {
                    int childCount = 1;
                    foreach (var child in op.Parents[0].Children)
                    {
                        if (op == child) break;
                        else childCount++;
                    }
                    op.GetIcon().GetComponent<IconProperties>().newPos += new Vector3(0, (childCount - 1) * 0.3f, 0);
                    if (op.Parents[0].GetType() == typeof(SplitDatasetOperator))
                    {
                        //assign parent transform as current transform for rotation of children
                        tempTransform = op.GetIcon().transform.parent;
                        tempTransformPos = tempTransform.position;
                        //shift the transform to the new SplitDatasetOp position
                        tempTransform.position = op.Parents[0].GetIcon().GetComponent<IconProperties>().newPos;
                        op.GetIcon().GetComponent<IconProperties>().newPos = op.Parents[0].GetComponent<SplitDatasetOperator>().getSpawnPositionOffsetForButton(tempTransform, childCount - 1, op.Parents[0].Children.Count); ;
                        op.GetIcon().GetComponent<IconProperties>().repos = true;
                        op.GetIcon().GetComponent<IconProperties>().originalPos = op.GetIcon().GetComponent<IconProperties>().newPos;
                        //shift transform back
                        tempTransform.position = tempTransformPos;
                    }
                }
                op.GetIcon().GetComponent<IconProperties>().repos = true;
            }
            else
            {
                op.GetIcon().GetComponent<IconProperties>().newPos = op.GetIcon().transform.parent.TransformPoint(Vector3.zero);
                op.GetIcon().GetComponent<IconProperties>().repos = true;
            }
        }
        base.ColorEdges();
        //set flag for this algorithm has finished
        SetFinish();
    }

    void OnEnable()
    {
        subscriber.addListener(this);
    }
    public override void PreScanCalculation()
    {
        StartAlgorithm();
        foreach (var op in observer.GetOperators())
        {
            op.GetIcon().transform.position = op.GetIcon().GetComponent<IconProperties>().newPos;
            if(op.GetComponent<LineRenderer>() != null)
            {
                op.GetComponent<LineRenderer>().positionCount = 2;
                op.GetComponent<LineRenderer>().SetPosition(0, op.GetIcon().transform.position);
                op.GetComponent<LineRenderer>().SetPosition(1, op.Parents[0].GetIcon().transform.position);
            }
        }
    }
}
