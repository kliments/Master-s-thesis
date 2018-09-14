using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
using System;

namespace Model.Operators
{
    public class SplitDatasetOperator : GenericOperator
    {
        public float _yThreshold = 0.5f;
        public bool press = false;

        private Observer _observer;
        private List<DataItem> _dataItems;
        private SimpleDatamodel[] _simpleDataModel;
        private List<GenericOperator> _parents;
        private int _counter = 0, _datasets = 2;
        private GameObject obj1, obj2;
        // Use this for initialization
        public override void Start()
        {
            base.Start();
            _observer = (Observer)(FindObjectOfType(typeof(Observer)));
            _dataItems = _rawInputData.GetDataItems();
            _simpleDataModel = new SimpleDatamodel[_datasets];
            _parents = new List<GenericOperator>();
            _parents.Add(this);

            SplitDataset();
        }

        private void Update()
        {
            //for debugging
            if(press)
            {
                press = false;
                for(int i=0;i<Children.Count;i++)
                {
                    Children[i].Delete(Children[i]);
                    --i;
                }
                SplitDataset();
            }
        }

        public override bool Process()
        {
            return true;
        }

        public override bool ValidateIfOperatorPossibleForParents(GenericOperator parent)
        {
            return true;
        }

        public void SplitDataset()
        {
            for (int i = 0; i < _simpleDataModel.Length; i++)
            {
                _simpleDataModel[i] = new SimpleDatamodel();
            }
            foreach (var dataItem in _dataItems)
            {
                if (dataItem.GetfirstThreeNumericColsAsVector().y >= _yThreshold)
                {
                    _simpleDataModel[0].Add(dataItem);
                }
                else
                {
                    _simpleDataModel[1].Add(dataItem);
                }
            }
            //create operators
            for(int i=0; i<_simpleDataModel.Length; i++)
            {
                CreateOperators(_simpleDataModel[i]);
            }
            SetOutputData(GetRawInputData());
            StartCoroutine(SetPosition());
            StartCoroutine(DestroyChildren());
        }

        private void CreateOperators(GenericDatamodel data)
        {
            GameObject obj = _observer.CreateOperator(_observer.GetOperatorPrefabs()[5], _parents);
            obj.GetComponent<GenericOperator>().SetRawInputData(data);
        }

        private Vector3 getSpawnPositionOffsetForButton(Transform origin, int nrButton, int totalButtons)
        {
            var defaultDistance = 1;
            Quaternion rotationSave = origin.rotation;
            Vector3 newPos;
            if (totalButtons == 1)
            {
                origin.Rotate(new Vector3(0, 90, 0));
                newPos = origin.position + origin.forward * defaultDistance;
                origin.rotation = rotationSave;
            }
            else
            {
                float maxangle = Math.Min(300, totalButtons * 10);
                float step = maxangle / (totalButtons - 1);
                float startdegree = maxangle / 2;

                float destinationdegree = nrButton * step;
                origin.Rotate(new Vector3(-startdegree + destinationdegree, 90, 0));
                newPos = origin.position + origin.forward * defaultDistance;
            }
            origin.rotation = rotationSave;
            return newPos;
        }

        IEnumerator SetPosition()
        {
            yield return 0;
            Transform t = GetIcon().transform;
            foreach (var child in Children)
            {
                _counter++;
                child.GetIcon().transform.position = getSpawnPositionOffsetForButton(t, _counter, 3);
                if (child.Children.Count > 0)
                {
                    child.Children[0].GetIcon().transform.position = child.GetIcon().transform.position + new Vector3(1f, 0, 0);
                }
            }
        }
        IEnumerator DestroyChildren()
        {
            yield return 0;
            foreach(var child in Children)
            {
                Destroy(child);
            }
        }
        private void Destroy(GenericOperator op)
        {
            if(op.GetType().Equals(typeof(NewOperator)))
            {
                Observer.DestroyOperator(op);
            }
            if(op.Children.Count==0)
            {
                return;
            }
            else
            {
                Destroy(op.Children[op.Children.Count - 1]);
            }
        }
    }
}

