﻿using UnityEngine;

namespace Controller.Input
{
    public class InputScreenController : InputController
    {
        private RaycastHit _hit;

        private void Update()
        {

            positionLeft = UnityEngine.Input.mousePosition;

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                EmitEvent(InputEventsEnum.LeftClickEvent);
                Debug.Log(gameObject.GetInstanceID());

                var ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                if (Physics.Raycast(ray, out _hit, 100))
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


        public override Vector3 getRaycastHitOnObject(GameObject hitObject)
        {
            Debug.Log(hitObject.name);
            var ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 100);
            if (hits != null) 
            {
                foreach (var hit in hits)
                {
                    if (hit.transform.gameObject == hitObject)
                    {
                        Debug.Log("RAYCAST HIT");
                        Debug.Log(hit.point.ToString());
                        return hit.point;
                    }
                   
                }
                
            }
            return Vector3.zero;
            
        }

    }
}
