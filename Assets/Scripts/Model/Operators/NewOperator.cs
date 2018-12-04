using System;
using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using Controller.Interaction.Icon;
using UnityEngine;

public class NewOperator :  GenericOperator
{
    private Vector3 _oldPos = new Vector3();
    private Vector3 _newPos = new Vector3();
    
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
                _oldPos = _newPos;
                _newPos = Parents[0].GetIcon().transform.position;
                // Update the line renderer if position of parent changes
                if (_oldPos != _newPos)
                {
                    GetComponent<LineRenderer>().SetPositions(new Vector3[] { _newPos, GetIcon().transform.position });
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
