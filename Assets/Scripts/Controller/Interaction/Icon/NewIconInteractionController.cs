using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewIconInteractionController : GenericIconInteractionController {

    public override void OnLeftClickOnTargetEventAction()
    {
        Debug.Log("New Icon OnLeftClickEvent");
    }
}
