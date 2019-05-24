using Assets.Scripts.Model;
using Model.Operators;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRandomData : MonoBehaviour {
    public int nodes, dataName;
    public bool generate;
    public List<string> list;

    private GameObject[] _prefabs;
    private int _randomInt, _parentID;
    private GenericOperatorContainer _container = new GenericOperatorContainer();
    private List<OperatorData> _operatorsData;
    private OperatorData _opData, _splitParentData;
    private Model.Operators.SplitDatasetOperator.CustomSplitData _customSplitData;
    private string _dataPath, _randomName;
    private string[] _axes;
	// Use this for initialization
	void Start () {
        _prefabs = Resources.LoadAll<GameObject>("Operators");
        list = new List<string>();
        _operatorsData = new List<OperatorData>();
        _dataPath = Application.dataPath + "/Resources/LoggedData/";
        _axes = new string[3] { "X", "Y", "Z" };
    }
	
	// Update is called once per frame
	void Update () {
		if(generate)
        {
            generate = false;
            Debug.Log(Application.dataPath);
            //GenerateData();
        }
	}

    public string GenerateData(string dataID)
    {
        list = new List<string>();
        _operatorsData = new List<OperatorData>();
        _container.operators = new List<OperatorData>();
        for(int i=0; i<nodes; i++)
        {
            //create another operator, if root node is about to be SplitDatasetOperator (to create SplitDatasetOperator, this node needs to be child of another operator
            _randomInt = UnityEngine.Random.Range(0, 7);
            if (i==0)
            {
                while (_prefabs[_randomInt].name == "SplitOperator")
                {
                    _randomInt = UnityEngine.Random.Range(0, 7);
                }
            }

            //dont generate New Operator, GlyphOperator(still has bugs) or KMeansClusteringOperator(also bugs)
            while(_prefabs[_randomInt].name == "NewOperator" || _prefabs[_randomInt].name == "GlyphOperator" || _prefabs[_randomInt].name == "KMeansClusteringOperator")
            {
                _randomInt = UnityEngine.Random.Range(0, 7);

                if (i == 0)
                {
                    while (_prefabs[_randomInt].name == "SplitOperator")
                    {
                        _randomInt = UnityEngine.Random.Range(0, 7);
                    }
                }
            }

            list.Add(_prefabs[_randomInt].name);
            _opData = new OperatorData();
            //data attributes
            _opData.name = _prefabs[_randomInt].name;
            _opData.ID = i;
            if (i == 0)
            {
                _opData.parent = 0;
                _opData.posX = 0;
                _opData.posY = 2.5f;
                _opData.posZ = 0;
                _opData.hour = (double)DateTime.Now.Hour - 1;
                _opData.minute = 0;
                _opData.second = 0;
                _opData.ms = 0;
            }
            else
            {
                _parentID = UnityEngine.Random.Range(0, i);
                //random nodes are not allowed to be children of SplitDatasetOperator
                while(list[_parentID] == "SplitOperator")
                {
                    _parentID = UnityEngine.Random.Range(0, i - 1);
                }
                _opData.parent = _parentID;
                _opData.posX = UnityEngine.Random.Range(_operatorsData[_opData.parent].posX - 1, _operatorsData[_opData.parent].posX + 1);
                _opData.posY = UnityEngine.Random.Range(_operatorsData[_opData.parent].posY - 1, _operatorsData[_opData.parent].posY + 1);
                _opData.posZ = UnityEngine.Random.Range(_operatorsData[_opData.parent].posZ - 1, _operatorsData[_opData.parent].posZ + 1);
                _opData.hour = _operatorsData[_opData.parent].hour;
                _opData.minute = UnityEngine.Random.Range((int)_operatorsData[_opData.parent].minute, (int)_operatorsData[_opData.parent].minute + 1);
                _opData.second = UnityEngine.Random.Range((int)_operatorsData[_opData.parent].second, 59);
                _opData.ms = UnityEngine.Random.Range((int)_operatorsData[_opData.parent].ms, 999);
            }
            _operatorsData.Add(_opData);
            _container.operators.Add(_opData);

            //add 2 extra operators if current operator is SplitDatasetOperator
            if(_prefabs[_randomInt].name == "SplitOperator")
            {
                _customSplitData = new SplitDatasetOperator.CustomSplitData();
                _customSplitData.thr = UnityEngine.Random.Range(0f, 1f);
                int random = UnityEngine.Random.Range(0, 3);
                _customSplitData.axis = _axes[random];
                _container.operators[_container.operators.Count-1].customData = _customSplitData;

                _splitParentData = _opData;
                for (int j=i; j<i+2; j++)
                {
                    _opData = new OperatorData();
                    _opData.name = list[_splitParentData.parent];
                    _opData.ID = j+1;
                    _opData.parent = _splitParentData.ID;
                    _opData.posX = UnityEngine.Random.Range(_splitParentData.posX - 1, _splitParentData.posX + 1);
                    _opData.posY = UnityEngine.Random.Range(_splitParentData.posY - 1, _splitParentData.posY + 1);
                    _opData.posZ = UnityEngine.Random.Range(_splitParentData.posZ - 1, _splitParentData.posZ + 1);
                    _opData.hour = _splitParentData.hour;
                    _opData.minute = UnityEngine.Random.Range((int)_splitParentData.minute, (int)_splitParentData.minute + 1);
                    _opData.second = UnityEngine.Random.Range((int)_splitParentData.second, 59);
                    _opData.ms = UnityEngine.Random.Range((int)_splitParentData.ms, 999);

                    list.Add(_opData.name);
                    _operatorsData.Add(_opData);
                    _container.operators.Add(_opData);
                }
                i += 2;
            }
        }
        _randomName = "RandomData" + dataID.ToString() + ".xml";
        var dir = Directory.CreateDirectory(_dataPath + "Participant" + dataID);
        SaveLoadData.SaveRandomData(dir.FullName.ToString() + "\\" + _randomName, _container);
        return (dir.FullName.ToString() + "\\" + _randomName);
    }
}
