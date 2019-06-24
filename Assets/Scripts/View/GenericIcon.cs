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

    public void SelectThisIcon()
    {
        GameObject selectedIcon = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Vector3 scale = transform.lossyScale;
        scale.z = 0.01f;
        foreach(Transform child in transform)
        {
            if(child.gameObject.activeSelf)
            {
                selectedIcon.transform.parent = child;
                break;
            }
        }
        selectedIcon.transform.localScale = new Vector3(1,1,1.1f);
        selectedIcon.transform.localPosition = new Vector3(0, 0, 0);
        selectedIcon.transform.localEulerAngles = new Vector3(0, 0, 0);
        selectedIcon.tag = "SelectedIcon";
        selectedIcon.GetComponent<MeshRenderer>().material.color = Color.red;
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
