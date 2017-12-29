using UnityEngine;

namespace Controller.Interaction.Icon
{
    public class DataLoaderInteractionController : GenericIconInteractionController {
        protected override void OnLeftClickOnTargetEventAction()
        {
            Debug.Log("Dataloader Icon OnLeftClickEvent");
        }
    }
}
