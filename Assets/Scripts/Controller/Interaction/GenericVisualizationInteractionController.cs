using System;
using Assets.Scripts.Model;
using UnityEngine;

namespace Controller.Interaction
{
    public abstract class GenericVisualizationInteractionController : Targetable {
        public GenericOperator GetOperator()
        {
            var t = transform;
            while (true)
            {
                if (t == null) throw new Exception("No operator parent found in hierarchy!");

                if (t.parent.GetComponent<GenericVisualization>() != null)
                {
                    return t.parent.GetComponent<GenericVisualization>().GetOperator();
                }
                t = t.parent;
            }
        }
    }
}
