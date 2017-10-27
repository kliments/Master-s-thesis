using UnityEngine;
using System.Collections;

public abstract class ITargetable : MonoBehaviour
{
   /* void startTargetedAction(InputViveController rightViveController);         // Called on trigger down.
    void endTargetedAction(InputViveController rightViveController);                                           // Called after startTargetedAction when there is another trigger down and this object is not targeted.
    void endTargetedActionOnTarget(InputViveController rightViveController);
    void targetEnteredAction(InputViveController rightViveController);                                         // Called like a mouseover event. Only fired once before a targetExitedAction is started.
    void targetExitedAction(InputViveController rightViveController);                                          // Called like a mouseout event.
    void startTargetedActionTouchpad(InputViveController rightViveController); // Called on touchpad touch
    */

    void OnEnable()
    {
        InputController.OnClicked += targetClicked;
    }


    void OnDisable()
    {
        InputController.OnClicked -= targetClicked;
    }

    void targetClicked(ITargetable t)
    {
        if (t == this)
        {
            onClicked();
        }
    }

    public abstract void onClicked();

}
