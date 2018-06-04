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
        public GenericOperator selectedOperator;

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
            _graphSpaceController.setObserver(this);
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

            if (parents != null)
            {
                foreach (GenericOperator parent in parents)
                {
                    parent.Children.Add(genericOperator);
                }
            }
            
            return go;
        }

        public void CreateOperator(int id, List<GenericOperator> parents = null)
        {
            if (id < 0 || id >= _operatorPrefabs.Count) return;

            CreateOperator(_operatorPrefabs[id], parents);
        }

        public void DestroyOperator(GenericOperator operatorInstance)
        {
            if (operatorInstance == null) return;
            List<GenericOperator> parents = operatorInstance.Parents;
            if (parents != null)
            {
                foreach (GenericOperator parent in parents)
                {
                    parent.Children.Remove(operatorInstance);
                }
            }
          
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

            genericOperator.processComplete = genericOperator.Process();

            // Emit Event after the new Operator has been initialized and the Process() function has been started
            if(NewOperatorInitializedAndRunnningEvent != null) NewOperatorInitializedAndRunnningEvent(genericOperator);

            
            _graphSpaceController.moveToSpawnPosition(genericOperator);

        }

        public void selectOperator(GenericOperator go)
        {
            if (selectedOperator == go || go.GetType().Equals((typeof(NewOperator)))) return;

            if(selectedOperator != null) selectedOperator.setSelected(false);
            go.setSelected(true);

            _visualizationSpaceController.InstallVisualization(go);

            selectedOperator = go;
        }


        private void InstallComponents(GenericOperator op)
        {
            if (op.GetIcon() != null)
            {
                _graphSpaceController.InstallNewIcon(op);
            }

            if (!op.GetType().Equals((typeof(NewOperator))))
            {
                selectOperator(op);
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

        public GenericOperator getOperatorByID(int id)
        {
            foreach (GenericOperator op in _operators)
            {
                if (op.Id == id) return op;
            }
            return null;
        }

        public GenericOperator spawnNewOperator(GenericOperator op)
        {
            List<GenericOperator> parent = new List<GenericOperator>();
            parent.Add(op);
            GameObject ob = CreateOperator(_operatorPrefabs[_operatorNewId], parent);
            GenericOperator go = ob.GetComponent<GenericOperator>();
       
            
            return go;
        }

        public List<GenericOperator> GetOperators()
        {
            return _operators;
        }


    }
}
