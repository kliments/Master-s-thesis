using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputScreenController : InputController
{
    private RaycastHit hit;

    void FixedUpdate()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.Log(ray);
            if (Physics.Raycast(ray, out hit, 100))
            {
                Targetable target = hit.transform.gameObject.GetComponent<Targetable>();
                // if (target == null && hit.transform.parent != null)
                // {
                //     target = hit.transform.parent.gameObject.GetComponent<Targetable>();
                // }
                Debug.Log(target);
                if (target != null)
                {
                    Debug.Log("asdfsdafasdfadsfa");
                    EmitOnClickedEvent(target);
                }
            }
        }
        
    }
    
}
