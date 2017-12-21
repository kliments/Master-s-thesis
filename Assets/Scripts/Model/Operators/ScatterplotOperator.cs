using Assets.Scripts.Model;

namespace Model.Operators
{
    public class ScatterplotOperator : GenericOperator
    {
    
    
        public override void Start()
        {
            base.Start();
        }

        public override bool validateIfOperatorPossibleForParents(GenericOperator parent)
        {
            // can only be spawned if parent has output data
            return parent.getOutputData() != null;
        }

        public override bool process()
        {
            // Create Visualization
            visualization.GetComponent<GenericVisualization>().createVisualization();
            // Enable Interaction Script
            return true;
        }
    
    }
}
