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

        public override void StoreData()
        {
            data.name = gameObject.name.Replace("(Clone)", "");
            data.ID = Id;
            if (Parents == null || Parents.Count == 0) data.parent = -1;
            else data.parent = Parents[0].Id;
            data.posX = GetIcon().transform.position.x;
            data.posY = GetIcon().transform.position.y;
            data.posZ = GetIcon().transform.position.z;
        }

        public override void LoadSpecificData(OperatorData data)
        {

        }
    }
}
