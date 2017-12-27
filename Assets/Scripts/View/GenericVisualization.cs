using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;

public abstract class GenericVisualization : MonoBehaviour
{
    private GenericOperator _op;

    private void Awake()
    {
        _op = transform.parent.GetComponent<GenericOperator>();
    }

    private void Update () {
		
	}

    public void SetOperator(GenericOperator operatorObj)
    {
        _op = operatorObj;
    }

    public GenericOperator GetOperator()
    {
        return _op;
    }

    public abstract void CreateVisualization();


}
