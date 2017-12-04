using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    public delegate void ClickAction(ITargetable obj);
    public static event ClickAction OnClicked;

    public void EmitOnClickedEvent(ITargetable obj)
    {
        if (OnClicked != null)
        {
            OnClicked(obj);
        }
        
    }

}
