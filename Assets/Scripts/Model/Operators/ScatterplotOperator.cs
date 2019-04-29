using Assets.Scripts.Model;
using UnityEngine;

namespace Model.Operators
{
    public class ScatterplotOperator : GenericOperator
    {
        public bool toggle;
    
        public override void Start()
        {
            base.Start();
        }

        public override bool ValidateIfOperatorPossibleForParents(GenericOperator parent)
        {
            // can only be spawned if parent has output data
            return parent != null && parent.GetOutputData() != null;
        }

        public override bool Process()
        {
            #region CommentedScript
            /*following script is commented for faster loading of operators
             * However, for functionality of operators
             * uncomment the code in the region
            // Create Visualization
            Visualization.GetComponent<GenericVisualization>().CreateVisualization();
            // Enable Interaction Script

            SetOutputData(GetRawInputData()); // Visualization does not change data
            */
            #endregion
            return true; 
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
data.hour = hour;
data.minute = minute;
data.second = second;
data.ms = millisecond;
}

public override void LoadSpecificData(OperatorData data)
{

}
}
}
