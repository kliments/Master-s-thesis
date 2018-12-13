﻿using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Controller.Interaction.Icon;
using UnityEditor;
using Model.Operators;
using System.Xml.Serialization;

namespace Assets.Scripts.Model
{
    public abstract class GenericOperator : MonoBehaviour
    {
        public Observer Observer;

        public int Id = -1;

        public List<GenericOperator> Parents; // stores all parents (GenericOperator)
        public List<GenericOperator> Children; // stores all children (GenericOperator)

        public GenericVisualization Visualization; // stores reference to visualization-object if one exists
        public GenericIcon Icon; // stores reference to icon object

        public GenericDatamodel _rawInputData;
        public GenericDatamodel _outputData;

        public bool hasInput = false;
        public bool hasOutput = false;

        public bool ProperInitializedStart;
        public bool processComplete;

        public bool isSelected = false;
        public GenericOperator newOperator;

        public GameObject isSelectedHighlighting;

        public OperatorData data = new OperatorData();

        private SaveLoadData.SerializeAction storeDataAction;
        private SaveLoadData.SerializeAction saveDataAction;

        public double timeStamp, normalizedTimeStamp, hour, minute, second, millisecond;
        public DateTime timeOfCreation, timeNow;

        private GraphSpaceController _graphSpace;
        private Vector3 _oldParentPos, _newParentPos, _oldPos, _newPos;

        private LayoutAlgorithm layout;
        //Default algorithm for saving original positions of nodes
        private DefaultAlgorithm defaultAlgorithm;
        
        public virtual void Start()
        {

            _graphSpace = GameObject.Find("GraphSpace").GetComponent<GraphSpaceController>();

            Visualization = GetComponentInChildren<GenericVisualization>();
            Icon = GetComponentInChildren<GenericIcon>();

            //properties for Layout algorithms
            Icon.gameObject.AddComponent<IconProperties>();
            Observer = FindObjectOfType<Observer>();
            
            GameObject iconHighlight = Resources.Load<GameObject>("Highlights/IconHighlight");
            isSelectedHighlighting = Instantiate(iconHighlight);
            isSelectedHighlighting.transform.parent = Icon.transform;
            isSelectedHighlighting.transform.position = Icon.transform.position;
            isSelectedHighlighting.SetActive(false);
            
            ProperInitializedStart = true;

            Observer.notifyObserverOperatorInitComplete(this);
            
            //set acceleration to 0, for Force-Directed Algorithm
            Icon.GetComponent<IconProperties>().acceleration = new Vector3(0, 0, 0);
            //increase the depth of the node
            if(Parents != null)
            {
                if(Parents.Count > 0) Icon.GetComponent<IconProperties>().depth = Parents[0].GetIcon().GetComponent<IconProperties>().depth + 1;
                //only root node has depth 1
                else Icon.GetComponent<IconProperties>().depth = 1;
            }
            //only root node has depth 1
            else Icon.GetComponent<IconProperties>().depth = 1;

            //time of creation of the operator, later used as temporal variable in visualization of the tree
            if (hour == 0 && minute == 0 && second == 0 && millisecond == 0)
            {
                timeNow = DateTime.Now;
                timeOfCreation = timeNow;
                if(Observer.GetOperators().Count > 1)
                {
                    timeOfCreation = Observer.GetOperators()[Observer.GetOperators().Count - 2].timeOfCreation.
                        AddMilliseconds((timeNow - Observer.GetOperators()[Observer.GetOperators().Count - 2].timeNow).TotalMilliseconds);
                }
                timeStamp = (double)timeOfCreation.Hour + (double)timeOfCreation.Minute/60 + (double)timeOfCreation.Second/3600 + (double)timeOfCreation.Millisecond / 3600000;
                hour = timeOfCreation.Hour;
                minute = timeOfCreation.Minute;
                second = timeOfCreation.Second;
                millisecond = timeOfCreation.Millisecond;
            }
            else
            {
                timeStamp = hour + minute / 60 + second / 3600 + millisecond / 3600000;
                DateTime x = DateTime.Today;
                timeOfCreation = new DateTime(x.Year, x.Month, x.Day).AddHours(hour).AddMinutes(minute).AddSeconds(second).AddMilliseconds(millisecond);
                timeNow = DateTime.Now;
            }
            _oldPos = new Vector3();
            _newPos = new Vector3();
            _oldParentPos = new Vector3();
            _newParentPos = new Vector3();

            // Reload current layout algorithm when Operator is created
            if(GetType() != typeof(NewOperator))
            {
                //save the default position of node
                defaultAlgorithm = (DefaultAlgorithm)FindObjectOfType(typeof(DefaultAlgorithm));
                defaultAlgorithm.positions.Add(GetIcon().transform.position);
                //re-run current layout algorithm
                layout = (LayoutAlgorithm)FindObjectOfType(typeof(LayoutAlgorithm));
                if (layout.currentLayout != null) layout.currentLayout.StartAlgorithm();
            }
        }

