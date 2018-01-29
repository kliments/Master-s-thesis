using System;
using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;
using System.Collections.Generic;


namespace Assets.Scripts.Model
{
    public abstract class GenericOperator : MonoBehaviour
    {
        public Observer Observer;

        public int Id = -1;
        public int PlacementCounter = 0;

        public List<GenericOperator> Parents; // stores all parents (GenericOperator)
        public List<GenericOperator> Children; // stores all children (GenericOperator)

        public GenericVisualization Visualization; // stores reference to visualization-object if one exists
        public GenericIcon Icon; // stores reference to icon object

        private GenericDatamodel _rawInputData;
        private GenericDatamodel _outputData;

        public bool ProperInitializedStart;

        public virtual void Start()
        {
            Visualization = GetComponentInChildren<GenericVisualization>();
            Icon = GetComponentInChildren<GenericIcon>();

            Observer = FindObjectOfType<Observer>();
            
            ProperInitializedStart = true;

            Observer.NotifyObserverInitComplete(this);
        }

        /**
        * Initializes Operator with parents and input data.
        * */
        public void Init(int id, List<GenericOperator> parentsList)
        {
            Id = id;
            Parents = parentsList;
            foreach(GenericOperator op in parentsList)
            {
                op.AddChild(this);
            }
            Fetchdata();
        }

        /**
        * Processes the data with the respective algorithm and (re-)calculates the corresponding visualization and 
        * icon. If necessary, children are recursively refresehed as well. 
        * Returns true if finished successfully.
        * */
        public abstract bool Process();

        /**
        * Refreshes data by collecting and combining data models from all parents.
        * */
        public void Fetchdata()
        {
            if (Parents == null || Parents.Count == 0) return;

            var unitedDataModel = Parents[0].GetOutputData();
            for(var i=1; i<Parents.Count; i++)
            {
                unitedDataModel = unitedDataModel.MergeDatamodels(Parents[i].GetOutputData());
            }
            SetRawInputData(unitedDataModel);
        }

        /**
        * Deletes recursively all children that do not have any other parent and refreshes all children that 
        * do have other parents. Deletes itself including icon and visualization. 
        * */
        public void Delete(GenericOperator op)
        {
            if (op.Equals(this) || Parents.Count == 1)
            {
                foreach (var parent in Parents)
                {
                    parent.RemoveChild(op);
                }
                foreach (var child in Children)
                {
                    child.Delete(this);
                }
                DestroyGenericOperator();
            }
            else
            {
                RemoveParent(op);
                Fetchdata();
                Process();
            }
        }

        /**
        * Adds Generic Operator to children
        * */
        protected void AddChild(GenericOperator child)
        {
            Children.Add(child);
        }

        /**
       * Removes Generic Operator from children
       * */
        protected void RemoveChild(GenericOperator child)
        {
            if(Children.Contains(child))
                Children.Remove(child);
        }

        /**
        * Adds Generic Operator to parents
        * */
        protected void AddParent(GenericOperator parent)
        {
            Parents.Add(parent);

        }

        /**
        * Removes Generic Operator from parents
        * */
        protected void RemoveParent(GenericOperator parent)
        {
            if(Parents.Contains(parent))
                Parents.Remove(parent);
        }


        /**
       * Returns the inputData
       * */
        public GenericDatamodel GetRawInputData()
        {
            return _rawInputData;
        }

        /**
        * Sets the inputData
        * */
        protected void SetRawInputData(GenericDatamodel newRawInputData)
        {
            _rawInputData = newRawInputData;
        }

        /**
        * Returns the outputData
        * */
        public GenericDatamodel GetOutputData()
        {
            return _outputData;
        }

        /**
        * Sets the outputData
        * */
        protected void SetOutputData(GenericDatamodel newOuputData)
        {
            _outputData = newOuputData;
        }

        /**
        * Returns the icon
        * */
        public GenericIcon GetIcon()
        {
            return Icon;
        }

        /**
        * Returns the visualization
        * */
        public GenericVisualization GetVisualization()
        {
            return Visualization;
        }


        /**
        * Removes the generic operator from the scene, including visualization and icon
        * */
        private void DestroyGenericOperator()
        {
            //TODO notifyObserver about delete of id

            if(Visualization.gameObject != null)
                Destroy(Visualization.gameObject);
            if (Icon.gameObject != null)
                Destroy(Icon.gameObject);
            if (gameObject != null)
                Destroy(gameObject);
        }

        public bool CheckConsistency()
        {
            return ProperInitializedStart;
        }

        public int GetId()
        {
            return Id;
        }

        public abstract bool ValidateIfOperatorPossibleForParents(GenericOperator parent);



    }

    
}
