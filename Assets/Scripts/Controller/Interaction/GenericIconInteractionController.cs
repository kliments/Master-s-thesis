using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericIconInteractionController : Targetable {

    void OnEnable()
    {
        InputController.LeftClickOnTargetEvent += OnLeftClickOnTargetEvent;
    }

    void OnDisable()
    {
        InputController.LeftClickOnTargetEvent -= OnLeftClickOnTargetEvent;
    }
}
