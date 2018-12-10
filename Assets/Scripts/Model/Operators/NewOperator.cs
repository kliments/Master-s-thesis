using System;
using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using Controller.Interaction.Icon;
using UnityEngine;

public class NewOperator :  GenericOperator
{
    private Vector3 _oldPosOp = new Vector3();
    private Vector3 _newPosOp = new Vector3();
    private Vector3 _oldPosParent = new Vector3();
    private Vector3 _newPosParent = new Vector3();
    
    public override bool Process()
    {
        SetOutputData(GetRawInputData());
        return true;
    }

    public override bool ValidateIfOperatorPossibleForParents(GenericOperator parent)
    {
        // new operator can only be spawned in new space - and not be derived from previous objects
        return parent == null; 
    }

    // Update is called once per frame
    private void Update () {

        if (Parents != null)
        {
            if (Parents.Count != 0)
            {
                _oldPosOp = _newPosOp;
                _newPosOp = GetIcon().transform.position;
                _oldPosParent = _newPosParent;
                _newPosParent = Parents[0].GetIcon().transform.position;
                if (GetComponent<LineRenderer>().positionCount == 2)
                {
                    //Update the line renderer if position of node changes
                    if (_oldPosOp != _newPosOp)
                    {
                        GetComponent<LineRenderer>().SetPositions(new Vector3[] { _newPosParent, GetIcon().transform.position });
                    }
                    // Update the line renderer if position of parent changes
                    if (_oldPosParent != _newPosParent)
                    {
                        GetComponent<LineRenderer>().SetPositions(new Vector3[] { _newPosParent, GetIcon().transform.position });
                    }
                }
            }
        }
    }

    protected override void OnUnselectAction()
    {
        NewIconInteractionController interactionController = Icon.GetComponentInChildren<NewIconInteractionController>();
        if(interactionController!=null) interactionController.hideOptions();
    }

    public override void StoreData()
    {

    }

    public override void LoadSpecificData(OperatorData data)
    {

    }
}
