using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.UI;

public class InputfieldScript : GenericMenueComponent
{

	private KMeansClusteringOperator _kmeansClusteringOperator;

	//what happens when the button is triggered
	public void Toggle()
	{
		var textObject = gameObject.transform.GetChild(1).GetComponent<Text>();
		var input = int.Parse(textObject.text);				
		_kmeansClusteringOperator = Observer.selectedOperator.GetComponent<KMeansClusteringOperator>();

		_kmeansClusteringOperator.K = input;
		
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

