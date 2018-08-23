using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using Controller.Interaction;
using UnityEngine;

public abstract class GenericIcon : MonoBehaviour {
    public GenericOperator Op;
    private Vector3 dragPosition;

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

//    private void Update()
//    {
//        GenericIconInteractionController interactionController = transform.GetComponentInChildren<GenericIconInteractionController>();
//        if (interactionController != null && interactionController.dragmode)
//        {
//            if (dragPosition == Vector3.zero)
//            {
//                dragPosition = GameObject.FindObjectOfType<InputController>().positionLeft;
//            }
//            else
//            {
//                gameObject.transform.position += Vector3.Normalize(GameObject.FindObjectOfType<InputController>().positionLeft - dragPosition) / 10;
//            }
//            
//        }
//        else
//        {
//            dragPosition = Vector3.zero;
//        }
//        
//    }



}
