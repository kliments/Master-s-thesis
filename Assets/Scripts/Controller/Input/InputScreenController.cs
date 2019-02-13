using UnityEngine;

namespace Controller.Input
{
    public class InputScreenController : InputController
    {
        private RaycastHit _hit;
        private int layerMask;

        private void Start()
        {
            layerMask = ~(1 << 9);
        }

        private void Update()
        {

            positionLeft = UnityEngine.Input.mousePosition;

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                EmitEvent(InputEventsEnum.LeftClickEvent);

                var ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                if (Physics.Raycast(ray, out _hit, 100, layerMask))
                {
                    var target = _hit.transform.gameObject.GetComponent<Targetable>();
                    if (target != null) EmitEvent(InputEventsEnum.LeftClickOnTargetEvent, target);
                }
            }

            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                EmitEvent(InputEventsEnum.LeftClickReleaseEvent);
            }


        }
    
    }
}
