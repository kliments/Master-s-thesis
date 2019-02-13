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
    public float DefaultStartingTemperature = 1f;
    
    //Temperature where the simulated annealing should stop
    public float DefaultMinimumTemperature = 0.01f;

    //The ratio between two successive temperatures in the simulated annealing
    public float DefaultTemperatureAttenuation = 0.99f;

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

    //Default algorithm used for temporal force directed
    private DefaultAlgorithm defaultAlg;

    LineRenderer lr = new LineRenderer();
    // Use this for initialization
    void Start () {
        ElectricForce = (d => 1 / (d * d));
        SpringForce = (d => 2 * Math.Log(d));

        // reset the temperature
        Temperature = DefaultStartingTemperature;

        loadLocations = true;

        _timeBins = new List<List<GenericOperator>>();

        defaultAlg = (DefaultAlgorithm)FindObjectOfType(typeof(DefaultAlgorithm));
    }
	
	// Update is called once per frame
	void Update () {
        
        if (calculateForceDirected)
        {
            Calculate();
            if (Temperature < DefaultMinimumTemperature)
            {
                Temperature = 0.2f;
                GetComponent<TwoDimensionalProjection>().SetPlane();
                if (!ReRunAlgorithm())
                {
                    calculateForceDirected = false;
                    //set flag that this algorithm has finished
                    SetFinish();
                }
            }
        }
    }

    public override void StartAlgorithm()
    {
        //check if another algorithm is running
        if (!GetComponent<LayoutAlgorithm>().currentLayout.AlgorithmHasFinished()) return;

        if (GetComponent<LayoutAlgorithm>().currentLayout != this)
        {
            if (GetTemporal()) defaultAlg.StartAlgorithm();
            GetComponent<LayoutAlgorithm>().currentLayout = this;
            RandomizePositions();
        }
        //set flag that this algorithm has started
        SetStart();
        //Algorithm is called in Update function
        calculateForceDirected = true;
    }

    bool ReRunAlgorithm()
    {
        float maxDistance = 0;
        foreach(var op in observer.GetOperators())
        {
            if (Vector3.Distance(op.GetIcon().GetComponent<IconProperties>().previousPosition, op.GetIcon().GetComponent<IconProperties>().newPos) > maxDistance)
            {
                maxDistance = Vector3.Distance(op.GetIcon().GetComponent<IconProperties>().previousPosition, op.GetIcon().GetComponent<IconProperties>().newPos);
            }
        }
        if(maxDistance > 0.01f)
        {
            return true;
        }
        Debug.Log(maxDistance);
        return false;
    }

    // Randomization of node positions
    void RandomizePositions()
    {
        Vector3 pos = new Vector3();
        Vector3 pos2 = new Vector3();
        float xRange = 1.5f;
        if(GetTemporal())
        {
            xRange = 0;
        }
        // reset the temperature
        Temperature = DefaultStartingTemperature;
        for(int i=0; i<observer.GetOperators().Count; i++)
        {
            pos = new Vector3(observer.GetOperators()[i].GetIcon().GetComponent<IconProperties>().newPos.x, UnityEngine.Random.Range(0.5f, 1.5f), UnityEngine.Random.Range(-1.5f, 1.5f));
            pos2 = observer.GetOperators()[i].GetIcon().GetComponent<IconProperties>().newPos;
            observer.GetOperators()[i].GetIcon().GetComponent<IconProperties>().newPos = pos;
            pos += new Vector3(UnityEngine.Random.Range(-xRange, xRange), 0, 0);
            observer.GetOperators()[i].GetIcon().GetComponent<IconProperties>().newPos = pos;
            pos2 = observer.GetOperators()[i].GetIcon().GetComponent<IconProperties>().newPos;
            if (observer.GetOperators()[i].Parents != null)
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
        IconProperties np = new IconProperties();
        Vector3 test = new Vector3();
        if(GetTemporal()) ForceDirectedWithTemporalDependency();
        else ForceDirectedWithoutTemporalDependency();
        //update positions
        foreach (var op in observer.GetOperators())
        {
            test = op.GetIcon().GetComponent<IconProperties>().acceleration / 2;
            np = op.GetIcon().GetComponent<IconProperties>();
            op.GetIcon().GetComponent<IconProperties>().newPos += op.GetIcon().GetComponent<IconProperties>().acceleration / 2;
            op.GetIcon().GetComponent<IconProperties>().repos = true;
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
                //Vector3 pos1 = node1.GetIcon().transform.position;
                Vector3 pos1 = node1.GetIcon().GetComponent<IconProperties>().newPos;
                double xForce = 0, yForce = 0, zForce = 0;

                //attraction forces towards children
                if (node1.Children != null)
                {
                    foreach (var child in node1.Children)
                    {
                        Vector3 pos2 = child.GetIcon().GetComponent<IconProperties>().newPos;
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
                        Vector3 pos2 = parent.GetIcon().GetComponent<IconProperties>().newPos;
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
                    //Vector3 pos2 = node2.GetIcon().transform.position;
                    Vector3 pos2 = node2.GetIcon().GetComponent<IconProperties>().newPos;
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
            //Vector3 pos1 = op1.GetIcon().transform.position;
            Vector3 pos1 = op1.GetIcon().GetComponent<IconProperties>().newPos;
            double xForce = 0, yForce = 0, zForce = 0;

            //attraction forces towards children
            if (op1.Children != null)
            {
                foreach (var child in op1.Children)
                {
                    //Vector3 pos2 = child.GetIcon().transform.position;
                    Vector3 pos2 = child.GetIcon().GetComponent<IconProperties>().newPos;
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
                    //Vector3 pos2 = parent.GetIcon().transform.position;
                    Vector3 pos2 = parent.GetIcon().GetComponent<IconProperties>().newPos;
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
                //Vector3 pos2 = op2.GetIcon().transform.position;
                Vector3 pos2 = op2.GetIcon().GetComponent<IconProperties>().newPos;
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

    bool AllNodesInPlace()
    {
        foreach(var op in observer.GetOperators())
        {
            if (op.GetIcon().GetComponent<IconProperties>().repos) return false;
        }
        return true;
    }

    void OnEnable()
    {
        subscriber.addListener(this);
    }
}
