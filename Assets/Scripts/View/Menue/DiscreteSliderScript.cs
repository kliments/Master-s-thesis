using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.UI;

public class DiscreteSliderScript : GenericMenueComponent {

	public Animator myAnimator;
	public Text NumberField; //the text field on the toggle button that indicates the current value of KMean parameter K
	private int _counter = 2; //the counter that gets written into the text field
	private KMeansClusteringOperator _kmeansClusteringOperator;

	//what happens when the button is triggered
	public void toggle()
	{
		ToggleSliderNumber();
		int transitionNumber = myAnimator.GetInteger("IsPressed");
		if (transitionNumber < 5)
		{
			myAnimator.SetInteger("IsPressed", _counter);
		}
		else
		{
			transitionNumber = 2;
			myAnimator.SetInteger("IsPressed", transitionNumber);
		}
	
		List<IMenueComponentListener> listeners = getListeners();
		foreach(IMenueComponentListener listener in listeners)
		{
			listener.menueChanged(this);
		}
	}

	//probably is useless?
	public int isActivated()
	{
		return myAnimator.GetInteger("IsPressed");
	}

	//toggle text field number
	private void ToggleSliderNumber()
	{
		if (_counter < 5)
		{
			_counter++;
		}
		else if (_counter == 5)
		{
			_counter = 2;
		}
		
		NumberField.text = _counter.ToString();
		
		_kmeansClusteringOperator = Observer.selectedOperator.GetComponent<KMeansClusteringOperator>();
		_kmeansClusteringOperator.K = _counter;
	}
}
