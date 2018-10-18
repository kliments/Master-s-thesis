using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldScript : GenericMenueComponent{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetInputType(InputField.ContentType inputType)
    {
        GetComponent<InputField>().contentType = inputType;
    }

    public void SetInputLimit(int limit)
    {
        GetComponent<InputField>().characterLimit = limit;
    }

    public void UpdateInputText(string text)
    {
        GetComponent<InputField>().text = text;
    }
}
