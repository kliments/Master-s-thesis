using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class middle_marker : MonoBehaviour {

    public bool visible = false;
    private float targetSize = 0.01f;

    // Use this for initialization
    void Start()
    {
        updateSize();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateSize()
    {
        if (!visible)
        {
            this.transform.localScale = new Vector3(0, 0, 0);
        }
        else
        {
            Vector3 parentSize = this.transform.parent.transform.lossyScale;
            Vector3 mySize = new Vector3((targetSize / parentSize.x), (targetSize / parentSize.y), (targetSize / parentSize.z));
            this.transform.localScale = mySize;
        }


    }
}
