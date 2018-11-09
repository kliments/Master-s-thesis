using Assets.Scripts.Model;
using Model.Operators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadController : Targetable {
    public Button saveButton, loadButton;
    public static List<GenericOperator> operatorList;
    public static UnityEngine.Events.UnityAction saveData;
    public static UnityEngine.Events.UnityAction loadData;
    public static string dataPath;

    private static Observer obs;
    //instance variable is needed to call static Coroutines
    private static SaveLoadController instance;
    // Use this for initialization
    void Start () {
        obs = (Observer)FindObjectOfType(typeof(Observer));
        operatorList = new List<GenericOperator>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Awake()
    {
        instance = this;
        dataPath = "C:/Kliment/Master's Project/VRVis/Assets/Resources/SavedData/operators.xml";
    }

    public static GenericOperator CreateGenericOperator(OperatorData data)
    {
        Vector3 position = new Vector3();
        List<GenericOperator> parents = new List<GenericOperator>();
        List<GameObject> prefabList = obs.GetOperatorPrefabs();
        foreach (var prefab in prefabList)
        {
            if (data.name == prefab.name)
            {
                parents = new List<GenericOperator>();
                position = new Vector3(data.posX, data.posY, data.posZ);
                int parentID = 0;
                foreach(var child in obs.GetOperators())
                {
                    parentID = child.Id;
                    if (parentID == data.parent)
                    {
                        parents.Add(child.GetComponent<GenericOperator>());
                        break;
                    }
                }
                GameObject go = obs.CreateOperator(prefab, parents);
                GenericOperator op = go.GetComponent<GenericOperator>() ?? go.AddComponent<GenericOperator>();
                //waiting for one frame, due to generating icons and children
                instance.StartCoroutine(instance.SetIconLocation(op, position));
                instance.StartCoroutine(instance.DestroyNewOperatorChildren(op.Children));
                op.Id = data.ID;
                op.LoadSpecificData(data);

                operatorList.Add(op);
                return op;
            }
        }
        return null;
    }

    IEnumerator SetIconLocation(GenericOperator op, Vector3 position)
    {
        yield return 0;
        op.GetIcon().transform.position = position;
        op.Fetchdata();
        op.Process();
    }

    IEnumerator DestroyNewOperatorChildren(List<GenericOperator> children)
    {
        yield return 0;
        foreach(var child in children.ToArray())
        {
            if (child.GetType().Equals(typeof(NewOperator)))
            {
                obs.DestroyOperator(child);
            }
        }
    }

    private void OnEnable()
    {
        saveData = delegate { SaveLoadData.SaveData(dataPath, SaveLoadData.genericOperatorContainer); };
        loadData = delegate { SaveLoadData.LoadData(dataPath); };
        operatorList = new List<GenericOperator>();
    }

    private void OnDisable()
    {

    }
}
