using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenueScript : MonoBehaviour {

    public ToggleScript togglePrefab;
    public GameObject elementList;

    private List<GenericMenueComponent> currentComponentList = new List<GenericMenueComponent>();
    private List<ToggleScript> currentTogglesList = new List<ToggleScript>();

	// Use this for initialization
	void Start () {
        
        

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public int addToggle(string name, IMenueComponentListener listener)
    {
        ToggleScript newToggle = Instantiate(togglePrefab);
        int componentId = getUnusedId();
        newToggle.initMe(componentId, name);
        newToggle.transform.parent = elementList.transform;
        //TODO: Create a scrollable List for elements.
        newToggle.transform.localPosition = new Vector3(0f, (0.5f - (currentComponentList.Count * 0.1f)), -0.005f);
        newToggle.transform.localScale = new Vector3(1f,1f,1f);
        currentComponentList.Add(newToggle);
        currentTogglesList.Add(newToggle);
        if (listener != null)
        {
            addListener(componentId, listener);
        }
        return componentId;
    }

    // TODO to remove a component, it has to be removed from the generic component list but also from the more specific list (for example currentTogglesList)

    public bool getToggleValue(int ComponentId)
    {
        ToggleScript component = getToggleById(ComponentId);
        if(component == null)
        {
            Debug.Log("Can not find Component!");
            return false;
        }
        return component.isActivated();
    }

    private int getUnusedId()
    {
        int nextId = 0;
        foreach(GenericMenueComponent com in currentComponentList)
        {
            int i = com.getId();
            if(i >= nextId)
            {
                nextId = i + 1;
            }
        }
        return nextId;
    }

    private GenericMenueComponent getComponentById(int id)
    {
        foreach(GenericMenueComponent com in currentComponentList)
        {
            if(com.getId() == id)
            {
                return com;
            }
        }
        return null;
    }

    private ToggleScript getToggleById(int id)
    {
        foreach (ToggleScript toggle in currentTogglesList)
        {
            if (toggle.getId() == id)
            {
                return toggle;
            }
        }
        return null;
    }

    public bool addListener(int componentId, IMenueComponentListener newListener)
    {
        GenericMenueComponent com = getComponentById(componentId);
        if(com == null)
        {
            Debug.Log("Could not find menue component with this id. Was it removed?");
            return false;
        }
        bool succes = com.addListener(newListener);
        return succes;
    }
}
