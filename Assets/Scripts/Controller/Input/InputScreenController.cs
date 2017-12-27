using UnityEngine;

namespace Controller.Input
{
    public class InputScreenController : InputController
    {
        private RaycastHit _hit;

        private void FixedUpdate()
        {
            if(UnityEngine.Input.GetMouseButtonDown(0))
            {
                EmitEvent(InputEventsEnum.LeftClickEvent);

                var ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                if (Physics.Raycast(ray, out _hit, 100))
                {
                    var target = _hit.transform.gameObject.GetComponent<Targetable>();
                    if (target != null) EmitEvent(InputEventsEnum.LeftClickOnTargetEvent, target);
                }
            }
        
        }
    
    }
}