        private void Update()
        {
            if(Parents != null)
            {
                if(Parents.Count != 0)
                {
                    _oldPos = _newPos;
                    _newPos = GetIcon().transform.position;
                    _oldParentPos = _newParentPos;
                    _newParentPos = Parents[0].GetIcon().transform.position;
                    if(GetComponent<LineRenderer>().positionCount == 2)
                    {
                        // Update the line renderer if position of this node changes
                        if (_oldPos != _newPos)
                        {
                            GetComponent<LineRenderer>().SetPositions(new Vector3[] { _newParentPos, GetIcon().transform.position });
                        }
                        // Update the line renderer if position of parent changes
                        if (_oldParentPos != _newParentPos)
                        {
                            GetComponent<LineRenderer>().SetPositions(new Vector3[] { _newParentPos, GetIcon().transform.position });
                        }
                    }
                    else if(GetComponent<LineRenderer>().positionCount == 3)
                    {// Update the line renderer if position of this node changes
                        if (_oldPos != _newPos)
                        {
                            GetComponent<LineRenderer>().SetPositions(new Vector3[] { _newParentPos, Parents[0].GetIcon().GetComponent<IconProperties>().refPoint , GetIcon().transform.position });
                        }
                        // Update the line renderer if position of parent changes
                        if (_oldParentPos != _newParentPos)
                        {
                            GetComponent<LineRenderer>().SetPositions(new Vector3[] { _newParentPos, Parents[0].GetIcon().GetComponent<IconProperties>().refPoint, GetIcon().transform.position });
                        }
                    }
                }
            }
        }

        /**
        * Initializes Operator with parents and input data.
        * */
        public void Init(int id, List<GenericOperator> parentsList)
        {
            Id = id;
            Parents = parentsList;
            Fetchdata();
        }

        /**
        * Processes the data with the respective algorithm and (re-)calculates the corresponding visualization and 
        * icon. If necessary, children are recursively refresehed as well. 
        * Returns true if finished successfully.
        * */
        public abstract bool Process();


        /**
        * Reprocesses the data, parsing the updated data from the parent node to its children recursively
        * */
        public void ReProcess(GenericDatamodel datamodel)
        {
            if(!GetType().Equals(typeof(DataloaderOperator)) || !GetType().Equals(typeof(DatageneratorOperator))) SetRawInputData(datamodel);
            Process();
            
             //it is necessary not to call SplitdatasetOperator, because it's children Reprocess function is already called within
             //calling it twice, overwrites the data
             
            if(GetType()!=typeof(SplitDatasetOperator))
            {
                foreach (var child in Children)
                {
                    child.ReProcess(GetOutputData());
                }
            }
        }

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
                if(Parents!=null)
                {
                    foreach (var parent in Parents)
                    {
                        parent.RemoveChild(op);
                    }
                }
                if(Children != null)
                {
                    foreach (var child in Children)
                    {
                        child.Delete(this);
                    }
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
        public void SetRawInputData(GenericDatamodel newRawInputData)
        {
            hasInput = (newRawInputData != null);
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
        public void SetOutputData(GenericDatamodel newOuputData)
        {
            hasOutput = (newOuputData != null);
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
        public void DestroyGenericOperator()
        {
            //TODO notifyObserver about delete of id

            if(Visualization != null && Visualization.gameObject != null)
                Destroy(Visualization.gameObject);
            if (Icon != null && Icon.gameObject != null)
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

        

        public void setSelected(bool selected)
        {
            isSelected = selected;

            if (selected)
            {
                OnSelectAction();

                isSelectedHighlighting.SetActive(true);

                // spawn a new NewOperator for newly initialized operator
                if (!this.GetType().Equals((typeof(NewOperator))) && !this.GetType().Equals((typeof(SplitDatasetOperator))))
                {
                    StartCoroutine(spawnNewOperatorAfterNewlyCreatedOperatorHasFinishedProcess());
                }
            }
            else
            {
                OnUnselectAction();

                isSelectedHighlighting.SetActive(false);

                if (newOperator != null) newOperator.Icon.GetComponentInChildren<NewIconInteractionController>().hideOptions();
                Observer.DestroyOperator(newOperator);
            }
        }

        private IEnumerator spawnNewOperatorAfterNewlyCreatedOperatorHasFinishedProcess()
        {
           while (processComplete == false)
           {
                yield return new WaitForFixedUpdate();
            }
            newOperator = Observer.spawnNewOperator(this);
        }

        public abstract bool ValidateIfOperatorPossibleForParents(GenericOperator parent);

        protected virtual void OnUnselectAction() { }
        protected virtual void OnSelectAction() { }

        public abstract void StoreData();

        public abstract void LoadSpecificData(OperatorData data);

        private void OnEnable()
        {
            if (GetType().Equals(typeof(NewOperator))) return;
            storeDataAction = delegate { StoreData(); };
            saveDataAction = delegate { SaveLoadData.AddOperatorData(data); };
            SaveLoadData.OnBeforeSave += storeDataAction;
            SaveLoadData.OnBeforeSave += saveDataAction;
        }
        private void OnDisable()
        {
            if (GetType().Equals(typeof(NewOperator))) return;
            SaveLoadData.OnBeforeSave -= storeDataAction;
            SaveLoadData.OnBeforeSave -= saveDataAction;
        }

        public void Disable()
        {
            if (GetType().Equals(typeof(NewOperator))) return;
            SaveLoadData.OnBeforeSave -= storeDataAction;
            SaveLoadData.OnBeforeSave -= saveDataAction;
        }
    }

    /*
     * Generic class that contains the generic data such as ID, parentID, x,y,z positions and CustomOperatorData
     * If added CustomOperatorData then the derived class must be included using XmlInclude below
     */
    [Serializable]
    //Add derived classes here
    [XmlInclude(typeof(SplitDatasetOperator.CustomSplitData))]
    public class OperatorData
    {
        [XmlAttribute("Name")]
        public string name;

        [XmlElement("ID")]
        public int ID;
        [XmlElement("Parent")]
        public int parent;
        [XmlElement("PosX")]
        public float posX;
        [XmlElement("PosY")]
        public float posY;
        [XmlElement("PosZ")]
        public float posZ;
        [XmlElement("Hour")]
        public double hour;
        [XmlElement("Minute")]
        public double minute;
        [XmlElement("Second")]
        public double second;
        [XmlElement("MilliSecond")]
        public double ms;

        public CustomOperatorData customData;
    }

    public abstract class CustomOperatorData
    {

    }
}
