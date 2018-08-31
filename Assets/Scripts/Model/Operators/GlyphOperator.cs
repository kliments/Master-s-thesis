using Assets.Scripts.Model;
using UnityEngine;

namespace Model.Operators
{
    public class GlyphOperator : GenericOperator
    {
        public override bool Process()
        {
            // Create Visualization
            Visualization.GetComponent<GenericVisualization>().CreateVisualization();
            // Enable Interaction Script

            SetOutputData(GetRawInputData()); // Visualization does not change data
            return true;
        }

        public override bool ValidateIfOperatorPossibleForParents(GenericOperator parent)
        {
            // can only be spawned if parent has output data
            return parent != null && parent.GetOutputData() != null;
        }

        // Use this for initialization
        void Start()
        {
            base.Start();
        }
    }
}
