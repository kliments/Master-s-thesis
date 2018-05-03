using System;
using Assets.Scripts.Model;
using UnityEngine;

namespace Controller.Interaction
{
    public abstract class GenericIconInteractionController : Targetable {
        private void OnEnable()
        {
            InputController.LeftClickOnTargetEvent += OnLeftClickOnTargetEvent;
            InputController.LeftClickOnTargetEvent += switchVisualization;
        }

        private void OnDisable()
        {
            InputController.LeftClickOnTargetEvent -= OnLeftClickOnTargetEvent;
            InputController.LeftClickOnTargetEvent -= switchVisualization;
        }

        protected GenericOperator GetOperator()
        {
            var t = transform;
            while (true)
            {
                if (t == null) throw new Exception("No operator parent found in hierarchy!");

                if (t.parent.GetComponent<GenericIcon>() != null)
                {
                    return t.parent.GetComponent<GenericIcon>().GetOperator();
                }
                t = t.parent;
            }
        }

        private void switchVisualization(Targetable target)
        {
            if(target == this)
                GetOperator().Observer.selectOperator(GetOperator());
        }
    }
}
