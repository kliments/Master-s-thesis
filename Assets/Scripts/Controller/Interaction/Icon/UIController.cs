using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{

	public GameObject ButtonManager;
	private List<GameObject> Buttons;

	private void Awake()
	{
		foreach (var child in ButtonManager.transform)
		{
			Debug.Log("kind gefunden");
			if ()
		}
	}

	// Use this for initialization
	void Start () {
//		Debug.Log(Buttons[0].ToString());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
