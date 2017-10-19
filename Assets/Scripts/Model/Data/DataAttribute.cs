using System;

public class DataAttribute
{
    private int column;
    private string name;
    public enum valuetype {val_na, val_obj, val_string, val_float}
    private valuetype value_datatype;

    private Object value;

    public void init(int column, string name, string value, valuetype value_datatype = valuetype.val_na)
    {
        this.column = column;
        this.name = name;

        if (value_datatype == valuetype.val_na)
        {
            this.value_datatype = getDataType(value);
        }
        else
        {
            this.value_datatype = value_datatype;
        }

        this.value_datatype = getDataType(value);
        this.value = getDataObject(value);
    }

    public static valuetype getDataType(string attVal)
    {
        float val_float; 
        if (float.TryParse(attVal, out val_float))
            return valuetype.val_float;

        return valuetype.val_string;
    }

    private Object getDataObject(string attVal)
    {
        if (value_datatype == valuetype.val_float)
        {
            float val_float;
            if (float.TryParse(attVal, out val_float))
            {
                return val_float;
            }
            throw new Exception("type conversion error!");
        }
        
        return attVal;
    }

    public valuetype getValueDataType()
    {
        return value_datatype;
    }

    public Object getValue()
    {
        return value;
    }
}
