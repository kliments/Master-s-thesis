using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadController : MonoBehaviour {
    public Button saveButton, loadButton;
    private static Observer obs = new Observer();

    private static string dataPath = string.Empty;
	// Use this for initialization
	void Start () {
        obs = (Observer)FindObjectOfType(typeof(Observer));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Awake()
    {
        dataPath = "C:/Kliment/Master's Project/VRVis/Assets/Resources/SavedData/operators.xml";
    }

    public static GenericOperator CreateGenericOperator(string name, Vector3 position)
    {
        List<GameObject> prefabList = obs.GetOperatorPrefabs();
        foreach(var prefab in prefabList)
        {
            if(name == prefab.name)
            {
                GameObject go = Instantiate(prefab, position, Quaternion.identity);
                GenericOperator op = go.GetComponent<GenericOperator>() ?? go.AddComponent<GenericOperator>();
                return op;
            }
        }
        return null;
    }

    public static GenericOperator CreateGenericOperator(OperatorData data, string name, Vector3 position)
    {
        List<GameObject> prefabList = obs.GetOperatorPrefabs();
        foreach (var prefab in prefabList)
        {
            if (name == prefab.name)
            {
                GameObject go = Instantiate(prefab, position, Quaternion.identity);
                GenericOperator op = go.GetComponent<GenericOperator>() ?? go.AddComponent<GenericOperator>();
                op.data = data;
                return op;
            }
        }
        return null;
    }

    private void OnEnable()
    {
        saveButton.onClick.AddListener(delegate { SaveLoadData.SaveData(dataPath, SaveLoadData.genericOperatorContainer); });
        loadButton.onClick.AddListener(delegate { SaveLoadData.LoadData(dataPath); });
    }

    private void OnDisable()
    {
        saveButton.onClick.RemoveListener(delegate { SaveLoadData.SaveData(dataPath, SaveLoadData.genericOperatorContainer); });
        loadButton.onClick.RemoveListener(delegate { SaveLoadData.LoadData(dataPath); });
    }
}
