using Assets.Scripts.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDirectedAlgorithm : MonoBehaviour {
    public Observer observer;

    // Time bins where nodes are contained depending on the time of their creation
    public List<List<GenericOperator>> _timeBins;

    //Default initial temperature for simulated annealing
    public float DefaultStartingTemperature = 0.2f;
    
    //Temperature where the simulated annealing should stop
    public float DefaultMinimumTemperature = 0.01f;

    //The ratio between two successive temperatures in the simulated annealing
    public float DefaultTemperatureAttenuation = 0.95f;

    //The current temperature in the simulated annealing
    public float Temperature;
    
    public bool calculateForceDirected, randomize, reloadOriginalLocations, saveLoc, applyTimeBins;
    private int counter = 0;

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

        _timeBins = new List<List<GenericOperator>>();
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
            if (Temperature < DefaultMinimumTemperature)
            {
                counter++;
                Temperature = 0.2f;
                if(counter > 5)
                {
                    calculateForceDirected = false;
                    counter = 0;
                }
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

    void AllocateTimeBins()
    {
        _timeBins = new List<List<GenericOperator>>();
        DateTime _firstNode = observer.GetOperators()[0].timeOfCreation;
        DateTime _lastNode = observer.GetOperators()[observer.GetOperators().Count - 1].timeOfCreation;
        double diffInSeconds = (_lastNode - _firstNode).TotalSeconds;
        int chunks = (int)diffInSeconds / 10;
        int counter = 0;
        for(int i = 0; i < chunks + 1; i++)
        {
            DateTime tempTime = _firstNode.AddSeconds(i*10 + 10);
            List<GenericOperator> tempList = new List<GenericOperator>();
            for(int j = counter; j < observer.GetOperators().Count; j++)
            {
                if(observer.GetOperators()[j].timeOfCreation < tempTime)
                {
                    tempList.Add(observer.GetOperators()[j]);
                    counter++;
                }
            }
            if(tempList.Count != 0) _timeBins.Add(tempList);
        }
    }

    protected void Calculate()
    {
        //ForceDirectedWithTemporalDependency();
        ForceDirectedWithoutTemporalDependency();
        //update positions
        foreach (var op in observer.GetOperators())
        {
            op.GetIcon().transform.position += op.GetIcon().GetComponent<IconProperties>().acceleration;
            op.GetIcon().GetComponent<IconProperties>().acceleration = Vector3.zero;
        }
        Temperature *= DefaultTemperatureAttenuation;
    }

    private void ForceDirectedWithTemporalDependency()
    {
        if (_timeBins.Count == 0) AllocateTimeBins();
        foreach (var list in _timeBins)
        {
            foreach (var node1 in list)
            {
                Vector3 pos1 = node1.GetIcon().transform.position;
                double xForce = 0, yForce = 0, zForce = 0;

                //attraction forces towards children
                if (node1.Children != null)
                {
                    foreach (var child in node1.Children)
                    {
                        //if (!list.Contains(child)) continue;
                        Vector3 pos2 = child.GetIcon().transform.position;
                        double d = Vector3.Distance(pos1, pos2);
                        double force = Temperature * SpringForce(d * 10);
                        //xForce += (pos2.x - pos1.x) / d * force;
                        yForce += (pos2.y - pos1.y) / d * force;
                        zForce += (pos2.z - pos1.z) / d * force;
                    }
                }

                //attraction forces towards parents
                if (node1.Parents != null)
                {
                    foreach (var parent in node1.Parents)
                    {
                        //if (!list.Contains(parent)) continue;
                        Vector3 pos2 = parent.GetIcon().transform.position;
                        double d = Vector3.Distance(pos1, pos2);
                        double force = Temperature * SpringForce(d * 10);
                        //xForce += (pos2.x - pos1.x) / d * force;
                        yForce += (pos2.y - pos1.y) / d * force;
                        zForce += (pos2.z - pos1.z) / d * force;
                    }
                }

                //repulsion forces
                foreach (var node2 in list)
                {
                    if (node1 == node2) continue;
                    if (!list.Contains(node2)) continue;
                    Vector3 pos2 = node2.GetIcon().transform.position;
                    double d = Vector3.Distance(pos1, pos2);
                    double force = Temperature * ElectricForce(d);
                    //xForce += (pos1.x - pos2.x) / d * force;
                    yForce += (pos1.y - pos2.y) / d * force;
                    zForce += (pos1.z - pos2.z) / d * force;
                }
                Vector3 finalForce = new Vector3((float)xForce, (float)yForce, (float)zForce);
                node1.GetIcon().GetComponent<IconProperties>().ApplyForce(finalForce);
            }
        }
    }

    private void ForceDirectedWithoutTemporalDependency()
    {
        foreach (var op1 in observer.GetOperators())
        {
            Vector3 pos1 = op1.GetIcon().transform.position;
            double xForce = 0, yForce = 0, zForce = 0;

            //attraction forces towards children
            if (op1.Children != null)
            {
                foreach (var child in op1.Children)
                {
                    Vector3 pos2 = child.GetIcon().transform.position;
                    double d = Vector3.Distance(pos1, pos2);
                    double force = Temperature * SpringForce(d * 10);
                    xForce += (pos2.x - pos1.x) / d * force;
                    yForce += (pos2.y - pos1.y) / d * force;
                    zForce += (pos2.z - pos1.z) / d * force;
                }
            }

            //attraction forces towards parents
            if (op1.Parents != null)
            {
                foreach (var parent in op1.Parents)
                {
                    Vector3 pos2 = parent.GetIcon().transform.position;
                    double d = Vector3.Distance(pos1, pos2);
                    double force = Temperature * SpringForce(d * 10);
                    xForce += (pos2.x - pos1.x) / d * force;
                    yForce += (pos2.y - pos1.y) / d * force;
                    zForce += (pos2.z - pos1.z) / d * force;
                }
            }

            //repulsion forces
            foreach (var op2 in observer.GetOperators())
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
    }
}
