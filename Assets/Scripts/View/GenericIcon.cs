using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
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

}
