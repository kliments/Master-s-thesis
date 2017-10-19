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

        public GameObject graphSpace;
        public GameObject visualizationSpace;
    
        
        // Use this for initialization
        void Start ()
        {
            Object[] prefabs = Resources.LoadAll<GameObject>("Operators");
            foreach (GameObject prefab in prefabs)
            {
                GameObject lo = (GameObject)prefab;
                if(prefab.GetComponent<GenericOperator>())
                    operatorPrefabs.Add(lo);
            }

            graphSpace = GameObject.Find("GraphSpace");
            visualizationSpace = GameObject.Find("VisualizationSpace");

            //test
            newOperator(0);

            
        }
	
        // Update is called once per frame

        public bool spawn = false;
        void Update () {
            if (spawn)
            {
                List<GenericOperator> l = new List<GenericOperator>();
                l.Add(operators[0]);

                newOperator(1,l);
                spawn = false;
            }
        }

        public void newOperator(int id, List<GenericOperator> parents = null)
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

            installIcon(genericOperator);
            installVis(genericOperator);
            
            genericOperator.process();
        }

        private void installIcon(GenericOperator op)
        {
            if (op.getIcon() == null) return;

            op.getIcon().gameObject.transform.parent = graphSpace.transform;
        }

        private void installVis(GenericOperator op)
        {
            if (op.getVisualization() == null) return;

            op.getVisualization().gameObject.transform.parent = visualizationSpace.transform;
        }

        private int requestID()
        {
            return currentID++;
        }
    }
}
