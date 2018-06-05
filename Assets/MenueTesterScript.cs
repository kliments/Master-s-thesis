using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenueTesterScript : MonoBehaviour, IMenueComponentListener {

    private MenueScript menue;

    // Use this for initialization
    void Start () {
        menue = FindObjectOfType<MenueScript>();
        if(menue == null)
        {
            Debug.Log("Did not find a valid menue!");
            return;
        }
        // Create a toggle and instantly assign this script as listener.
        menue.addToggle("showXAxis", this);
        menue.addToggle("showYAxis", this);

        // Create a toggle with no listener. Add this element as a listener afterwards. An arbitrary amount of listeners can be added.
        int id = menue.addToggle("showZAxis", null);
        menue.addListener(id, this);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void menueChanged(GenericMenueComponent changedComponent)
    {
        if (menue == null)
        {
            Debug.Log("Did not find a valid menue!");
            return;
        }

        string name = changedComponent.getName();
        if (name.Equals("showXAxis")){
            Debug.Log("New value for showXAxis: " + menue.getToggleValue(changedComponent.getId()));
            // Do something with the value.
        }
        if (name.Equals("showYAxis"))
        {
            Debug.Log("New value for showYAxis: " + menue.getToggleValue(changedComponent.getId()));
            // Do something with the value.
        }
        if (name.Equals("showZAxis"))
        {
            Debug.Log("New value for showZAxis: " + menue.getToggleValue(changedComponent.getId()));
            // Do something with the value.
        }
    }
}
