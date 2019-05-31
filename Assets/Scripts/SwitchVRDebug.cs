using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class SwitchVRDebug : MonoBehaviour
{

	public GameObject VRCamera;
	public GameObject NormalCamera;
    public Transform Menue;
    public Transform controller;
    public Texture2D texture;

    private Vector3 controllerPos = new Vector3(0, 0.047f, 0.2f);
    private Vector3 outsidePos = new Vector3(0, 2.308f, 0.951f);
    private Vector3 controllerScale = new Vector3(0.2f, 0.2f, 0.2f);
    private Vector3 outsideScale = new Vector3(1,1,1);
    private Vector3 controllerRotation = new Vector3(90, 0, 0);
    private Vector3 outsideRotation = new Vector3(0, 0, 0);
	
	private void OnGUI()
	{
		if (GUI.Button(new Rect(10, 10, 100, 40), "2D"))
		{
			UnityEngine.XR.XRSettings.enabled = false;
			NormalCamera.SetActive(true);
			VRCamera.SetActive(false);
            Menue.parent = null;
            Menue.localPosition = outsidePos;
            Menue.localScale = outsideScale;
            Menue.eulerAngles = outsideRotation;
        }
		
		if (GUI.Button(new Rect(10, 60, 100, 40), "VR"))
		{
			UnityEngine.XR.XRSettings.enabled = true;
			NormalCamera.SetActive(false);
			VRCamera.SetActive(true);
            Menue.parent = controller;
            Menue.localPosition = controllerPos;
            Menue.localScale = controllerScale;
            Menue.localEulerAngles = controllerRotation;
        }
	}
}
