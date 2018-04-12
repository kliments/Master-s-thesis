﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/* 
  conventions:
    example of left mouse click or respective similar action with e.g., vive controller (left trigger)

    Delegate:       ***Action (LeftClickOnTargetAction)
    Event:          ***Event (LeftClickOnTargetEvent)
*/


public class InputController : MonoBehaviour {
    public delegate void LeftClickAction();
    public static event LeftClickAction LeftClickEvent;

    public delegate void LeftClickOnTargetAction(Targetable obj);
    public static event LeftClickOnTargetAction LeftClickOnTargetEvent;
    

    public enum InputEventsEnum { LeftClickEvent, LeftClickOnTargetEvent } 

    public void EmitEvent(InputEventsEnum inputEvent, Targetable target = null)
    {
        Debug.Log(inputEvent);

        switch (inputEvent)
        {
            case InputEventsEnum.LeftClickEvent:
                if (LeftClickEvent != null) LeftClickEvent();
                break;
            case InputEventsEnum.LeftClickOnTargetEvent:
                if (LeftClickOnTargetEvent != null) LeftClickOnTargetEvent(target);
                Debug.Log(target);
                break;
        }
    }

}
