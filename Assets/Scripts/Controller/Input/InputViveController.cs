using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class InputViveController : InputController
{
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
    private int left_controller_index;
    private int right_controller_index;
    private Targetable lastTarget = null;
    
    private RaycastHit hit;

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Start()
    {

        if (GetComponent<SteamVR_LaserPointer>() != null)
        {
            right_controller_index = (int)trackedObj.index;
        }
        else
        {
            left_controller_index = (int)trackedObj.index;
        }
        device = SteamVR_Controller.Input((int)trackedObj.index);
    }
    
    void FixedUpdate()
    {
       
        Vector3 forward = trackedObj.transform.TransformDirection(Vector3.forward);
        
        if (device.index == right_controller_index)
        {
            
            Vector3 fwd = trackedObj.transform.TransformDirection(Vector3.forward);

            if (Physics.Raycast(trackedObj.transform.position, fwd, out hit, 60))
            {
                Targetable target = hit.transform.gameObject.GetComponent<Targetable>();
                if (target != null)
                {
                    lastTarget = target;
                    if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
                    {
                       EmitOnClickedEvent(target);
                    }
                }
                else
                {
                    if (lastTarget != null)
                    {
                        lastTarget = null;
                    }
                }

            }
        }
    }
}
