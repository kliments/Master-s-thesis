using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDirectedAlgorithm : MonoBehaviour {
    public Observer observer;

    private const double STIFFNESS_CONSTANT = 0.1f;
    private const double REPLUSION_CONSTANT = 40000;

    private const double DEFAULT_DAMPING = 0.5f;
    private const int DEFAULT_SPRING_LENGHT = 1;
    private const int DEFAULT_MAX_ITERATIONS = 500;

    public bool touch;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(touch)
        {
            touch = false;
            ApplyCoulombsLaw();
        }
	}

    private void Arrange(float iTimeStep)
    {

    }

    protected void ApplyCoulombsLaw()
    {
        foreach(var op1 in observer.GetOperators())
        {
            foreach(var op2 in observer.GetOperators())
            {
                if(op1!=op2)
                {
                    Vector3 d = op1.GetIcon().transform.position - op2.GetIcon().transform.position;
                    float distance = Vector3.Distance(op1.GetIcon().transform.position, op2.GetIcon().transform.position);
                    float distance2 = d.magnitude + 0.1f;
                    Vector3 direction = d.normalized;
                    int x = 0;
                }
            }
        }
    }

    private void ApplyForce(GenericOperator op, Vector3 force)
    {

    }
}
