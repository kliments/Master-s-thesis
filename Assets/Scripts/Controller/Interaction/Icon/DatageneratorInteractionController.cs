using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Controller.Interaction.Icon
{
    public class DatageneratorInteractionController : GenericIconInteractionController
    {

        protected override void OnLeftClickOnTargetEventAction()
        {
            Debug.Log("Datagenerator Icon OnLeftClickEvent");
        }
    }
}