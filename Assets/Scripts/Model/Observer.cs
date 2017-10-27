using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Model
{
    public class Observer : MonoBehaviour
    {
        private List<GameObject> operatorPrefabs = new List<GameObject>();
        private List<GenericOperator> operators = new List<GenericOperator>();
        private int currentID = 1;
        private int operatorNewId = -1;

        private GraphSpaceController graphSpaceController;
        private VisualizationSpaceController visualizationSpaceController;
    
        
        // Use this for initialization
        void Start ()
        {
            Object[] prefabs = Resources.LoadAll<GameObject>("Operators");
            foreach (GameObject prefab in prefabs)
            {
                GameObject lo = (GameObject)prefab;
                if (prefab.GetComponent<GenericOperator>())
                {
                    operatorPrefabs.Add(lo);
                    if (lo.tag == "Operator_New") operatorNewId = operatorPrefabs.Count - 1;
                }
            }

            graphSpaceController = GameObject.Find("GraphSpace").GetComponent<GraphSpaceController>();
            visualizationSpaceController = GameObject.Find("VisualizationSpace").GetComponent<VisualizationSpaceController>();

            //test
            createOperator(operatorNewId);

            createOperator(0);
            
        }
	
        // Update is called once per frame

        public bool spawn = false;
        void Update () {
            if (spawn)
            {
                List<GenericOperator> l = new List<GenericOperator>();
                l.Add(operators[1]);

                createOperator(2,l);
                spawn = false;
            }
        }

        public void createOperator(int id, List<GenericOperator> parents = null)
        {
            if (id < 0 || id >= operatorPrefabs.Count) return;

            GameObject go = GameObject.Instantiate(operatorPrefabs[id]);
            go.transform.parent = transform;
            GenericOperator genericOperator = go.GetComponent<GenericOperator>();
            operators.Add(genericOperator);
            
            genericOperator.init(requestID(), parents);
        }

        public void notifyObserverInitComplete(GenericOperator genericOperator)
        {
            if (!genericOperator.checkConsistency()) throw new InvalidProgramException("base.Start() etc. methods needs to be called in respective inherited methods");

            installComponents(genericOperator);
            
            genericOperator.process();
        }

        
        private void installComponents(GenericOperator op)
        {
            if (op.getIcon() != null)
            {
                graphSpaceController.installNewIcon(op);
            }

            if (op.getVisualization() != null)
            {
                visualizationSpaceController.installNewVisualization(op);
            }
        }


        
        private int requestID()
        {
            return currentID++;
        }

        public VisualizationSpaceController getVisualizationSpaceController()
        {
            return visualizationSpaceController;
        }

        public GraphSpaceController getGraphSpaceController()
        {
            return graphSpaceController;
    }
    }
}
