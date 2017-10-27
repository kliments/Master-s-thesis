using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;

public abstract class GenericVisualization : MonoBehaviour
{
    GenericOperator op;

    void Awake()
    {
        op = transform.parent.GetComponent<GenericOperator>();
    }
    
    void Update () {
		
	}

    public void setOperator(GenericOperator operatorObj)
    {
        op = operatorObj;
    }

    public GenericOperator getOperator()
    {
        return op;
    }

    public abstract void createVisualization();


}
