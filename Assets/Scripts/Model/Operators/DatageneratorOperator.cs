using System;
using System.IO;
using Assets.Scripts.Model;
using UnityEngine;

public class DatageneratorOperator : GenericOperator
{
    public override bool Process()
    {

        // add here data generation
        // Analog to Dataloader: 
        // var dataModel = ReadCsv().MergeDatamodels(_rawInputData);
        // SetOutputData(dataModel);

        return true;
    }

    public override bool ValidateIfOperatorPossibleForParents(GenericOperator parent)
    {
        return true;
    }

    public override void Start()
    {
        base.Start();
    }
}
