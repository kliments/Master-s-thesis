using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScript : GenericMenueComponent
{
    public Animator myAnimator;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

   public bool toggle()
    {
        bool activated = myAnimator.GetBool("isPressed");
        myAnimator.SetBool("isPressed", !activated);
        List<IMenueComponentListener> listeners = getListeners();
        foreach(IMenueComponentListener listener in listeners)
        {
            listener.menueChanged(this);
        }
        return !activated;
    }

    public bool isActivated()
    {
        return myAnimator.GetBool("isPressed");
    }

    public override void CloseAllMenus()
    {
        List<IMenueComponentListener> listeners = getListeners();
        foreach (IMenueComponentListener listener in listeners)
        {
            listener.CloseAllMenus();
        }
    }
}
