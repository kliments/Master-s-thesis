using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDatamodel : GenericDatamodel {
    public List<Vector3> GetCoords()
    {
        var list = new List<Vector3>();
        foreach (var dataItem in DataItems)
        {
            list.Add(dataItem.GetfirstThreeNumericColsAsVector());
        }
        return list;
    }
}
