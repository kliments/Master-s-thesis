using System.Collections;
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
        //When grip was pressed
        if(_rightDevice.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            //only if the current view is one of the 3 options
            if(currentView != null)
            {
                foreach(var view in viewObjects)
                {
                    if (view.GetComponent<QualityMetricViewPort>() == currentView)
                    {
                        if(studieScript.isTraining && trialNr >= 4)
                        {
                            Debug.Log("Reached limit of 10 steps");
                            Debug.Log("Please proceed with next step of the study!");

                            studieScript.ToggleTrainingSession();
                            trialNr = 0;
                            return;
                        }
                        else if (!studieScript.isTraining && trialNr >= 9)
                        {
                            Debug.Log("Reached limit of 10 steps");
                            Debug.Log("Please proceed with next step of the study!");

                            studieScript.ToggleTrainingSession();
                            trialNr = 0;
                            return;
                        }

                        //Set rotation of graph back to 0
                        _rotate.SetBackToZero();

                        view.GetComponent<ReadjustQualityMetrics>().ReadjustMetrics();
                        view.GetComponent<ReadjustQualityMetrics>().AddLogDataToFile();
                        currentView = null;
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
