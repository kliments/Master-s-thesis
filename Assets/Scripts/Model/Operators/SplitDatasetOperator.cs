using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
using System;
using UnityEngine.UI;

namespace Model.Operators
{
    public class SplitDatasetOperator : GenericOperator
    {
        public float threshold = 0.5f;
        public string axis;
        public bool press = false;
        public GameObject menueInputPrefab, menueButtonPrefab, textNextToInputPrefab, menueDropdown;

        private Observer _observer;
        private List<DataItem> _dataItems;
        private SimpleDatamodel[] _simpleDataModel;
        private List<GenericOperator> _parents;
        private int _counter = 0, _datasets = 2;
        private GameObject _canvas, _menueStartButton, _thresholdInput, _axisInput, _textNextToThresholdInput, _textNextToAxisInput;
        private bool _hasBeenRotated = false;
        // Use this for initialization
        public override void Start()
        {
            base.Start();
            _observer = (Observer)(FindObjectOfType(typeof(Observer)));
            _dataItems = _rawInputData.GetDataItems();
            _simpleDataModel = new SimpleDatamodel[_datasets];
            _parents = new List<GenericOperator>();
            _parents.Add(this);
            _canvas = GameObject.Find("Canvas");
            SplitDataset();
            CreateMenueButtons();
            axis = "X";
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
                if(axis == "X")
                {
                    if (dataItem.GetfirstThreeNumericColsAsVector().x >= threshold)
                    {
                        _simpleDataModel[0].Add(dataItem);
                    }
                    else
                    {
                        _simpleDataModel[1].Add(dataItem);
                    }
                }
                else if (axis == "Y")
                {
                    if (dataItem.GetfirstThreeNumericColsAsVector().y >= threshold)
                    {
                        _simpleDataModel[0].Add(dataItem);
                    }
                    else
                    {
                        _simpleDataModel[1].Add(dataItem);
                    }
                }
                else if (axis == "Z")
                {
                    if (dataItem.GetfirstThreeNumericColsAsVector().z >= threshold)
                    {
                        _simpleDataModel[0].Add(dataItem);
                    }
                    else
                    {
                        _simpleDataModel[1].Add(dataItem);
                    }
                }
            }
            //create operators
            for(int i=0; i<_simpleDataModel.Length; i++)
            {
                StartCoroutine(CreateOperators(_simpleDataModel[i]));
            }
            SetOutputData(GetRawInputData());
        }

        IEnumerator CreateOperators(GenericDatamodel data)
        {
            //wait for the next frame
            yield return 0;

            GameObject obj = _observer.CreateOperator(_observer.GetOperatorPrefabs()[5], _parents);
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

        private void CreateMenueButtons()
        {
            _thresholdInput = Instantiate(menueInputPrefab, _canvas.transform);
            _thresholdInput.GetComponent<RectTransform>().localPosition = new Vector3(-100, 230, 0);
            _thresholdInput.GetComponent<InputField>().contentType = InputField.ContentType.DecimalNumber;
            _thresholdInput.GetComponent<InputField>().text = "0.5";
            _thresholdInput.GetComponent<InputField>().onEndEdit.AddListener(delegate { this.UpdateValues(); });

            _textNextToThresholdInput = Instantiate(textNextToInputPrefab, _canvas.transform);
            _textNextToThresholdInput.GetComponent<RectTransform>().localPosition = new Vector3(-185, 230, 0);
            _textNextToThresholdInput.GetComponent<Text>().text = "Threshold: ";

            /*_axisInput = Instantiate(menueInputPrefab, _canvas.transform);
            _axisInput.GetComponent<RectTransform>().localPosition = new Vector3(-100, 190, 0);
            _axisInput.GetComponent<InputField>().contentType = InputField.ContentType.Name;
            _axisInput.GetComponent<InputField>().text = "X";
            _axisInput.GetComponent<InputField>().characterLimit = 1;
            _axisInput.GetComponent<InputField>().onEndEdit.AddListener(delegate { this.UpdateValues(); });*/

            _axisInput = Instantiate(menueDropdown, _canvas.transform);
            _axisInput.GetComponent<RectTransform>().localPosition = new Vector3(-100, 190, 0);
            _axisInput.GetComponent<Dropdown>().onValueChanged.AddListener(delegate { this.UpdateValues(); });

            _textNextToAxisInput = Instantiate(textNextToInputPrefab, _canvas.transform);
            _textNextToAxisInput.GetComponent<RectTransform>().localPosition = new Vector3(-185, 190, 0);
            _textNextToAxisInput.GetComponent<Text>().text = "Axis: ";

            _menueStartButton = Instantiate(menueButtonPrefab, _canvas.transform);
            _menueStartButton.GetComponent<RectTransform>().localPosition = new Vector3(-100, 100, 0);
            _menueStartButton.GetComponent<Button>().onClick.AddListener(delegate { StartSplitDatasets(); });
            _menueStartButton.transform.GetChild(0).GetComponent<Text>().text = "Split Dataset";
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
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Delete(Children[i]);
                --i;
            }
            ResetMe();
            SplitDataset();
        }

        private void ResetMe()
        {
            _hasBeenRotated = false;
            _counter = 0;
        }
    }
}

