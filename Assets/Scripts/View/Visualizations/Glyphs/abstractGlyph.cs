using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class abstractGlyph : MonoBehaviour {

	public abstract void setValues(float[] Values);

    public void facePlayer(Transform target)
    {
        //this.transform.LookAt(target);
        this.transform.LookAt(target);
    }

    
}
