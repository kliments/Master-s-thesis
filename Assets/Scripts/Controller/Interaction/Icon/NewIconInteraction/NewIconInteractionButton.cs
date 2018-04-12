using System.Collections;
using System.Collections.Generic;
using Controller.Interaction.Icon;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewIconInteractionButton : Targetable
{

    public NewIconInteractionController controller;
    public int id;

    private void OnEnable()
    {
        InputController.LeftClickOnTargetEvent += OnLeftClickOnTargetEvent;
    }

    private void OnDisable()
    {
        InputController.LeftClickOnTargetEvent -= OnLeftClickOnTargetEvent;
    }

    protected override void OnLeftClickOnTargetEventAction()
    {
        Debug.Log("click on button"+id);
        controller.spawnPrefab(id);
    }
}
