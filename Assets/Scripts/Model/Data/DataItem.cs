using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataItem {
    private List<DataAttribute> _dataAttributeValuePairs = new List<DataAttribute>();
   // private id
    public void Add(DataAttribute dataAttribute)
    {
        _dataAttributeValuePairs.Add(dataAttribute);
    }

    public List<DataAttribute> GetDataAttributeValuePairs()
    {
        return _dataAttributeValuePairs;
    }

    public Vector3 GetfirstThreeNumericColsAsVector()
    {
        var v = new Vector3();
        var c = 0;
        foreach (var attr in _dataAttributeValuePairs)
        {
            if (c > 2) return v;
            if (attr.GetValueDataType() == DataAttribute.Valuetype.ValFloat)
            {
                v[c] = (float)attr.GetValue();
                c++;
            }
        }
        return v;
    }
    
    public Vector3 GetfirstFourNumericColsAsVector()
    {
        var v = new Vector3();
        var c = 0;
        foreach (var attr in _dataAttributeValuePairs)
        {
            if (c > 3) return v;
            if (attr.GetValueDataType() == DataAttribute.Valuetype.ValFloat)
            {
                v[c] = (float)attr.GetValue();
                c++;
            }
        }
        return v;
    }
}
