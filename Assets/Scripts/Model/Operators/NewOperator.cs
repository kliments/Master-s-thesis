using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewOperator :  GenericOperator
{

    public override bool Process()
    {
        return true;
    }

    public override bool ValidateIfOperatorPossibleForParents(GenericOperator parent)
    {
        // new operator can allways be spawned
        return true; 
    }

    // Update is called once per frame
    private void Update () {
	   
    }

    
}
