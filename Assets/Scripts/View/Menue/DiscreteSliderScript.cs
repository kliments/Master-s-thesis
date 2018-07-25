using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscreteSliderScript : GenericMenueComponent {

	public Animator myAnimator;
	public Text NumberField;

	private int counter = 2;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int toggle()
	{
		bool activated = myAnimator.GetBool("isPressed");
		myAnimator.SetBool("isPressed", !activated);
		List<IMenueComponentListener> listeners = getListeners();
		
		foreach(IMenueComponentListener listener in listeners)
		{
			listener.menueChanged(this);
		}
		
		ToggleSliderNumber();
		return counter;
	}

	public bool isActivated()
	{
		return myAnimator.GetBool("isPressed");
	}

	private void ToggleSliderNumber()
	{
		if (counter < 3)
		{
			counter++;
		}
		else if (counter == 3)
		{
			counter = 2;
		}
		
		NumberField.text = counter.ToString();
	}
}
