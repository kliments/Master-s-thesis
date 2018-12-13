using Assets.Scripts.Model;
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
    private static GraphSpaceController graphSpace;
    public static DefaultAlgorithm algorithm;
    // Use this for initialization
    void Start () {
        graphSpace = (GraphSpaceController)FindObjectOfType(typeof(GraphSpaceController));
        algorithm = (DefaultAlgorithm)FindObjectOfType(typeof(DefaultAlgorithm));
    }

    private void Awake()
    {
        instance = this;
        observer = (Observer)FindObjectOfType(typeof(Observer));
    }

    public static void SaveData(string path, GenericOperatorContainer operators)
    {
        Debug.Log("press save");
        if (genericOperatorContainer.operators.Count > 0) ClearOperators();
        OnBeforeSave();
        SaveOperators(path, operators);
        ClearOperators();
    }

    public static void LoadData(string path)
    {
        Debug.Log("press load");
        ClearOperators();
        //destroy any current nodes in observer
        if (observer.GetOperators()!=null)
        {
            for(int i= observer.GetOperators().Count-1; i>=0; i--)
            {
                observer.GetOperators()[i].Disable();
                observer.DestroyOperator(observer.GetOperators()[i]);
            }
        }
        algorithm.positions = new List<Vector3>();
        genericOperatorContainer = LoadOperators(path);
        graphSpace.graphEdges = new List<LineRenderer>();
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
        FileStream stream;
        stream = new FileStream(path, FileMode.Create);

        serializer.Serialize(stream, operators);

        stream.Close();
    }

    IEnumerator ReloadData(GenericOperator firstNode)
    {
        yield return 0;
        algorithm.positions = new List<Vector3>();
        if (firstNode.GetRawInputData()!=null)
        {
            firstNode.ReProcess(firstNode.GetRawInputData());
        }
        else
        {
            firstNode.ReProcess(firstNode.GetOutputData());
        }
    }
}
