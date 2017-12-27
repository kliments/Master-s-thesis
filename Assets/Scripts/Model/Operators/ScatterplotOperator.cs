﻿using Assets.Scripts.Model;

namespace Model.Operators
{
    public class ScatterplotOperator : GenericOperator
    {
    
    
        public override void Start()
        {
            base.Start();
        }

        public override bool ValidateIfOperatorPossibleForParents(GenericOperator parent)
        {
            // can only be spawned if parent has output data
            return parent.GetOutputData() != null;
        }

        public override bool Process()
        {
            // Create Visualization
            Visualization.GetComponent<GenericVisualization>().CreateVisualization();
            // Enable Interaction Script
            return true;
        }
    
    }
}
