using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewOperator :  GenericOperator
{

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
	   
    }

    
}
