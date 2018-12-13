using Assets.Scripts.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Calculates the position of nodes in Force Directed Algorithm
 * Also implements time-dependent positioning of nodes, by keeping their X position
 * and nodes affecting each-other only within particular time-bins
 */
public class ForceDirectedAlgorithm : GeneralLayoutAlgorithm {
    public Observer observer;

    // Time bins where nodes are contained depending on the time of their creation
    public List<List<GenericOperator>> _timeBins;

    // Original positions of nodes
    public Vector3[] positions;

    //Default initial temperature for simulated annealing
    public float DefaultStartingTemperature = 0.2f;
    
    //Temperature where the simulated annealing should stop
    public float DefaultMinimumTemperature = 0.01f;

    //The ratio between two successive temperatures in the simulated annealing
    public float DefaultTemperatureAttenuation = 0.999f;

    //The current temperature in the simulated annealing
    public float Temperature;
    
    public bool calculateForceDirected, randomize, saveOriginalLocations, loadLocations;

    /// The function defining the attraction force between two connected nodes.
    /// Arcs are viewed as springs that want to bring the two connected nodes together.
    /// The function takes a single parameter, which is the distance of the two nodes.
    public Func<double, double> SpringForce { get; set; }

    /// The function defining the repulsion force between two nodes.
    /// Nodes are viewed as electrically charged particles which repel each other.
    /// The function takes a single parameter, which is the distance of the two nodes.
    public Func<double, double> ElectricForce { get; set; }

    //frame counter for slower forceDirected visualization
    private int frameCounter;

    //repeater of algorithm
    private int repeatCounter;


    LineRenderer lr = new LineRenderer();
    // Use this for initialization
    void Start () {
        ElectricForce = (d => 1 / (d * d));
        SpringForce = (d => 2 * Math.Log(d));

        // reset the temperature
        Temperature = DefaultStartingTemperature;

        loadLocations = true;

        _timeBins = new List<List<GenericOperator>>();
    }
	
	// Update is called once per frame
	void Update () {
        if(saveOriginalLocations)
        {
            saveOriginalLocations = false;
            positions = new Vector3[observer.GetOperators().Count];
            for(int i=0; i<positions.Length; i++)
            {
                positions[i] = observer.GetOperators()[i].GetIcon().transform.position;
            }
        }
        if(loadLocations)
        {
            loadLocations = false;
            for (int i = 0; i < positions.Length; i++)
            {
                observer.GetOperators()[i].GetIcon().transform.position = positions[i];
            }
            if (GetComponent<ConeTreeAlgorithm>().RDT) GetComponent<ConeTreeAlgorithm>().CalculateRDT();
        }
	}

    private void FixedUpdate()
    {
        if (calculateForceDirected)
        {
            frameCounter++;
            if (frameCounter == 2)
            {
                frameCounter = 0;
                if (Temperature < DefaultMinimumTemperature)
                {
                    Temperature = 0.2f;
                    calculateForceDirected = false;
                    GetComponent<TwoDimensionalProjection>().SetPlane();
                    //re-run algorithm if nodes are not placed accodringly
                    ReRunAlgorithm();
                    if (repeatCounter < 5)
                    {
                        calculateForceDirected = true;
                        repeatCounter++;
                    }
                    else
                    {
                        repeatCounter = 0;
                        calculateForceDirected = false;
                    }
                }
                Calculate();
            }
        }
    }

    public override void StartAlgorithm()
    {
        if (GetComponent<LayoutAlgorithm>().currentLayout != this) RandomizePositions();
        //Algorithm is called in Update function
        calculateForceDirected = true;
        GetComponent<LayoutAlgorithm>().currentLayout = this;
    }

    void ReRunAlgorithm()
    {
        foreach (var op1 in observer.GetOperators())
        {
            float distance = 0;
            foreach (var op2 in op1.Children)
            {
                if (op1 == op2) continue;
                distance = Vector3.Distance(op1.GetIcon().transform.position, op2.GetIcon().transform.position);
                if (distance > 10 && !GetTemporal())
                {
                    if(op2.Children != null)
                    {
                        if (op2.Children.Count == 0) StartAlgorithm();
                    }
                    else if(op2.Children.Count == 0) StartAlgorithm();
                }
            }
        }
    }

    // Randomization of node positions
    void RandomizePositions()
    {
        // reset the temperature
        Temperature = DefaultStartingTemperature;
        for(int i=0; i<observer.GetOperators().Count; i++)
        {
            var newPos = new Vector3(observer.GetOperators()[i].GetIcon().GetComponent<IconProperties>().originalPos.x, UnityEngine.Random.Range(0f, 3f), UnityEngine.Random.Range(0f, 3f));
            observer.GetOperators()[i].GetIcon().transform.position = newPos;
            if(observer.GetOperators()[i].Parents != null)
            {
                if (observer.GetOperators()[i].Parents.Count != 0)
                {
                    observer.GetOperators()[i].GetComponent<LineRenderer>().positionCount = 2;
                    observer.GetOperators()[i].GetComponent<LineRenderer>().SetPositions(new Vector3[] {
                                                                                        observer.GetOperators()[i].Parents[0].GetIcon().transform.position,
                                                                                        observer.GetOperators()[i].GetIcon().transform.position});
                }
            }
        }
        GetComponent<TwoDimensionalProjection>().SetPlane();
    }

    /*
     * Allocates nodes in time-bins of 10 seconds
     * Used for time-depending positioning of nodes
     */
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
        if(GetTemporal()) ForceDirectedWithTemporalDependency();
        else ForceDirectedWithoutTemporalDependency();
        //update positions
        foreach (var op in observer.GetOperators())
        {
            op.GetIcon().transform.position += op.GetIcon().GetComponent<IconProperties>().acceleration/2;
            op.GetIcon().GetComponent<IconProperties>().acceleration = Vector3.zero;
            op.GetIcon().GetComponent<IconProperties>().oldPos = op.GetIcon().transform.position;
            if (op.Parents != null)
            {
                if (op.Parents.Count != 0)
                {
                    if(op.GetComponent<LineRenderer>().positionCount == 3)
                    {
                        op.GetComponent<LineRenderer>().positionCount = 2;
                    }
                    op.GetComponent<LineRenderer>().SetPositions(new Vector3[] { op.Parents[0].GetIcon().transform.position, op.GetIcon().transform.position });
                }
            }
        }
        Temperature *= DefaultTemperatureAttenuation;
    }

    /*
     * Time-dependent force-directed algorithm
     * Nodes affect each-other only within specific time-bins of 10 seconds
     * Also, keeping their X position
     */
    private void ForceDirectedWithTemporalDependency()
    {
        AllocateTimeBins();
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

    /*
     * Classical force-directed 3D algorithm where connected nodes attract,
     * and non-connected nodes repel eachother
     */
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
