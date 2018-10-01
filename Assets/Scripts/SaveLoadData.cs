using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class SaveLoadData {
    public static GenericOperatorContainer genericOperatorContainer = new GenericOperatorContainer();
    public delegate void SerializeAction();

    public static event SerializeAction OnLoaded;
    public static event SerializeAction OnBeforeSave;

	// Use this for initialization
	void Start () {

	}
	
    public static void SaveData(string path, GenericOperatorContainer operators)
    {
        OnBeforeSave();
        SaveOperators(path, operators);
        ClearOperators();
    }

    public static void LoadData(string path)
    {
        genericOperatorContainer = LoadOperators(path);
        foreach(OperatorData data in genericOperatorContainer.operators)
        {
            SaveLoadController.CreateGenericOperator(data, data.name, new Vector3(data.posX, data.posY, data.posZ));
        }
        OnLoaded();
    }

    public static void AddOperatorData(OperatorData data)
    {
        genericOperatorContainer.operators.Add(data);
    }

    public static void ClearOperators()
    {
        genericOperatorContainer.operators.Clear();
    }

    private static GenericOperatorContainer LoadOperators(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(GenericOperatorContainer));

        FileStream stream = new FileStream(path, FileMode.Open);

        GenericOperatorContainer operators = serializer.Deserialize(stream) as GenericOperatorContainer;

        stream.Close();

        return operators;
    }

    private static void SaveOperators(string path, GenericOperatorContainer operators)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(GenericOperatorContainer));

        FileStream stream = new FileStream(path, FileMode.Truncate);

        serializer.Serialize(stream, operators);

        stream.Close();
    }
}
