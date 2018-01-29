using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class UiController : VRTK_InteractableObject
{

    public GameObject ButtonManager;

    //	private List<object> Buttons;

    private static bool _buttonsAreDisabled;
    private static Button[] _radialButtons;

    private void Awake()
    {
        //put all UI buttons into a list for later reference
        _radialButtons = FindObjectsOfType<Button>();
        //Debug.Log("das hier wird ausgeführt");


        //		foreach (var child in ButtonManager.transform)
        //		{
        ////			Debug.Log(child + " gefunden");
        //			
        //			
        ////			string nameX = child.ToString();
        ////			GameObject buttonTest = GameObject.Find(nameX);
        ////			Debug.Log(buttonTest.ToString());
        ////			Vector3 pos = buttonTest.transform.position;
        ////			Vector3 pos = GameObject.Find(nameX).transform.position;
        ////			pos.y -= -1f;
        ////			GameObject.Find(nameX).transform.position = pos;
        //			
        //			
        ////			Buttons.Add(child);
        ////				
        ////			
        ////			
        ////			Debug.Log(Buttons);
        //		}


    }

    // Use this for initialization
    private void Start()
    {
        //let the buttons be added to the list but then immediately disable them because they should be in the beginning
        //they have to be active at the start because otherwise you can't grab them for the list
        ButtonSwitch();
    }

    	public override void StartUsing(VRTK_InteractUse usingObject)
    	{
    		base.StartUsing(usingObject);
    		ButtonSwitch();
    	}
    	
    	public override void StopUsing(VRTK_InteractUse usingObject)
    	{
    		base.StopUsing(usingObject);
    		ButtonSwitch();
    	}

    //Switch enabled/disabled state of the UI buttons, need to disable all components separately apparently or I'm too dumb (likely)
    public static void ButtonSwitch()
    {
        if (!_buttonsAreDisabled)
        {
            foreach (var button in _radialButtons)
            {
                button.enabled = false;
                button.GetComponent<Image>().enabled = false;
                button.GetComponentInChildren<Text>().enabled = false;
                _buttonsAreDisabled = true;
            }
        }
        else
        {
            foreach (var button in _radialButtons)
            {
                button.enabled = true;
                button.GetComponent<Image>().enabled = true;
                button.GetComponentInChildren<Text>().enabled = true;
                _buttonsAreDisabled = false;
            }
        }


    }
}
