using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewportMarkerButtonComponent : Targetable {
    public GameObject cameraRig, cameraEye;
    public float counter;
    public QualityMetricViewPort viewPort;
    public ViewPortOptimizer optimizer;
    public bool smoothTransition;

    private Observer observer;
    private LayoutAlgorithm current;
    private Vector3 distFromCameraToRig;
    private ViewportMarkerButtonComponent[] list;
    // Use this for initialization
    void Start () {
        if (cameraRig == null)
        {
            cameraRig = GameObject.Find("[CameraRig]");
        }
        if (cameraEye == null)
        {
            cameraEye = GameObject.Find("Camera (eye)");
        }
        current = ((LayoutAlgorithm)FindObjectOfType(typeof(LayoutAlgorithm)));
        observer = (Observer)FindObjectOfType(typeof(Observer));
        list = (ViewportMarkerButtonComponent[])FindObjectsOfType(typeof(ViewportMarkerButtonComponent));
    }

    // Update is called once per frame
    void Update ()
    {
        transform.LookAt(Camera.main.transform);
        if (smoothTransition)
        {
            if (Camera.main.name != "Camera")
            {
                distFromCameraToRig = cameraRig.transform.position - cameraEye.transform.position;
                cameraRig.transform.position = Vector3.Lerp(cameraRig.transform.position, transform.position + distFromCameraToRig, Time.deltaTime * 2f);
                optimizer.FindCenterOfGraph();
                if (Vector3.Distance(cameraRig.transform.position, transform.position + distFromCameraToRig) < 0.025f)
                {
                    smoothTransition = false;
                    cameraRig.transform.position = transform.position + distFromCameraToRig;
                    if (current.currentLayout != viewPort.algorithm)
                    {
                        current.currentLayout = viewPort.algorithm;
                        for (int i = 0; i < observer.GetOperators().Count; i++)
                        {
                            observer.GetOperators()[i].GetIcon().GetComponent<IconProperties>().newPos = viewPort.listPos[i];
                            observer.GetOperators()[i].GetIcon().GetComponent<IconProperties>().repos = true;
                        }
                        if (current.currentLayout.GetType().Equals(typeof(RDTAlgorithm))) optimizer.CalculateRDT();
                        else current.currentLayout.PlaceEdges();
                    }
                }
            }
            else
            {
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position, Time.deltaTime * 2f);
                optimizer.FindCenterOfGraph();
                if (Vector3.Distance(Camera.main.transform.position, transform.position) < 0.025f)
                {
                    smoothTransition = false;
                    Camera.main.transform.position = transform.position;
                    if (current.currentLayout != viewPort.algorithm)
                    {
                        current.currentLayout = viewPort.algorithm;
                        for (int i = 0; i < observer.GetOperators().Count; i++)
                        {
                            observer.GetOperators()[i].GetIcon().GetComponent<IconProperties>().newPos = viewPort.listPos[i];
                            observer.GetOperators()[i].GetIcon().GetComponent<IconProperties>().repos = true;
                        }
                        if (current.currentLayout.GetType().Equals(typeof(RDTAlgorithm))) optimizer.CalculateRDT();
                        else current.currentLayout.PlaceEdges();
                    }
                }
                Camera.main.transform.LookAt(optimizer.transform);
                optimizer.CentralizeCamera();
            }
        }

    }
    private void OnEnable()
    {
        InputController.LeftClickOnTargetEvent += OnLeftClickOnTargetEvent;
    }
    private void OnDisable()
    {
        InputController.LeftClickOnTargetEvent -= OnLeftClickOnTargetEvent;
    }
    protected override void OnLeftClickOnTargetEventAction()
    {
        foreach(var item in list)
        {
            item.smoothTransition = false;
        }
        smoothTransition = true;
    }
}
