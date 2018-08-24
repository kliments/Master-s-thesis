using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using Controller.Interaction;
using UnityEngine;

public abstract class GenericIcon : MonoBehaviour {
    public GenericOperator Op;

    private void Awake()
    {
        if(transform != null && transform.parent != null)
        Op = transform.parent.GetComponent<GenericOperator>();
    }
    
    public void SetOperator(GenericOperator operatorObj)
    {
        Op = operatorObj;
    }

    public GenericOperator GetOperator()
    {
        return Op;
    }

    private void Update()
    {
        GenericIconInteractionController interactionController = transform.GetComponentInChildren<GenericIconInteractionController>();
        if (interactionController != null && interactionController.dragmode)
        {
            Vector3 vec = GetOperator().Observer.GetComponent<InputController>().getRaycastHitOnObject(GameObject.Find("wall2d"));
            if (vec != Vector3.zero)
                Op.GetIcon().transform.position = vec;
        }
    }



}
