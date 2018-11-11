//using GracesGames.SimpleFileBrowser.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectoryButtonScript : GenericMenueComponent {
    private List<IMenueComponentListener> listeners;
    //private UserInterface UI;
    // Use this for initialization
    void Start ()
    {
       // UI = (UserInterface)(FindObjectOfType(typeof(UserInterface)));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDisable()
    {
        //remove listeners when directory button is destroyed
        listeners = getListeners();
        for(int i=listeners.Count; i>0; i--)
        {
            removeListener(listeners[i - 1]);
            listeners = getListeners();
        }
        //if(UI!=null) UI.dirList.Remove(gameObject);
    }
}
