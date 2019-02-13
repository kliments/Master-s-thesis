using System;
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

    public delegate void LeftClickReleaseAction();
    public static event LeftClickReleaseAction LeftClickReleaseEvent;

    public delegate void LeftClickOnTargetAction(Targetable obj);
    public static event LeftClickOnTargetAction LeftClickOnTargetEvent;

    public Vector3 positionLeft = new Vector3();
    public Vector3 positionRight = new Vector3();


    public enum InputEventsEnum { LeftClickEvent, LeftClickReleaseEvent, LeftClickOnTargetEvent } 

    public void EmitEvent(InputEventsEnum inputEvent, Targetable target = null)
    {

        switch (inputEvent)
        {
            case InputEventsEnum.LeftClickEvent:
                if (LeftClickEvent != null) LeftClickEvent();
                break;
            case InputEventsEnum.LeftClickReleaseEvent:
                if (LeftClickReleaseEvent != null) LeftClickReleaseEvent();
                break;
            case InputEventsEnum.LeftClickOnTargetEvent:
                if (LeftClickOnTargetEvent != null) LeftClickOnTargetEvent(target);
                //Debug.Log(target+ " has been clicked.");
                break;
        }
    }

}
