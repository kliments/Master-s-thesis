using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ModeEventListener : MonoBehaviour {
    
    //internal variable for the action to perform
    public MENU_ACTION menuAction = MENU_ACTION.SELECT;

    //parent of tree for scaling, moving and rotating
    public GameObject treeParent;
    //layers to ignore in modify actions and select action
    public LayerMask layersToIgnoreModify;
    public LayerMask layersToIgnoreSelect;
    VRTK_SimplePointer pointer;
    VRTK_ControllerEvents controller;
    private SteamVR_TrackedObject _trackedObj;
    private SteamVR_Controller.Device _device;
    private LayoutAlgorithm layout;
    private ConeTreeAlgorithm alg = new ConeTreeAlgorithm();
    bool fixSelection = false;

    // Indicates whether the rotation of the selected object should be updated
    bool updateRotation = false;
    Vector3 initialRotationEulerAngles;
    Vector3 initialRotationDiff;

    // Indicates whether the scaling of the selected object should be updated
    bool updateScaling = false;
    Vector3 initialPosition;
    Vector3 initialScale;

    GameObject selection = null;
    // sets the correct menuaction
    public MENU_ACTION MenuAction
    {
        get { return menuAction; }
        set
        {
            switch(value)
            {
                case MENU_ACTION.MOVE:
                    SetMoveMode();
                    break;
                case MENU_ACTION.ROTATE:
                    SetRotateMode();
                    break;
                case MENU_ACTION.SCALE:
                    SetScaleMode();
                    break;
                default:
                    SetSelectMode();
                    break;
            }
        }
    }
	// Use this for initialization
	void Start ()
    {

        pointer = GetComponent<VRTK_SimplePointer>();
        controller = GetComponent<VRTK_ControllerEvents>();

        //Setup pointer event listeners
        pointer.DestinationMarkerEnter += new DestinationMarkerEventHandler(selectObject);
        pointer.DestinationMarkerExit += new DestinationMarkerEventHandler(deselectObject);
        
        //Setup controller button event Listener;
        controller.TriggerPressed += new ControllerInteractionEventHandler(triggerPressed);
        controller.TriggerReleased += new ControllerInteractionEventHandler(triggerReleased);

        _trackedObj = GetComponent<SteamVR_TrackedObject>();
        _device = SteamVR_Controller.Input((int)_trackedObj.index);

        layout = (LayoutAlgorithm)FindObjectOfType(typeof(LayoutAlgorithm));
    }
	
	// Update is called once per frame
	void Update () {
        //adapt rotation of the selection to track the parent
        if (updateRotation)
        {
            selection.transform.rotation = Quaternion.FromToRotation(initialRotationDiff, selection.transform.position - gameObject.transform.position) * Quaternion.Euler(initialRotationEulerAngles);
        }

        //adapt scaling based on the difference in height from the original starting point of the selection
        if (updateScaling)
        {
            float scaleDiff = 1 + (gameObject.transform.position.y - initialPosition.y) * 1.2f;
            selection.transform.localScale = initialScale * scaleDiff;
        }

        if(_device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (menuAction == MENU_ACTION.MOVE || menuAction == MENU_ACTION.SCALE || menuAction == MENU_ACTION.ROTATE)
            {
                selectObject();
            }
        }

        if(_device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (menuAction == MENU_ACTION.MOVE || menuAction == MENU_ACTION.SCALE || menuAction == MENU_ACTION.ROTATE)
            {
                if (layout.currentLayout.GetType() == typeof(RDTAlgorithm))
                {
                    layout.currentLayout.GetComponent<RDTAlgorithm>().CalculateRDT();
                }
            }
        }
    }

    private void selectObject()
    {
        selection = treeParent;
        return;
    }

    private void selectObject(object sender, DestinationMarkerEventArgs e)
    {

        if (menuAction == MENU_ACTION.MOVE || menuAction == MENU_ACTION.SCALE || menuAction == MENU_ACTION.ROTATE)
        {
            selection = treeParent;
            return;
        }
        if (fixSelection)
        {
            return;
        }

        if (!e.target)
        {
            selection = null;
            return;
        }
        selection = e.target.gameObject;

    }

    private void deselectObject(object sender, DestinationMarkerEventArgs e)
    {
        if (fixSelection)
        {
            return;
        }

        selection = null;
    }

    private void triggerPressed(object sender, ControllerInteractionEventArgs e)
    {
        if (selection == null)
        {
            return;
        }

        switch (menuAction)
        {
            case MENU_ACTION.MOVE:
                moveSelection(selection, true);
                break;
            case MENU_ACTION.ROTATE:
                rotateSelection(selection, true);
                break;
            case MENU_ACTION.SCALE:
                scaleSelection(selection, true);
                break;
            default:
                break;
        }
    }

    private void triggerReleased(object sender, ControllerInteractionEventArgs e)
    {
        if (selection == null)
        {
            return;
        }

        switch (menuAction)
        {
            case MENU_ACTION.MOVE:
                moveSelection(selection, false);
                break;
            case MENU_ACTION.ROTATE:
                rotateSelection(selection, false);
                break;
            case MENU_ACTION.SCALE:
                scaleSelection(selection, false);
                break;
            default:
                break;
        }
    }

    // Starts or Ends the movement of a selected object by parenting it to the controller
    private void moveSelection(GameObject selectedObject, bool start)
    {
        if (selectedObject.transform.parent.name == "Icon") return;
        if (start)
        {
            selectedObject.transform.parent.transform.parent = gameObject.transform;
            //disallow the changing of the selection while the move is progress
            fixSelection = true;
        }
        else
        {
            selectedObject.transform.parent.transform.parent = null;
            //free selection again
            fixSelection = false;
        }
    }

    // Starts or Ends the rotation of a selected object by having it track the location of the controller
    private void rotateSelection(GameObject selectedObject, bool start)
    {
        if (selectedObject.transform.parent.name == "Icon") return;
        if (start)
        {
            //disallow the changing of the selection while the move is progress
            fixSelection = true;
            //enable updating the rotation
            updateRotation = true;
            //set the original rotation vectors
            initialRotationEulerAngles = selectedObject.transform.rotation.eulerAngles;
            initialRotationDiff = selectedObject.transform.position - gameObject.transform.position;
        }
        else
        {
            //free selection again
            fixSelection = false;
            //disable updatin the roation
            updateRotation = false;
        }
    }

    // Starts or Ends the rotation of a selected object by tracking the height comoponent of the controller
    private void scaleSelection(GameObject selectedObject, bool start)
    {
        if (selectedObject.transform.parent.name == "Icon") return;
        if (start)
        {
            //dissalow the changing of the selection while the scaling is in process
            fixSelection = true;
            //enable updating the rotation
            updateScaling = true;
            //inital position of the controller (to compare the 
            initialPosition = gameObject.transform.position;
            initialScale = selectedObject.transform.localScale;
        }
        else
        {
            //free selection again
            fixSelection = false;
            //disable scale update
            updateScaling = false;
        }
    }

    public void SetMoveMode()
    {
        menuAction = MENU_ACTION.MOVE;
        GetComponent<VRTK_SimplePointer>().layersToIgnore = layersToIgnoreModify;
    }
    public void SetRotateMode()
    {
        menuAction = MENU_ACTION.ROTATE;
        GetComponent<VRTK_SimplePointer>().layersToIgnore = layersToIgnoreModify;
    }
    public void SetScaleMode()
    {
        menuAction = MENU_ACTION.SCALE;
        GetComponent<VRTK_SimplePointer>().layersToIgnore = layersToIgnoreModify;
    }
    public void SetSelectMode()
    {
        menuAction = MENU_ACTION.SELECT;
        GetComponent<VRTK_SimplePointer>().layersToIgnore = layersToIgnoreSelect;
    }
}
