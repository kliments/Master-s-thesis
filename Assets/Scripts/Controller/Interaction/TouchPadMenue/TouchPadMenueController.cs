using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum MENU_ACTION
{
    MOVE,
    ROTATE,
    SCALE,
    SELECT
}

public class TouchPadMenueController : MonoBehaviour {
    public UnityEvent moveListeners;
    public UnityEvent rotateListeners;
    public UnityEvent scaleListeners;
    public UnityEvent selectListeners;

    public void MoveModeSelected()
    {
        moveListeners.Invoke();
    }

    public void RotateModeSelected()
    {
        rotateListeners.Invoke();
    }

    public void ScaleModeSelected()
    {
        scaleListeners.Invoke();
    }

    public void SelectModeSelected()
    {
        selectListeners.Invoke();
    }
}
