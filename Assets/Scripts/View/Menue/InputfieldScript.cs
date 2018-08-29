using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputfieldScript : GenericMenueComponent {



	//what happens when the button is triggered
	public void Toggle()
	{
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

