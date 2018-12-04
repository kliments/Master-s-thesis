using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class InputViveController : InputController
{
    private SteamVR_TrackedObject _trackedObj;
    private SteamVR_Controller.Device _device;
    private int _leftControllerIndex;
    private int _rightControllerIndex;
    private Targetable _lastTarget = null;
    private ModeEventListener mode;
    
    private RaycastHit _hit;

    private void Awake()
    {
        _trackedObj = GetComponent<SteamVR_TrackedObject>();
        mode = (ModeEventListener)FindObjectOfType(typeof(ModeEventListener));
    }

    private void Start()
    {

        if (GetComponent<SteamVR_LaserPointer>() != null)
        {
            _rightControllerIndex = (int)_trackedObj.index;
        }
        else
        {
            _leftControllerIndex = (int)_trackedObj.index;
        }
        _device = SteamVR_Controller.Input((int)_trackedObj.index);
    }

    private void Update()
    {
       
        var forward = _trackedObj.transform.TransformDirection(Vector3.forward);
        
        if (_device.index == _rightControllerIndex)
        {
            
            var fwd = _trackedObj.transform.TransformDirection(Vector3.forward);

            if (Physics.Raycast(_trackedObj.transform.position, fwd, out _hit, 60))
            {
                var target = _hit.transform.gameObject.GetComponent<Targetable>();
                if (target != null)
                {
                    _lastTarget = target;
                    if (_device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && mode.menuAction == MENU_ACTION.SELECT)
                    {
                        EmitEvent(InputEventsEnum.LeftClickOnTargetEvent, target);
                    }
                }
                else
                {                 
                   _lastTarget = null;                   
                }

            }
        }
    }
}
