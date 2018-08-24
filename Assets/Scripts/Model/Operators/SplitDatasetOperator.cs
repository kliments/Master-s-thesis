using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
public class SplitDatasetOperator : GenericOperator
{   
    public float _yThreshold = 0.5f;

    private List<DataItem> _dataItems;
    private SimpleDatamodel[] _simpleDataModel;
	// Use this for initialization
	public override void Start () {
        base.Start();
        _dataItems = _rawInputData.GetDataItems();
        _simpleDataModel = new SimpleDatamodel[2];
        SplitDataset();
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
        for(int i=0; i<_simpleDataModel.Length; i++)
        {
            _simpleDataModel[i] = new SimpleDatamodel();
        }
        foreach(var dataItem in _dataItems)
        {
            if(dataItem.GetfirstThreeNumericColsAsVector().y>=_yThreshold)
            {
                _simpleDataModel[0].Add(dataItem);
            }
            else
            {
                _simpleDataModel[1].Add(dataItem);
            }
        }
        foreach(var data in _simpleDataModel)
        {
            SetOutputData(data);
            Parents[Parents.Count - 1].Process();
        }
    }
}
