using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class MenueScript : MonoBehaviour {

    public ToggleScript TogglePrefab;
    public DiscreteSliderScript DiscreteSliderPrefab;
    public ButtonScript ButtonPrefab;
    public InputfieldScript InputfieldPrefab;
    public GameObject ElementList;  

    private List<GenericMenueComponent> _currentComponentList = new List<GenericMenueComponent>();
    private List<ToggleScript> _currentTogglesList = new List<ToggleScript>();
    private List<DiscreteSliderScript> _currentSliderList = new List<DiscreteSliderScript>();
    private List<ButtonScript> _currentButtonList = new List<ButtonScript>();
    private List<InputfieldScript> _currentInputfieldList = new List<InputfieldScript>();

    public int AddToggle(string name, IMenueComponentListener listener)
    {
        ToggleScript newToggle = Instantiate(TogglePrefab);
        int componentId = GetUnusedId();
        newToggle.initMe(componentId, name);
        newToggle.transform.parent = ElementList.transform;
        //TODO: Create a scrollable List for elements.
        newToggle.transform.localPosition = new Vector3(0f, (0.5f - (_currentComponentList.Count * 0.1f)), -0.01f);
        newToggle.transform.localScale = new Vector3(1f,1f,1f);
        _currentComponentList.Add(newToggle);
        _currentTogglesList.Add(newToggle);
        if (listener != null)
        {
            AddListener(componentId, listener);
        }
        return componentId;
    }

    public int AddDiscreteSlider(string name, IMenueComponentListener listener)
    {
        DiscreteSliderScript newDiscreteSlider = Instantiate(DiscreteSliderPrefab);
        int componentId = GetUnusedId();
        newDiscreteSlider.initMe(componentId, name);
        newDiscreteSlider.transform.parent = ElementList.transform;
        //TODO: Create a scrollable List for elements.
        newDiscreteSlider.transform.localPosition = new Vector3(0f, (0.5f - (_currentComponentList.Count * 0.1f)), -0.01f);
        newDiscreteSlider.transform.localScale = new Vector3(1f,1f,1f);
        _currentComponentList.Add(newDiscreteSlider);
        _currentSliderList.Add(newDiscreteSlider);
        if (listener != null)
        {
            AddListener(componentId, listener);
        }
        return componentId;
    }
    
    public int AddButton(string name, IMenueComponentListener listener)
    {
        ButtonScript newButton = Instantiate(ButtonPrefab);
        int componentId = GetUnusedId();
        newButton.initMe(componentId, name);
        newButton.transform.parent = ElementList.transform;
        //TODO: Create a scrollable List for elements.
        newButton.transform.localPosition = new Vector3(0f, (0.5f - (_currentComponentList.Count * 0.1f)), -0.01f);
        newButton.transform.localScale = new Vector3(1f,1f,1f);
        _currentComponentList.Add(newButton);
        _currentButtonList.Add(newButton);
        if (listener != null)
        {
            AddListener(componentId, listener);
        }
        return componentId;
    }
    
    public int AddInputfield(string name, IMenueComponentListener listener)
    {
        InputfieldScript newInputfield = Instantiate(InputfieldPrefab);
        int componentId = GetUnusedId();
        newInputfield.initMe(componentId, name);
        newInputfield.transform.parent = ElementList.transform;
        //TODO: Create a scrollable List for elements.
        newInputfield.transform.localPosition = new Vector3(0f, (0.5f - (_currentComponentList.Count * 0.1f)), -0.01f);
        newInputfield.transform.localScale = new Vector3(0.0025f,0.0025f,0.0025f);
        _currentComponentList.Add(newInputfield);
        _currentInputfieldList.Add(newInputfield);
        if (listener != null)
        {
            AddListener(componentId, listener);
        }
        return componentId;
    }
    
    public int AddLabel(string name, IMenueComponentListener listener)
    {
        InputfieldScript newInputfield = Instantiate(InputfieldPrefab);
        int componentId = GetUnusedId();
        newInputfield.initMe(componentId, name);
        newInputfield.transform.parent = ElementList.transform;
        //TODO: Create a scrollable List for elements.
        newInputfield.transform.localPosition = new Vector3(0f, (0.5f - (_currentComponentList.Count * 0.1f)), -0.01f);
        newInputfield.transform.localScale = new Vector3(0.0025f,0.0025f,0.0025f);
        _currentComponentList.Add(newInputfield);
        _currentInputfieldList.Add(newInputfield);
        if (listener != null)
        {
            AddListener(componentId, listener);
        }
        return componentId;
    }

    // TODO to remove a component, it has to be removed from the generic component list but also from the more specific list (for example currentTogglesList)

    public bool GetToggleValue(int componentId)
    {
        ToggleScript component = GetToggleById(componentId);
        if(component == null)
        {
            Debug.Log("Can not find Component!");
            return false;
        }
        return component.isActivated();
    }
    
    public int GetInputfieldValue(int componentId)
    {
        InputfieldScript component = GetInputfieldById(componentId);

        var value = 0;
        
        if(component == null)
        {
            Debug.Log("Can not find Component!");
            return value;
        }
        
        
        try
        {
            value = int.Parse(component.GetComponentInChildren<Text>().text);
//            Debug.Log("Yo cluster me about " + K + " blops!");
        }
        catch (Exception e)
        {
//            Debug.Log("There's no input in the inputfield.");
        }

        return value;
    }

    private int GetUnusedId()
    {
        int nextId = 0;
        foreach(GenericMenueComponent com in _currentComponentList)
        {
            int i = com.getId();
            if(i >= nextId)
            {
                nextId = i + 1;
            }
        }
        return nextId;
    }

    private GenericMenueComponent GetComponentById(int id)
    {
        foreach(GenericMenueComponent com in _currentComponentList)
        {
            if(com.getId() == id)
            {
                return com;
            }
        }
        return null;
    }

    private ToggleScript GetToggleById(int id)
    {
        foreach (ToggleScript toggle in _currentTogglesList)
        {
            if (toggle.getId() == id)
            {
                return toggle;
            }
        }
        return null;
    }
    
    private InputfieldScript GetInputfieldById(int id)
    {
        foreach (InputfieldScript inputfield in _currentInputfieldList)
        {
            if (inputfield.getId() == id)
            {
                return inputfield;
            }
        }
        return null;
    }

    public bool AddListener(int componentId, IMenueComponentListener newListener)
    {
        GenericMenueComponent com = GetComponentById(componentId);
        if(com == null)
        {
            Debug.Log("Could not find menue component with this id. Was it removed?");
            return false;
        }
        bool succes = com.addListener(newListener);
        return succes;
    }
    
    public void Delete()
    {
        while (ElementList.transform.GetChild(0) != null)
        {
            Destroy(ElementList.transform.GetChild(0));
        }
        
        _currentComponentList.Clear();
        _currentButtonList.Clear();
        _currentInputfieldList.Clear();
        _currentSliderList.Clear();
        _currentTogglesList.Clear();
    }
}
