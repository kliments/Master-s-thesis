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
    public virtual void OnLeftClickEventAction() { }


    public void OnLeftClickOnTargetEvent(Targetable target)
    {
        if (target == this) OnLeftClickOnTargetEventAction();
    }
    public virtual void OnLeftClickOnTargetEventAction() { }

}
