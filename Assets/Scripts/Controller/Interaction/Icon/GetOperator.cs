using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class GetOperator : MonoBehaviour {

    private static VRTK_Pointer pointer;
    public static GameObject hitObject;

    private void Start()
    {
        pointer = GetComponent<VRTK_Pointer>();
    }

    private void Update()
    {
        GetObjectByRaycast();
    }

    public static GameObject GetObjectByRaycast ()
    {
        hitObject = pointer.pointerRenderer.GetDestinationHit().transform.gameObject;
        //Debug.Log(hitObject.name.ToString());
        return hitObject;
        // TODO hitobject auf static op in newIconInteractionController umspeichern sodass
        //      der operator bei klick mit dem vive controller abgefangen werden kann
    }



}
