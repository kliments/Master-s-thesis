using Assets.Scripts.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDirectedAlgorithm : MonoBehaviour {
    public Observer observer;

    //Default initial temperature for simulated annealing
    public float DefaultStartingTemperature = 0.2f;
    
    //Temperature where the simulated annealing should stop
    public float DefaultMinimumTemperature = 0.01f;

    //The ratio between two successive temperatures in the simulated annealing
    public float DefaultTemperatureAttenuation = 0.95f;

    //The current temperature in the simulated annealing
    public float Temperature;

    public float STIFFNESS_CONSTANT;
    public int REPLUSION_CONSTANT;

    public float DEFAULT_DAMPING;
    public float SPRING_LENGHT;
    public int DEFAULT_MAX_ITERATIONS;
    public int x;
    public bool calculateForceDirected, randomize;

    /// The function defining the attraction force between two connected nodes.
    /// Arcs are viewed as springs that want to bring the two connected nodes together.
    /// The function takes a single parameter, which is the distance of the two nodes.
    public Func<double, double> SpringForce { get; set; }

    /// The function defining the repulsion force between two nodes.
    /// Nodes are viewed as electrically charged particles which repel each other.
    /// The function takes a single parameter, which is the distance of the two nodes.
    public Func<double, double> ElectricForce { get; set; }

    // Use this for initialization
    void Start () {
        ElectricForce = (d => 1 / (d * d));
        SpringForce = (d => 2 * Math.Log(d));

        // reset the temperature
        Temperature = DefaultStartingTemperature;
    }
	
	// Update is called once per frame
	void Update () {
        if(randomize)
        {
            randomize = false;
            RandomizePositions();
        }
		if(calculateForceDirected)
        {
            if (x > DEFAULT_MAX_ITERATIONS)
            {
                calculateForceDirected = false;
                x = -1;
            }
            Calculate();
            x++;
        }
	}

    void RandomizePositions()
    {
        // reset the temperature
        Temperature = DefaultStartingTemperature;
        foreach (var op in observer.GetOperators())
        {
            var newPos = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            op.GetIcon().transform.localPosition = newPos;
        }
    }

    protected void Calculate()
    {
        foreach(var op1 in observer.GetOperators())
        {
            Vector3 pos1 = op1.GetIcon().transform.localPosition;
            double xForce = 0, yForce = 0, zForce = 0;

            //attraction forces towards children
            foreach(var child in op1.Children)
            {
                Vector3 pos2 = child.GetIcon().transform.localPosition;
                double d = Vector3.Distance(pos1, pos2) * 10;
                double force = Temperature * SpringForce(d);
                xForce += (pos2.x - pos1.x) / d * force;
                yForce += (pos2.y - pos1.y) / d * force;
                zForce += (pos2.z - pos1.z) / d * force;
            }

            //attraction forces towards parents
            foreach (var parent in op1.Parents)
            {
                Vector3 pos2 = parent.GetIcon().transform.localPosition;
                double d = Vector3.Distance(pos1, pos2) * 10;
                double force = Temperature * SpringForce(d);
                xForce += (pos2.x - pos1.x) / d * force;
                yForce += (pos2.y - pos1.y) / d * force;
                zForce += (pos2.z - pos1.z) / d * force;
            }

            //repulsion forces
            foreach(var op2 in observer.GetOperators())
            {
                if (op1 == op2) continue;
                Vector3 pos2 = op2.GetIcon().transform.localPosition;
                double d = Vector3.Distance(pos1, pos2) * 10;
                double force = Temperature * ElectricForce(d);
                xForce += (pos1.x - pos2.x) / d * force;
                yForce += (pos1.y - pos2.y) / d * force;
                zForce += (pos1.z - pos2.z) / d * force;
            }
            Vector3 finalForce = new Vector3((float)xForce, (float)yForce, (float)zForce);
            op1.GetIcon().GetComponent<IconProperties>().ApplyForce(finalForce);
        }

        //update positions
        foreach(var op in observer.GetOperators())
        {
            op.GetIcon().transform.localPosition += op.GetIcon().GetComponent<IconProperties>().acceleration;
            op.GetIcon().GetComponent<IconProperties>().acceleration = Vector3.zero;
        }
        Temperature *= DefaultTemperatureAttenuation;
    }
    private void ApplyForce(GenericOperator op, Vector3 force)
    {
        op.GetIcon().GetComponent<IconProperties>().ApplyForce(force);
    }
}
