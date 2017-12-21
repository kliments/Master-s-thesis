using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericIconInputController : Targetable {

    void OnEnable()
    {
        InputController.LeftClickOnTargetEvent += OnLeftClickOnTargetEvent;
    }

    void OnDisable()
    {
        InputController.LeftClickOnTargetEvent -= OnLeftClickOnTargetEvent;
    }
}
