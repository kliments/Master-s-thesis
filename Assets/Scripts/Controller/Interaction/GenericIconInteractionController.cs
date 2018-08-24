using System;
using Assets.Scripts.Model;
using UnityEngine;

namespace Controller.Interaction
{
    public abstract class GenericIconInteractionController : Targetable
    {

        public bool dragmode = false; 

        private void OnEnable()
        {
            InputController.LeftClickOnTargetEvent += OnLeftClickOnTargetEvent;
            InputController.LeftClickOnTargetEvent += switchVisualization;
            InputController.LeftClickOnTargetEvent += switchDragmode;
            InputController.LeftClickReleaseEvent += switchDragmode;
        }

        private void OnDisable()
        {
            InputController.LeftClickOnTargetEvent -= OnLeftClickOnTargetEvent;
            InputController.LeftClickOnTargetEvent -= switchVisualization;
            InputController.LeftClickOnTargetEvent -= switchDragmode;
            InputController.LeftClickReleaseEvent -= switchDragmode;
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
            if (target == this)
            {
                GetOperator().Observer.selectOperator(GetOperator());
            }
            else
            {
                setDragmode(false);
            }
        }

        private void switchDragmode()
        {
            setDragmode(false);
        }

        private void switchDragmode(Targetable target)
        {
            
            if (target == this)
            {
                setDragmode(true);
            }
            else
            {
                setDragmode(false);
            }
        }

        private void setDragmode(bool dragmode)
        {
            this.dragmode = dragmode;
        }

    }
}
