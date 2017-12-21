﻿using System;
using Assets.Scripts.Model;
using UnityEngine;

namespace Controller.Interaction
{
    public abstract class GenericIconInteractionController : Targetable {

        void OnEnable()
        {
            InputController.LeftClickOnTargetEvent += OnLeftClickOnTargetEvent;
        }

        void OnDisable()
        {
            InputController.LeftClickOnTargetEvent -= OnLeftClickOnTargetEvent;
        }

        public GenericOperator GetOperator()
        {
            Transform t = this.transform;
            while (true)
            {
                if (t == null) throw new Exception("No operator parent found in hierarchy!");

                if (t.parent.GetComponent<GenericIcon>() != null)
                {
                    return t.parent.GetComponent<GenericIcon>().getOperator();
                }
                else
                {
                    t = t.parent;
                }
            }
        }
    }
}
