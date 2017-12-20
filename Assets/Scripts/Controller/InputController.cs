using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    public delegate void ClickAction(Targetable obj);
    public static event ClickAction OnClicked;

    public void EmitOnClickedEvent(Targetable obj)
    {
        if (OnClicked != null)
        {
            OnClicked(obj);
        }
        
    }

}
