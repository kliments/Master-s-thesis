using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataItem {
    private List<DataAttribute> dataAttributeValuePairs = new List<DataAttribute>();
   // private id
    public void add(DataAttribute dataAttribute)
    {
        dataAttributeValuePairs.Add(dataAttribute);
    }

    public List<DataAttribute> getDataAttributeValuePairs()
    {
        return dataAttributeValuePairs;
    }

    public Vector3 getfirstThreeNumericColsAsVector()
    {
        Vector3 v = new Vector3();
        int c = 0;
        foreach (var attr in dataAttributeValuePairs)
        {
            if (c > 2) return v;
            if (attr.getValueDataType() == DataAttribute.valuetype.val_float)
            {
                v[c] = (float)attr.getValue();
                c++;
            }
        }
        return v;
    }
}
