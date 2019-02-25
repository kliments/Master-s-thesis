using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonsToggle : MonoBehaviour, IMenueComponentListener {
    public GameObject menuButtonsContainer;
    public GenericMenueComponent listener;
    private bool _areShown = false;
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
            menuButtonsContainer.SetActive(false);
        }
        else
        {
            _areShown = true;
            menuButtonsContainer.SetActive(true);
        }
    }

    void OnEnable()
    {
        listener.addListener(this);
    }

    public void CloseAllMenus()
    {
        _areShown = false;
        menuButtonsContainer.SetActive(false);
    }

}
