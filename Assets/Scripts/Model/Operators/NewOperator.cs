using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewOperator :  GenericOperator
{

    public override bool process()
    {
        return true;
    }

    public override bool validateIfOperatorPossibleForParents(GenericOperator parent)
    {
        // new operator can allways be spawned
        return true; 
    }

    // Update is called once per frame
	void Update () {
	   
    }

    
}
