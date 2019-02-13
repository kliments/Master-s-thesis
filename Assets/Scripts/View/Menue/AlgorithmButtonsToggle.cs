using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmButtonsToggle : MonoBehaviour, IMenueComponentListener {
    public GameObject[] algorithmButtons;
    public GenericMenueComponent listener;
    private bool _areShown;
    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {

    }
    public void menueChanged(GenericMenueComponent changedComponent)
    {
         if(_areShown)
        {
            _areShown = false;
            foreach(var button in algorithmButtons)
            {
                button.SetActive(false);
            }
        }
        else
        {
            _areShown = true;
            foreach(var button in algorithmButtons)
            {
                button.SetActive(true);
            }
        }
    }

    void OnEnable()
    {
        listener.addListener(this);
    }

}
