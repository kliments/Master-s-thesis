using System;
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

        public virtual void Start()
        {



            Visualization = GetComponentInChildren<GenericVisualization>();
            Icon = GetComponentInChildren<GenericIcon>();

            Observer = FindObjectOfType<Observer>();



            GameObject iconHighlight = Resources.Load<GameObject>("Highlights/IconHighlight");
            isSelectedHighlighting = Instantiate(iconHighlight);
            isSelectedHighlighting.transform.parent = Icon.transform;
            isSelectedHighlighting.transform.position = Icon.transform.position;
            isSelectedHighlighting.SetActive(false);



            ProperInitializedStart = true;

            Observer.notifyObserverOperatorInitComplete(this);

           
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
        protected void SetOutputData(GenericDatamodel newOuputData)
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

        public void StoreData()
        {
            if (this == null) return;
            data.name = gameObject.name.Replace("(Clone)", "");
            data.posX = GetIcon().transform.position.x;
            data.posY = GetIcon().transform.position.y;
            data.posZ = GetIcon().transform.position.z;
        }

        public void LoadData()
        {
            Vector3 pos = new Vector3(data.posX, data.posY, data.posZ);
            GetIcon().transform.position = pos;
        }

        private void OnEnable()
        {
            SaveLoadData.OnLoaded += delegate { LoadData(); };
            SaveLoadData.OnBeforeSave += delegate { StoreData(); };
            SaveLoadData.OnBeforeSave += delegate { SaveLoadData.AddOperatorData(data); };
        }
        private void OnDisable()
        {
            SaveLoadData.OnLoaded -= delegate { LoadData(); };
            SaveLoadData.OnBeforeSave -= delegate { StoreData(); };
            SaveLoadData.OnBeforeSave -= delegate { SaveLoadData.AddOperatorData(data); };
        }
    }

    public class OperatorData
    {
        [XmlAttribute("Name")]
        public string name;

        [XmlElement("PosX")]
        public float posX;
        [XmlElement("PosY")]
        public float posY;
        [XmlElement("PosZ")]
        public float posZ;

    }


}
