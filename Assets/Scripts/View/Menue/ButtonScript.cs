using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : GenericMenueComponent {

	public Animator myAnimator;
	private GameObject KMeansOperator;

	//what happens when the button is triggered
	public void Toggle()
	{
		myAnimator.SetTrigger("press");
		KMeansOperator = GameObject.Find("KMeansClusteringOperator(Clone)");
		KMeansOperator.GetComponent<KMeansClusteringOperator>().Restart();

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
