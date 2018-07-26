using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscreteSliderScript : GenericMenueComponent {

	public Animator myAnimator;
	public Text NumberField;

	private int _counter = 2;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public bool toggle()
	{
		bool activated = myAnimator.GetBool("Clicked");
		myAnimator.SetBool("Clicked", !activated);
		List<IMenueComponentListener> listeners = getListeners();
		foreach(IMenueComponentListener listener in listeners)
		{
			listener.menueChanged(this);
		}
		ToggleSliderNumber();
		return !activated;
	}

	public bool isActivated()
	{
		return myAnimator.GetBool("Clicked");
	}

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
		Debug.Log(_counter);
		
		KMeansClusteringOperator._k = _counter;
		Debug.Log(KMeansClusteringOperator._k);
	}
}
