using System;

public class DataAttribute
{
    private int _column;
    private string _name;
    public enum Valuetype {ValNa, ValObj, ValString, ValFloat}
    private Valuetype _valueDatatype;

    private object _value;

    public void Init(int column, string name, string value, Valuetype valueDatatype = Valuetype.ValNa)
    {
        this._column = column;
        this._name = name;

        if (valueDatatype == Valuetype.ValNa)
        {
            this._valueDatatype = GetDataType(value);
        }
        else
        {
            this._valueDatatype = valueDatatype;
        }

        this._valueDatatype = GetDataType(value);
        this._value = GetDataObject(value);
    }

    public static Valuetype GetDataType(string attVal)
    {
        float valFloat; 
        if (float.TryParse(attVal, out valFloat))
            return Valuetype.ValFloat;

        return Valuetype.ValString;
    }

    private object GetDataObject(string attVal)
    {
        if (_valueDatatype == Valuetype.ValFloat)
        {
            float valFloat;
            if (float.TryParse(attVal, out valFloat))
            {
                return valFloat;
            }
            throw new Exception("type conversion error!");
        }
        
        return attVal;
    }

    public Valuetype GetValueDataType()
    {
        return _valueDatatype;
    }

    public object GetValue()
    {
        return _value;
    }

    public string GetName()
    {
        return _name;
    }
}
