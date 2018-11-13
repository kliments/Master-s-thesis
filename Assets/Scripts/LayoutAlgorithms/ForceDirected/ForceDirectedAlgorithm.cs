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
    
    public bool calculateForceDirected, randomize, reloadOriginalLocations, saveLoc;

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

        saveLoc = true;
    }
	
	// Update is called once per frame
	void Update () {
        if(observer.GetOperators().Count > 10 && saveLoc)
        {

        }
        if(randomize)
        {
            randomize = false;
            RandomizePositions();
        }
		if(calculateForceDirected)
        {
            if (Temperature < DefaultMinimumTemperature)
            {
                calculateForceDirected = false;
                Temperature = 0.2f;
            }
            Calculate();
        }
	}

    void RandomizePositions()
    {
        // reset the temperature
        Temperature = DefaultStartingTemperature;
        foreach (var op in observer.GetOperators())
        {
            var newPos = new Vector3(op.GetIcon().transform.position.x, UnityEngine.Random.Range(0f, 3f), UnityEngine.Random.Range(2f, 6f));
            op.GetIcon().transform.position = newPos;
        }
    }

    protected void Calculate()
    {
        foreach(var op1 in observer.GetOperators())
        {
            Vector3 pos1 = op1.GetIcon().transform.position;
            double xForce = 0, yForce = 0, zForce = 0;

            //attraction forces towards children
            foreach(var child in op1.Children)
            {
                Vector3 pos2 = child.GetIcon().transform.position;
                double d = Vector3.Distance(pos1, pos2);
                double force = Temperature * SpringForce(d*10);
                xForce += (pos2.x - pos1.x) / d * force;
                yForce += (pos2.y - pos1.y) / d * force;
                zForce += (pos2.z - pos1.z) / d * force;
            }

            //attraction forces towards parents
            foreach (var parent in op1.Parents)
            {
                Vector3 pos2 = parent.GetIcon().transform.position;
                double d = Vector3.Distance(pos1, pos2);
                double force = Temperature * SpringForce(d*10);
                xForce += (pos2.x - pos1.x) / d * force;
                yForce += (pos2.y - pos1.y) / d * force;
                zForce += (pos2.z - pos1.z) / d * force;
            }

            //repulsion forces
            foreach(var op2 in observer.GetOperators())
            {
                if (op1 == op2) continue;
                Vector3 pos2 = op2.GetIcon().transform.position;
                double d = Vector3.Distance(pos1, pos2);
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
            op.GetIcon().transform.position += op.GetIcon().GetComponent<IconProperties>().acceleration;
            op.GetIcon().GetComponent<IconProperties>().acceleration = Vector3.zero;
        }
        Temperature *= DefaultTemperatureAttenuation;
    }
    private void ApplyForce(GenericOperator op, Vector3 force)
    {
        op.GetIcon().GetComponent<IconProperties>().ApplyForce(force);
    }

    double Distance(Vector3 a, Vector3 b)
    {
        Vector3 difference = a - b;
        difference.x /= 10;
        difference.y /= 5;
        difference.z *= 100;
        return Math.Sqrt(Math.Pow(difference.x, 2) + Math.Pow(difference.y, 2) + Math.Pow(difference.z, 2));
    }
}
