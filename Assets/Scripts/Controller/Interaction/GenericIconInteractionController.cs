using System;
using Assets.Scripts.Model;
using UnityEngine;

namespace Controller.Interaction
{
    public abstract class GenericIconInteractionController : Targetable {
        private void OnEnable()
        {
            InputController.LeftClickOnTargetEvent += OnLeftClickOnTargetEvent;
        }

        private void OnDisable()
        {
            InputController.LeftClickOnTargetEvent -= OnLeftClickOnTargetEvent;
        }

        public GenericOperator GetOperator()
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
    }
}
