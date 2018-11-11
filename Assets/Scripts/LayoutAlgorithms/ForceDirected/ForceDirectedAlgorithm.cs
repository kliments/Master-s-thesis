using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDirectedAlgorithm : MonoBehaviour {
    public Observer observer;

    private const float STIFFNESS_CONSTANT = 81.76f;
    private const int REPLUSION_CONSTANT = 10000;

    private const float DEFAULT_DAMPING = 0.5f;
    private const int DEFAULT_SPRING_LENGHT = 1;
    private const int DEFAULT_MAX_ITERATIONS = 500;
    private const float SPRING_LENGHT = 0.05f;
    public bool touch;

    private int threshold = 500;
    private int x = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(touch)
        {
            if (x == threshold) touch = false;
            Calculate(0.5f);
            UpdatePosition(0.5f);
            x++;
        }
	}

    protected void Calculate(float iTimeStep)
    {
        foreach(var op1 in observer.GetOperators())
        {
            //apply Coulomb's law for the nodes
            foreach(var op2 in observer.GetOperators())
            {
                if(op1!=op2)
                {
                    Vector3 d = op1.GetIcon().transform.localPosition - op2.GetIcon().transform.localPosition;
                    float distance = Vector3.Distance(op1.GetIcon().transform.localPosition, op2.GetIcon().transform.localPosition);
                    Vector3 direction = d.normalized;

                    ApplyForce(op1, (direction * REPLUSION_CONSTANT)/(distance*0.5f));
                    ApplyForce(op2, (direction * REPLUSION_CONSTANT) / (distance * -0.5f));
                }
            }

            //apply Hook's law for the edges
            foreach(var op3 in op1.Children)
            {
                Vector3 d = op3.GetIcon().transform.localPosition - op1.GetIcon().transform.localPosition;
                float displacement = SPRING_LENGHT - d.magnitude;
                Vector3 direction = d.normalized;

                ApplyForce(op1, (direction * (STIFFNESS_CONSTANT * displacement * -0.5f)));
                ApplyForce(op3, (direction * (STIFFNESS_CONSTANT * displacement * 0.5f)));
            }

            //attract to centre
            Vector3 dir = op1.GetIcon().transform.localPosition * -1;
            float displ = dir.magnitude;
            dir = dir.normalized;
            ApplyForce(op1,(dir * (STIFFNESS_CONSTANT * displ * 0.4f)));

            //update velocity
            Vector3 temp = op1.GetIcon().GetComponent<IconProperties>().acceleration * 0.5f;
            op1.GetIcon().GetComponent<IconProperties>().velocity += temp;
            op1.GetIcon().GetComponent<IconProperties>().velocity *= DEFAULT_DAMPING;
            op1.GetIcon().GetComponent<IconProperties>().acceleration = Vector3.zero;
        }
    }

    private void ApplyForce(GenericOperator op, Vector3 force)
    {
        op.GetIcon().GetComponent<IconProperties>().ApplyForce(force);
    }

    protected void UpdatePosition(float iTimeStep)
    {
        foreach(var op in observer.GetOperators())
        {
            var velocity = op.GetIcon().GetComponent<IconProperties>().velocity;
            op.GetIcon().transform.localPosition += velocity * iTimeStep;
        }
    }
}
