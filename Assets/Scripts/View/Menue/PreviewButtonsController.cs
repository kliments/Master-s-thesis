﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewButtonsController : MonoBehaviour {
    public Transform child;
    public Texture2D[] textures;
    public QualityMetricViewPort[] views;
    public QualityMetricViewPort currentView;
    public int[] order;
    public int trialNr;
    public List<GameObject> viewObjects;

    public Transform rightController;
    public Transform leftController;
    private SteamVR_TrackedObject _rightTrackedObj, _leftTrackedObject;
    private SteamVR_Controller.Device _rightDevice, _leftDevice;
    private StudieScript studieScript;
    private InstantRotationOfGraph _rotate;
    // Use this for initialization
    void Start () {
        textures = new Texture2D[3];
        views = new QualityMetricViewPort[3];
        order = new int[3] { 0, 1, 2 };

        if (Camera.main.name == "Camera (eye)")
        {
            _rightTrackedObj = rightController.GetComponent<SteamVR_TrackedObject>();
            _rightDevice = SteamVR_Controller.Input((int)_rightTrackedObj.index);
            _leftTrackedObject = leftController.GetComponent<SteamVR_TrackedObject>();
            _leftDevice = SteamVR_Controller.Input((int)_leftTrackedObject.index);
        }
        studieScript = (StudieScript)FindObjectOfType(typeof(StudieScript));
        _rotate = (InstantRotationOfGraph)FindObjectOfType(typeof(InstantRotationOfGraph));
    }
	
	// Update is called once per frame
	void Update () {
        //When grip was pressed, choose the selected view
        if((_rightDevice != null && _rightDevice.GetPressDown(SteamVR_Controller.ButtonMask.Grip)) || Input.GetMouseButtonDown(0))
        {
            //boolean value that sets true when number of trials has exceeded
            if (studieScript.dontProceed) return;
            //only if the current view is one of the 3 options
            if(currentView != null)
            {
                foreach(var view in viewObjects)
                {
                    if (view.GetComponent<QualityMetricViewPort>() == currentView)
                    {
                        if(studieScript.isTraining && trialNr > 4)
                        {
                            Debug.Log("Training limit of 5 trials reached!!");
                            Debug.Log("Please proceed with next step of the study!");

                            studieScript.ToggleTrainingSession();
                            trialNr = 0;
                            studieScript.dontProceed = true;
                            return;
                        }
                        else if (!studieScript.isTraining && trialNr > 9)
                        {
                            Debug.Log("Training limit of 10 trials reached!!");
                            Debug.Log("Please proceed with next step of the study!");

                            studieScript.ToggleTrainingSession();
                            trialNr = 0;
                            studieScript.dontProceed = true;
                            return;
                        }

                        //remove old highlighted nodes and create new ones if there are previous (only in training mode)
                        if (studieScript.isTraining)
                        {
                            //destroy previously created highlighted nodes
                            GameObject[] highlightedIcons = GameObject.FindGameObjectsWithTag("SelectedIcon");
                            if (highlightedIcons != null)
                            {
                                for (int i = highlightedIcons.Length - 1; i >= 0; i--)
                                {
                                    Destroy(highlightedIcons[i]);
                                }
                            }

                            //Create new highlighted nodes
                            studieScript.SelectRandomlyTwoNodes();
                        }
                        //Set rotation of graph back to 0
                        _rotate.SetBackToZero();

                        view.GetComponent<ReadjustQualityMetrics>().ReadjustMetrics();
                        view.GetComponent<ReadjustQualityMetrics>().AddLogDataToFile();
                        currentView = null;
                        break;
                    }
                }
            }
        }
	}

    public void CallForCreatingButtons(List<string> list, float delta)
    {
        StartCoroutine(CreateButtons(list, delta));
    }

    IEnumerator CreateButtons(List<string> list, float delta)
    {
        yield return 0;

        int i = 0;
        foreach (Transform button in child)
        {
            button.GetComponent<RawImage>().texture = textures[i];
            button.GetComponent<QualityMetricViewPort>().AssignValues(views[i]);
            button.GetComponent<ReadjustQualityMetrics>().delta = delta;
            i++;
        }
    }

    void ShuffleOrder()
    {
        System.Random rand = new System.Random();

        // For each spot in the array, pick
        // a random item to swap into that spot.
        for (int i = 0; i < order.Length - 1; i++)
        {
            int j = rand.Next(i, order.Length);
            int temp = order[i];
            order[i] = order[j];
            order[j] = temp;
        }
    }
}
