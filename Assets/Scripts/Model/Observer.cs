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
        private List<GenericOperator> _operators = new List<GenericOperator>();
        private int _currentId = 1;
        private int _operatorNewId = -1;

        private GraphSpaceController _graphSpaceController;
        private VisualizationSpaceController _visualizationSpaceController;


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

        public void CreateOperator(int id, List<GenericOperator> parents = null)
        {
            if (id < 0 || id >= _operatorPrefabs.Count) return;

            var go = Instantiate(_operatorPrefabs[id]);
            go.transform.parent = transform;
            var genericOperator = go.GetComponent<GenericOperator>();
            _operators.Add(genericOperator);

            genericOperator.Init(RequestId(), parents);
        }

        public void NotifyObserverInitComplete(GenericOperator genericOperator)
        {
            if (!genericOperator.CheckConsistency())
                throw new InvalidProgramException(
                    "base.Start() etc. methods needs to be called in respective inherited methods");

            InstallComponents(genericOperator);

            genericOperator.Process();
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
