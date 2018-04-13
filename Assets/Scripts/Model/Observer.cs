using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Model
{
    public class Observer : MonoBehaviour
    {
        private readonly List<GameObject> _operatorPrefabs = new List<GameObject>();
        public static List<GenericOperator> _operators = new List<GenericOperator>(); //was just private, changed because of "HighlightOperator"
        private int _currentId = 1;
        private int _operatorNewId = -1;

        private GraphSpaceController _graphSpaceController;
        private VisualizationSpaceController _visualizationSpaceController;

        public delegate void NewOperatorInitializedAndRunnning(GenericOperator genericOperator);
        public event NewOperatorInitializedAndRunnning NewOperatorInitializedAndRunnningEvent;

        // Use this for initialization
        private void Start()
        {
            var prefabs = Resources.LoadAll<GameObject>("Operators");
            foreach (var prefab in prefabs)
            {
                var lo = prefab;
                if (prefab.GetComponent<GenericOperator>())
                {
                    _operatorPrefabs.Add(lo);

                    // store the id of "new operator" to spawn first element
                    if (lo.CompareTag("Operator_New")) _operatorNewId = _operatorPrefabs.Count - 1;
                }
            }

            _graphSpaceController = GameObject.Find("GraphSpace").GetComponent<GraphSpaceController>();
            _visualizationSpaceController =
                GameObject.Find("VisualizationSpace").GetComponent<VisualizationSpaceController>();
            
            // Spawn initial "New Operator"
            CreateOperator(_operatorNewId);
        }

        public GameObject CreateOperator(GameObject operatorPrefab, List<GenericOperator> parents = null)
        {
            var go = Instantiate(operatorPrefab);
            go.transform.parent = transform;
            var genericOperator = go.GetComponent<GenericOperator>();
            if (genericOperator == null)
            {
                // no operator Prefab ! 
                Destroy(go);
                return null;
            }
            _operators.Add(genericOperator);

            genericOperator.Init(RequestId(), parents);
            
            return go;
        }

        public void CreateOperator(int id, List<GenericOperator> parents = null)
        {
            if (id < 0 || id >= _operatorPrefabs.Count) return;

            CreateOperator(_operatorPrefabs[id], parents);
        }

        public void DestroyOperator(GenericOperator operatorInstance)
        {
            _operators.Remove(operatorInstance);
            operatorInstance.DestroyGenericOperator();
        }

        public void DestroyOperator(int id)
        {
            if (id < 0 || id >= _operators.Count) return;

            DestroyOperator(_operators[id]);
        }


        public void notifyObserverOperatorInitComplete(GenericOperator genericOperator)
        {
            if (!genericOperator.CheckConsistency())
                throw new InvalidProgramException(
                    "base.Start() etc. methods needs to be called in respective inherited methods");

            InstallComponents(genericOperator);

            genericOperator.Process();

            // Emit Event after the new Operator has been initialized and the Process() function has been started
            if(NewOperatorInitializedAndRunnningEvent != null) NewOperatorInitializedAndRunnningEvent(genericOperator);

            // spawn a new NewOperator for newly initialized operator
            if (genericOperator.GetComponent<NewOperator>() == null)
            {
                List<GenericOperator> parent = new List<GenericOperator>();
                parent.Add(genericOperator);
                CreateOperator(_operatorPrefabs[_operatorNewId], parent);
            }
            else
            {
                if (genericOperator.Parents != null)
                {
                    genericOperator.GetIcon().transform.position = genericOperator.Parents[0].GetIcon().transform.position + new Vector3(1, 0, 0);
                }
            }
                
        }


        private void InstallComponents(GenericOperator op)
        {
            if (op.GetIcon() != null)
            {
                _graphSpaceController.InstallNewIcon(op);
            }

            if (op.GetVisualization() != null)
            {
                _visualizationSpaceController.InstallNewVisualization(op);
            }
        }

        private int RequestId()
        {
            return _currentId++;
        }

        public VisualizationSpaceController GetVisualizationSpaceController()
        {
            return _visualizationSpaceController;
        }

        public GraphSpaceController GetGraphSpaceController()
        {
            return _graphSpaceController;
        }

        public List<GameObject> GetOperatorPrefabs()
        {
            return _operatorPrefabs;
        }


    }
}
