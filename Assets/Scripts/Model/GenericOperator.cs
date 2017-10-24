using System;
using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;
using System.Collections.Generic;


namespace Assets.Scripts.Model
{
    public abstract class GenericOperator : MonoBehaviour
    {
        public Observer observer;

        public int id = -1;

        public List<GenericOperator> parents; // stores all parents (GenericOperator)
        public List<GenericOperator> children; // stores all children (GenericOperator)

        public GenericVisualization visualization; // stores reference to visualization-object if one exists
        public GenericIcon icon; // stores reference to icon object

        private GenericDatamodel rawInputData;
        private GenericDatamodel outputData;

        public bool properInitializedStart;

        public virtual void Start()
        {
            visualization = GetComponentInChildren<GenericVisualization>();
            icon = GetComponentInChildren<GenericIcon>();

            observer = GameObject.FindObjectOfType<Observer>();
            
            properInitializedStart = true;

            observer.notifyObserverInitComplete(this);
        }

        /**
        * Initializes Operator with parents and input data.
        * */
        public void init(int id, List<GenericOperator> parentsList)
        {
            this.id = id;
            this.parents = parentsList;
            fetchdata();
        }

        /**
        * Processes the data with the respective algorithm and (re-)calculates the corresponding visualization and 
        * icon. If necessary, children are recursively refresehed as well. 
        * Returns true if finished successfully.
        * */
        public abstract bool process();

        /**
        * Refreshes data by collecting and combining data models from all parents.
        * */
        public void fetchdata()
        {
            if (parents == null || parents.Count == 0) return;

            GenericDatamodel unitedDataModel = parents[0].getOutputData();
            for(int i=1; i<parents.Count; i++)
            {
                unitedDataModel = unitedDataModel.mergeDatamodels(parents[i].getOutputData());
            }
            setRawInputData(unitedDataModel);
        }

        /**
        * Deletes recursively all children that do not have any other parent and refreshes all children that 
        * do have other parents. Deletes itself including icon and visualization. 
        * */
        public void delete(GenericOperator op)
        {
            if (op.Equals(this) || parents.Count == 1)
            {
                foreach (GenericOperator parent in parents)
                {
                    parent.removeChild(op);
                }
                foreach (GenericOperator child in children)
                {
                    child.delete(this);
                }
                destroyGenericOperator();
            }
            else
            {
                removeParent(op);
                fetchdata();
                process();
            }
        }

        /**
        * Adds Generic Operator to children
        * */
        protected void addChild(GenericOperator child)
        {
            children.Add(child);
        }

        /**
       * Removes Generic Operator from children
       * */
        protected void removeChild(GenericOperator child)
        {
            if(children.Contains(child))
                children.Remove(child);
        }

        /**
        * Adds Generic Operator to parents
        * */
        protected void addParent(GenericOperator parent)
        {
            parents.Add(parent);

        }

        /**
        * Removes Generic Operator from parents
        * */
        protected void removeParent(GenericOperator parent)
        {
            if(parents.Contains(parent))
                parents.Remove(parent);
        }


        /**
       * Returns the inputData
       * */
        public GenericDatamodel getRawInputData()
        {
            return rawInputData;
        }

        /**
        * Sets the inputData
        * */
        protected void setRawInputData(GenericDatamodel newRawInputData)
        {
            rawInputData = newRawInputData;
        }

        /**
        * Returns the outputData
        * */
        public GenericDatamodel getOutputData()
        {
            return outputData;
        }

        /**
        * Sets the outputData
        * */
        protected void setOutputData(GenericDatamodel newOuputData)
        {
            outputData = newOuputData;
        }

        /**
        * Returns the icon
        * */
        public GenericIcon getIcon()
        {
            return icon;
        }

        /**
        * Returns the visualization
        * */
        public GenericVisualization getVisualization()
        {
            return visualization;
        }


        /**
        * Removes the generic operator from the scene, including visualization and icon
        * */
        private void destroyGenericOperator()
        {
            //TODO notifyObserver about delete of id

            if(visualization.gameObject != null)
                Destroy(visualization.gameObject);
            if (icon.gameObject != null)
                Destroy(icon.gameObject);
            if (gameObject != null)
                Destroy(gameObject);
        }

        public bool checkConsistency()
        {
            return properInitializedStart;
        }

        public int getId()
        {
            return id;
        }
    }

    
}
