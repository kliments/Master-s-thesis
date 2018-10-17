using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenueScript : MonoBehaviour {

    public ToggleScript togglePrefab;
    public DiscreteSliderScript DiscreteSliderPrefab;
    public ButtonScript buttonPrefab;
    public DropdownScript dropdownPrefab;
    public InputFieldScript inputFieldPrefab;
    public GameObject elementList;

    public GameObject ClusterNumber;
    public GameObject LoopNumber;
    public GameObject KMeanUpdateButton;
    public GameObject KMeanStartButton;


    private List<GenericMenueComponent> currentComponentList = new List<GenericMenueComponent>();
    private List<ToggleScript> currentTogglesList = new List<ToggleScript>();
    private List<ButtonScript> currentButtonsList = new List<ButtonScript>();

    // Use this for initialization
    void Start() {



    }

    // Update is called once per frame
    void Update() {

    }

    public int addToggle(string name, IMenueComponentListener listener)
    {
        ToggleScript newToggle = Instantiate(togglePrefab);
        int componentId = getUnusedId();
        newToggle.initMe(componentId, name);
        newToggle.transform.parent = elementList.transform;
        //TODO: Create a scrollable List for elements.
        newToggle.transform.localPosition = new Vector3(0f, (0.5f - (currentComponentList.Count * 0.1f)), -0.005f);
        newToggle.transform.localScale = new Vector3(1f, 1f, 1f);
        currentComponentList.Add(newToggle);
        currentTogglesList.Add(newToggle);
        if (listener != null)
        {
            addListener(componentId, listener);
        }
        return componentId;
    }

    public int AddDiscreteSlider(string name, IMenueComponentListener listener)
    {
        DiscreteSliderScript newDiscreteSlider = Instantiate(DiscreteSliderPrefab);
        int componentId = getUnusedId();
        newDiscreteSlider.initMe(componentId, name);
        newDiscreteSlider.transform.parent = elementList.transform;
        //TODO: Create a scrollable List for elements.
        newDiscreteSlider.transform.localPosition = new Vector3(0f, (0.5f - (currentComponentList.Count * 0.1f)), -0.005f);
        newDiscreteSlider.transform.localScale = new Vector3(1f, 1f, 1f);
        currentComponentList.Add(newDiscreteSlider);
        //        currentTogglesList.Add(newDiscreteSlider);
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
        if (component == null)
        {
            Debug.Log("Can not find Component!");
            return false;
        }
        return component.isActivated();
    }

    private int getUnusedId()
    {
        int nextId = 0;
        foreach (GenericMenueComponent com in currentComponentList)
        {
            int i = com.getId();
            if (i >= nextId)
            {
                nextId = i + 1;
            }
        }
        return nextId;
    }

    private GenericMenueComponent getComponentById(int id)
    {
        foreach (GenericMenueComponent com in currentComponentList)
        {
            if (com.getId() == id)
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
        if (com == null)
        {
            Debug.Log("Could not find menue component with this id. Was it removed?");
            return false;
        }
        bool succes = com.addListener(newListener);
        return succes;
    }

    public ButtonScript AddButton(string name, IMenueComponentListener listener)
    {
        ButtonScript newButton = Instantiate(buttonPrefab);
        int componentId = getUnusedId();
        newButton.initMe(componentId, name);
        newButton.transform.parent = elementList.transform;
        newButton.transform.localPosition = new Vector3(0f, (0.5f - (currentComponentList.Count * 0.1f)), -0.01f);
        newButton.transform.localScale = new Vector3(0.0025f, 0.0025f, 0.0025f);

        SetTextOfComponent(newButton, name);
        currentComponentList.Add(newButton);
        currentButtonsList.Add(newButton);
        if (listener != null)
        {
            addListener(componentId, listener);
        }
        return newButton;
    }

    public DropdownScript AddDropdown(string name, IMenueComponentListener listener)
    {
        DropdownScript newDropdown = Instantiate(dropdownPrefab);
        int componentId = getUnusedId();
        newDropdown.initMe(componentId, name);
        newDropdown.transform.parent = elementList.transform;
        newDropdown.transform.localPosition = new Vector3(0f, (0.5f - (currentComponentList.Count * 0.1f)), -0.01f);
        newDropdown.transform.localScale = new Vector3(0.0025f, 0.0025f, 0.0025f);

        SetTextOfComponent(newDropdown, name);
        currentComponentList.Add(newDropdown);
        if (listener != null)
        {
            addListener(componentId, listener);
        }
        return newDropdown;
    }

    public InputFieldScript AddInputField(string name, IMenueComponentListener listener)
    {
        InputFieldScript newInputField = Instantiate(inputFieldPrefab);
        int componentId = getUnusedId();
        newInputField.initMe(componentId, name);
        newInputField.transform.parent = elementList.transform;
        newInputField.transform.localPosition = new Vector3(0f, (0.5f - (currentComponentList.Count * 0.1f)), -0.01f);
        newInputField.transform.localScale = new Vector3(0.0025f, 0.0025f, 0.0025f);
        currentComponentList.Add(newInputField);

        SetTextOfComponent(newInputField, name);
        if (listener != null)
        {
            addListener(componentId, listener);
        }
        return newInputField;
    }

    private void SetTextOfComponent(GenericMenueComponent component, string text)
    {
        //the text is always the last child in the hierarchy
        component.transform.GetChild(component.transform.childCount - 1).GetComponent<Text>().text = text;
    }

    public void RemoveAllComponents()
    {
        for(int i=0; i<currentComponentList.Count; i++)
        {
            Destroy(currentComponentList[i].gameObject);
            currentComponentList.Remove(currentComponentList[i]);
            i--;
        }
    }
}
