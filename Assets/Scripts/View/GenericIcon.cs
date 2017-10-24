using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;

public abstract class GenericIcon : MonoBehaviour {
    public GenericOperator op;

    void Awake()
    {
        op = transform.parent.GetComponent<GenericOperator>();
    }

    public void setOperator(GenericOperator operatorObj)
    {
        op = operatorObj;
    }

    public GenericOperator getOperator()
    {
        return op;
    }

}
