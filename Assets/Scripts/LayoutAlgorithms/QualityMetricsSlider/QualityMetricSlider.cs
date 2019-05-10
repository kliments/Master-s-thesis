using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QualityMetricSlider : MonoBehaviour
{
    public TextMesh value;
    public float qualityFactor;
    public bool slide, repos;
    public SteamVR_TrackedObject rightController;

    private Vector3 borderPosition, mousePos, tempLocal;
    private float min, max, xPos, textValue;
    private Ray ray;
    private RaycastHit hit;
    private bool move=false;

    // Use this for initialization
    void Start () {
        borderPosition = transform.localPosition;
        min = -0.5f;
        max = 0.5f;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(slide)
        {
            if (Camera.main.name == "Camera") ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            else
            {
                ray.direction = rightController.transform.forward;
                ray.origin = rightController.transform.position;
            }
            if (!move)
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.transform == transform) move = true;
                }
            }
            if (move)
            {
                
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    mousePos.x = hit.point.x;
                    mousePos.y = transform.position.y;
                    mousePos.z = transform.position.z;

                    tempLocal.y = transform.localPosition.y;
                    tempLocal.z = transform.localPosition.z;
                }
                transform.position = mousePos;
                tempLocal.x = transform.localPosition.x;
                transform.localPosition = tempLocal;
                //limit moving the slider to the left
                if (transform.localPosition.x < -0.5f)
                {
                    borderPosition = transform.localPosition;
                    borderPosition.x = -0.5f;
                    transform.localPosition = borderPosition;
                }
                //limit moving the slider to the right
                else if (transform.localPosition.x > 0.5f)
                {
                    borderPosition = transform.localPosition;
                    borderPosition.x = 0.5f;
                    transform.localPosition = borderPosition;
                }
                else
                {
                    borderPosition.x = transform.localPosition.x;
                    transform.localPosition = borderPosition;
                }
            }
            qualityFactor = NormalizedSliderValue();
            textValue = (float)(Math.Round(qualityFactor, 1));
            value.text = textValue.ToString();
        }
        else
        {
            if (move) move = false;
        }
        if(repos)
        {
            repos = false;
            MoveSlideFromValue(qualityFactor);
        }
    }

    public float NormalizedSliderValue()
    {
        return ((transform.localPosition.x - min) / max - min) - 0.5f;
    }

    public void MoveSlideFromValue(float weight)
    {
        weight = (float)(Math.Round(weight, 1));
        Vector3 newPos = transform.localPosition;
        newPos.x = weight/2 + min;
        transform.localPosition = newPos;
        value.text = weight.ToString();
    }
}

