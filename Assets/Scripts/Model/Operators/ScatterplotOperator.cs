using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class ScatterplotOperator : GenericOperator
{
    
    
    public override void Start()
    {
        base.Start();
    }

    public override bool process()
    {
        // Create Visualization
        visualization.GetComponent<GenericVisualization>().createVisualization();
        // Enable Interaction Script
        return true;
    }
    
}
