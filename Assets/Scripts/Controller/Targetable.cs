using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Model;

public abstract class Targetable : MonoBehaviour
{
    public void OnLeftClickEvent()
    {
        OnLeftClickEventAction();
    }
    protected virtual void OnLeftClickEventAction() { }


    protected void OnLeftClickReleaseEvent(Targetable target)
    {
        if (target == this) OnLeftClickReleaseEventAction();
    }
    protected virtual void OnLeftClickReleaseEventAction() { }


    protected void OnLeftClickOnTargetEvent(Targetable target)
    {
        if (target == this) OnLeftClickOnTargetEventAction();
    }
    protected virtual void OnLeftClickOnTargetEventAction() { }
    
}
