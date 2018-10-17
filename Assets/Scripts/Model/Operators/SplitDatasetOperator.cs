using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
using System;
using UnityEngine.UI;

namespace Model.Operators
{
    public class SplitDatasetOperator : GenericOperator, IMenueComponentListener
    {
        public float threshold = 0.5f;
        public string axis;
        public bool press = false;
        public GameObject menueInputPrefab, menueButtonPrefab, textNextToInputPrefab, menueDropdown;

        private Observer _observer;
        private List<DataItem> _dataItems;
        private SimpleDatamodel[] _simpleDataModels;
        private List<GenericOperator> _parents;
        private int _counter = 0, _datasets = 2, _parentIndex;
        private GameObject _canvas, _menueStartButton, _thresholdInput, _axisInput, _textNextToThresholdInput, _textNextToAxisInput;
        private MenueScript _menu;
        private bool _hasBeenRotated = false;
        private InputFieldScript _input;
        private DropdownScript _dropdown;
        private ButtonScript _button;
        // Use this for initialization
        public override void Start()
        {
            _observer = (Observer)(FindObjectOfType(typeof(Observer)));
            _parents = new List<GenericOperator>();
            _parents.Add(this);
            _canvas = GameObject.Find("Canvas");
            _menu = (MenueScript)(FindObjectOfType(typeof(MenueScript)));
            axis = "X";

            base.Start();
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
            if (GetRawInputData()!=null)
            {
                _dataItems = _rawInputData.GetDataItems();
                _simpleDataModels = new SimpleDatamodel[_datasets];
                SplitDataset();
                //create operators
                if (Children.Count == 0)
                {
                    for (int i = 0; i < _simpleDataModels.Length; i++)
                    {
                        StartCoroutine(CreateOperators(_simpleDataModels[i]));
                    }
                    SetOutputData(GetRawInputData());
                }
                else
                {
                    for (int i = 0; i < Children.Count; i++)
                    {
                        Children[i].SetRawInputData(null);
                        Children[i].SetRawInputData(_simpleDataModels[i]);
                        Children[i].reProcess(_simpleDataModels[i]);
                    }
                }
            }
            return true;
        }

        public override bool ValidateIfOperatorPossibleForParents(GenericOperator parent)
        {
            //spawn SplitDatasetOperator only if a parent has output data
            return parent.hasOutput;
        }

        public void SplitDataset()
        {
            for (int i = 0; i < _simpleDataModels.Length; i++)
            {
                _simpleDataModels[i] = new SimpleDatamodel();
            }
            foreach (var dataItem in _dataItems)
            {
                if(axis == "X")
                {
                    if (dataItem.GetfirstThreeNumericColsAsVector().x >= threshold)
                    {
                        _simpleDataModels[0].Add(dataItem);
                    }
                    else
                    {
                        _simpleDataModels[1].Add(dataItem);
                    }
                }
                else if (axis == "Y")
                {
                    if (dataItem.GetfirstThreeNumericColsAsVector().y >= threshold)
                    {
                        _simpleDataModels[0].Add(dataItem);
                    }
                    else
                    {
                        _simpleDataModels[1].Add(dataItem);
                    }
                }
                else if (axis == "Z")
                {
                    if (dataItem.GetfirstThreeNumericColsAsVector().z >= threshold)
                    {
                        _simpleDataModels[0].Add(dataItem);
                    }
                    else
                    {
                        _simpleDataModels[1].Add(dataItem);
                    }
                }
            }
            //get parent operator index for creating
            bool toBreak = false;
            foreach(var op in _observer.GetOperatorPrefabs())
            {
                foreach(var parent in Parents)
                {
                    if (parent.name.Contains(op.name))
                    {
                        toBreak = true;
                        break;
                    }
                }
                if (toBreak) break;
                _parentIndex++;
            }
        }

        IEnumerator CreateOperators(GenericDatamodel data)
        {
            //wait for the next frame
            yield return 0;

            GameObject obj = _observer.CreateOperator(_observer.GetOperatorPrefabs()[_parentIndex], _parents);
            obj.GetComponent<GenericOperator>().SetRawInputData(data);
            StartCoroutine(SetPosition());
        }

        IEnumerator SetPosition()
        {
            //make sure function is called only one time
            if(!_hasBeenRotated)
            {
                _hasBeenRotated = true;
                //wait for the next frame
                yield return 0;

                foreach (var child in Children)
                {
                    child.GetIcon().transform.position = getSpawnPositionOffsetForButton(GetIcon().transform, _counter, _datasets);
                    if (child.Children.Count > 0)
                    {
                        child.Children[0].GetIcon().transform.position = child.GetIcon().transform.position + new Vector3(1f, 0, 0);
                    }
                    _counter++;
                }
                //destroy newoperators that are generated due to selection of child nodes
                StartCoroutine(DestroyChildren());
                Observer.selectOperator(this);
            }
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

        IEnumerator DestroyChildren()
        {
            //wait for the next frame
            yield return 0;

            foreach(var child in Children)
            {
                DestroyNewOperatorChildren(child);
            }
        }
        private void DestroyNewOperatorChildren(GenericOperator op)
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
                DestroyNewOperatorChildren(op.Children[op.Children.Count - 1]);
            }
        }

        private IEnumerator CreateMenueButtons()
        {
            yield return 0;
            _input = _menu.AddInputField("Threshold", this);
            _dropdown = _menu.AddDropdown("Axis", this);
            _button = _menu.AddButton("SplitDataset", this);
        }

        public void UpdateValues()
        {
            if(_thresholdInput.GetComponent<InputField>().text != "")
            {
                threshold = float.Parse(_thresholdInput.GetComponent<InputField>().text);
            }
            axis = _axisInput.GetComponent<Dropdown>().options[_axisInput.GetComponent<Dropdown>().value].text;
        }
        public void StartSplitDatasets()
        {
            ResetMe();
            Process();
        }

        private void ResetMe()
        {
            _hasBeenRotated = false;
            _counter = 0;
            _parentIndex = 0;
        }

        protected override void OnSelectAction()
        {
            base.OnSelectAction();
            StartCoroutine(CreateMenueButtons());
        }

        protected override void OnUnselectAction()
        {
            base.OnUnselectAction();
            _menu.RemoveAllComponents();
        }

        public void menueChanged(GenericMenueComponent changedComponent)
        {
            threshold = float.Parse(_input.GetComponent<InputField>().text);
            switch(_dropdown.GetComponent<Dropdown>().value)
            {
                case 0:
                    axis = "X";
                    break;
                case 1:
                    axis = "Y";
                    break;
                case 2:
                    axis = "Z";
                    break;
            }
            StartSplitDatasets();
        }
    }

}

