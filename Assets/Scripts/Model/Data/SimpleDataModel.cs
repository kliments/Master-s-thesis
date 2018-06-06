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

    public void addFloatMatrixColwise(float[][] data) {
        for (int i = 0; i < data.Length; i++) {
            var dataItem = new DataItem();
            for (int j = 0; j < data[0].Length; j++) {
                var dataAttribute = new DataAttribute();
                dataAttribute.Init(j, "var"+j, data[i][j]+"", DataAttribute.Valuetype.ValFloat);
                dataItem.Add(dataAttribute);
            }
            Add(dataItem);
        }
    }
}
