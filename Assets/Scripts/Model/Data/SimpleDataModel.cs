using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDatamodel : GenericDatamodel {
    public List<Vector3> getCoords()
    {
        List<Vector3> list = new List<Vector3>();
        foreach (var dataItem in dataItems)
        {
            list.Add(dataItem.getfirstThreeNumericColsAsVector());
        }
        return list;
    }
}
