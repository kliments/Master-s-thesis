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
        private int _currentId = 1;
        private int _operatorNewId = -1;

        private GraphSpaceController graphSpaceController;
        private VisualizationSpaceController visualizationSpaceController;


        // Use this for initialization
        void Start()
        {
            Object[] prefabs = Resources.LoadAll<GameObject>("Operators");
            foreach (GameObject prefab in prefabs)
            {
                GameObject lo = (GameObject) prefab;
                if (prefab.GetComponent<GenericOperator>())
                {
                    operatorPrefabs.Add(lo);

                    // store the id of "new operator" to spawn first element
                    if (lo.CompareTag("Operator_New")) _operatorNewId = operatorPrefabs.Count - 1;
                }
            }

            graphSpaceController = GameObject.Find("GraphSpace").GetComponent<GraphSpaceController>();
            visualizationSpaceController =
                GameObject.Find("VisualizationSpace").GetComponent<VisualizationSpaceController>();
            
            // Spawn initial "New Operator"
            CreateOperator(_operatorNewId);
        }

        public void CreateOperator(int id, List<GenericOperator> parents = null)
        {
            if (id < 0 || id >= operatorPrefabs.Count) return;

            GameObject go = GameObject.Instantiate(operatorPrefabs[id]);
            go.transform.parent = transform;
            GenericOperator genericOperator = go.GetComponent<GenericOperator>();
            operators.Add(genericOperator);

            genericOperator.init(RequestId(), parents);
        }

        public void NotifyObserverInitComplete(GenericOperator genericOperator)
        {
            if (!genericOperator.checkConsistency())
                throw new InvalidProgramException(
                    "base.Start() etc. methods needs to be called in respective inherited methods");

            InstallComponents(genericOperator);

            genericOperator.process();
        }


        private void InstallComponents(GenericOperator op)
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



        private int RequestId()
        {
            return _currentId++;
        }

        public VisualizationSpaceController GetVisualizationSpaceController()
        {
            return visualizationSpaceController;
        }

        public GraphSpaceController GetGraphSpaceController()
        {
            return graphSpaceController;
        }

        public List<GameObject> GetOperatorPrefabs()
        {
            return operatorPrefabs;
        }


    }
}
