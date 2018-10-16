﻿using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class SaveLoadData:MonoBehaviour {
    public static GenericOperatorContainer genericOperatorContainer = new GenericOperatorContainer();
    public delegate void SerializeAction();

    private static Observer observer;
    public static event SerializeAction OnBeforeSave;
    private static GenericOperator root;
    private static SaveLoadData instance;
    // Use this for initialization
    void Start () {

	}

    private void Awake()
    {
        instance = this;
        observer = (Observer)FindObjectOfType(typeof(Observer));
    }

    public static void SaveData(string path, GenericOperatorContainer operators)
    {
        OnBeforeSave();
        SaveOperators(path, operators);
        ClearOperators();
    }

    public static void LoadData(string path)
    {
        //destroy any current nodes in observer
        if(observer.GetOperators()!=null)
        {
            for(int i=0; i< observer.GetOperators().Count; i++)
            {
                if(observer.GetOperators()[i]!= null)
                {
                    observer.DestroyOperator(observer.GetOperators()[i]);
                    i = -1;
                }
            }
        }

        genericOperatorContainer = LoadOperators(path);
        foreach (OperatorData data in genericOperatorContainer.operators)
        {
            SaveLoadController.CreateGenericOperator(data);
        }
        root = observer.GetOperators()[0];
        instance.StartCoroutine(instance.ReloadData(root));
    }

    public static void AddOperatorData(OperatorData data)
    {
        if (data.name == null) return;
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

    IEnumerator ReloadData(GenericOperator firstNode)
    {
        yield return 0;
        firstNode.reProcess(firstNode.GetOutputData());
    }
}
