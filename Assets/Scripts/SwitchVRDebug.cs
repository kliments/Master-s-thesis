using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class SwitchVRDebug : MonoBehaviour
{

	public GameObject VRCamera;
	public GameObject NormalCamera;
	
	private void OnGUI()
	{
		if (GUI.Button(new Rect(10, 10, 100, 40), "2D"))
		{
			UnityEngine.XR.XRSettings.enabled = false;
			NormalCamera.SetActive(true);
			VRCamera.SetActive(false);
		}
		
		if (GUI.Button(new Rect(10, 60, 100, 40), "VR"))
		{
			UnityEngine.XR.XRSettings.enabled = true;
			NormalCamera.SetActive(false);
			VRCamera.SetActive(true);
		}
	}
}
