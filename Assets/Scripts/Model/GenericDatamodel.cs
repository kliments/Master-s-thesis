using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public abstract class GenericDatamodel
{
    protected List<DataItem> DataItems = new List<DataItem>(); // list of all dataitems (DataItem)

    public void Add(DataItem dataItem)
    {
        DataItems.Add(dataItem);
    }

    public List<DataItem> GetDataItems()
    {
        return DataItems;
    }

    public GenericDatamodel MergeDatamodels(GenericDatamodel datamodeltomerge)
    {
        if (datamodeltomerge.GetType() != GetType())
        {
            throw new Exception("Cannot merge two different DataTypeModels");
        }

        DataItems.AddRange(datamodeltomerge.GetDataItems());

        return this;
    }

}
