using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : GenericMenueComponent {

//	public Animator myAnimator;
	public Material PressedMaterial;

	private Transform Button;

	private void Start()
	{
		Button = gameObject.transform.GetChild(1);
	}

	//what happens when the button is triggered
	public void Toggle()
	{
		Debug.Log("toggle");
		//at first toggle the color of the button
		var oldMaterial = Button.GetComponent<Renderer>().sharedMaterial;
		Button.GetComponent<Renderer>().material = PressedMaterial;
		
		//then toggle the animation
		// TODO: animation
		List<IMenueComponentListener> listeners = getListeners();
		foreach(IMenueComponentListener listener in listeners)
		{
			listener.menueChanged(this);
		}
		
	}

	//probably is useless?
//	public int isActivated()
//	{
////		return myAnimator.GetInteger("IsPressed");
//	}

}
