using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownScript : GenericMenueComponent{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddOption(string text)
    {
        Dropdown.OptionData opt = new Dropdown.OptionData();
        opt.text = text;
        GetComponent<Dropdown>().options.Add(opt);
    }

    public void UpdateOption(string text)
    {
        int value = 0;
        switch(text)
        {
            case "X":
                value = 0;
                break;
            case "Y":
                value = 1;
                break;
            case "Z":
                value = 2;
                break;
        }
        GetComponent<Dropdown>().value = value;
    }
}
