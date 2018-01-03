using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{

	public GameObject ButtonManager;
//	private List<object> Buttons;
	private bool _buttonsAreDisabled;
	private Button[] _radialButtons;

	private void Awake()
	{
		//put all UI buttons into a list for later reference
		_radialButtons = FindObjectsOfType<Button>();
		
		
		
		
		
		
		
		
		
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
	private void Start ()
	{
		ButtonSwitch();
	}
	
	// Update is called once per frame
	void Update () {		
		
	}
	
	//Switch enabled/disabled state of the UI buttons, need to disable all components separately apparently or I'm too dumb (likely)
	private void ButtonSwitch()
	{
		if (!_buttonsAreDisabled)
		{
			foreach (var button in _radialButtons)
			{			
//				Debug.Log(button);
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
