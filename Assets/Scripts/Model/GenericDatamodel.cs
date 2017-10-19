using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public abstract class GenericDatamodel
{
    protected List<DataItem> dataItems = new List<DataItem>(); // list of all dataitems (DataItem)

    public void add(DataItem dataItem)
    {
        dataItems.Add(dataItem);
    }

    public List<DataItem> getDataItems()
    {
        return dataItems;
    }

    public GenericDatamodel mergeDatamodels(GenericDatamodel datamodeltomerge)
    {
        if (datamodeltomerge.GetType() != GetType())
        {
            throw new Exception("Cannot merge two different DataTypeModels");
        }

        dataItems.AddRange(datamodeltomerge.getDataItems());

        return this;
    }

}
