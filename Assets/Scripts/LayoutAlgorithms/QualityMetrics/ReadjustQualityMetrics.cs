using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadjustQualityMetrics : MonoBehaviour, IMenueComponentListener {
    public List<GameObject> other;
    public bool readjust;
    public GenericMenueComponent listener;
    public float delta;
    public Transform rightController;

    private SteamVR_TrackedObject _trackedObj;
    private SteamVR_Controller.Device _device;
    private QualityMetricViewPort _qualityMetricsValues;
    private ViewPortOptimizer _viewport;
    private InstantRotationOfGraph _rotate;
    private float edgeCrossingWeight, nodeOverlapWeight, edgeLenWeight, angResWeight, edgeCrossResWeight, min, max, sum, sumNoDelta, edgeCrossNoDelta, nodeOverlapNoDelta, edgeLenNoDelta, angResNoDelta, edgeCrossAngNoDelta;
    private RaycastHit _hit;
    private Ray ray;
    private Camera mainCamera;
    private Vector3 smallSize, bigSize, vrSize, backPos, frontPos;
    // Use this for initialization
    void Start ()
    {
        _trackedObj = rightController.GetComponent<SteamVR_TrackedObject>();
        _device = SteamVR_Controller.Input((int)_trackedObj.index);
        _viewport = (ViewPortOptimizer)FindObjectOfType(typeof(ViewPortOptimizer));
        _rotate = (InstantRotationOfGraph)FindObjectOfType(typeof(InstantRotationOfGraph));
        other = new List<GameObject>();
        _qualityMetricsValues = GetComponent<QualityMetricViewPort>();
        foreach (Transform child in transform.parent)
        {
            if (child == transform) continue;
            other.Add(child.gameObject);
        }
        min = -2;
        max = 2;
        smallSize = new Vector3(0.08f, 0.08f, 0.08f);
        bigSize = new Vector3(0.16f, 0.16f, 0.16f);
        vrSize = new Vector3(0.64f, 0.64f, 0.64f);
        backPos = transform.localPosition;
        frontPos = backPos + new Vector3(0, 0, -50);
    }
	
	// Update is called once per frame
	void Update () {
        mainCamera = Camera.main;
        if(mainCamera.name == "Camera")
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out _hit, 100))
            {
                if(_hit.collider.gameObject == transform.GetChild(0).gameObject && transform.localScale != bigSize)
                {
                    transform.localScale = bigSize;
                    transform.localPosition = frontPos;
                }
                else if(_hit.collider.gameObject != transform.GetChild(0).gameObject && transform.localScale == bigSize)
                {
                    transform.localScale = smallSize;
                    transform.localPosition = backPos;
                }
            }
            else
            {
                if(transform.localScale == bigSize)
                {
                    transform.localScale = smallSize;
                    transform.localPosition = backPos;
                }
            }
        }
        else if(mainCamera.name == "Camera (eye)")
        {
            ray = new Ray(rightController.position, rightController.forward);
            if (Physics.Raycast(ray, out _hit, 100))
            {
                if (_hit.collider.gameObject == transform.GetChild(0).gameObject && _device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
                {
                    _rotate.GraphRotation(GetComponent<QualityMetricViewPort>().cameraPosition);
                }
            }
        }
    }

    void ReadjustMetrics()
    {
        QualityMetricViewPort temp = new QualityMetricViewPort();
        edgeCrossingWeight = 0;
        nodeOverlapWeight = 0;
        edgeLenWeight = 0;
        angResWeight = 0;
        edgeCrossResWeight = 0;
        //calculate summed up differences
        foreach(var obj in other)
        {
            temp = obj.GetComponent<QualityMetricViewPort>();
            edgeCrossingWeight += (_qualityMetricsValues.normalizedEdgeCrossings - temp.normalizedEdgeCrossings) * delta;
            edgeCrossNoDelta += (_qualityMetricsValues.normalizedEdgeCrossings - temp.normalizedEdgeCrossings);

            nodeOverlapWeight += (_qualityMetricsValues.normalizedNodeOverlaps - temp.normalizedNodeOverlaps) * delta;
            nodeOverlapNoDelta += (_qualityMetricsValues.normalizedNodeOverlaps - temp.normalizedNodeOverlaps);

            edgeLenWeight += (_qualityMetricsValues.normalizedEdgeLength - temp.normalizedEdgeLength) * delta;
            edgeLenNoDelta += (_qualityMetricsValues.normalizedEdgeLength - temp.normalizedEdgeLength);

            angResWeight += (_qualityMetricsValues.angResRM - temp.angResRM) * delta;
            angResNoDelta += (_qualityMetricsValues.angResRM - temp.angResRM);

            edgeCrossResWeight += (_qualityMetricsValues.edgeCrossAngle - temp.edgeCrossAngle) * delta;
            edgeCrossAngNoDelta += (_qualityMetricsValues.edgeCrossAngle - temp.edgeCrossAngle);
        }

        //normalize on scale between 0-2
        edgeCrossingWeight = Normalize(edgeCrossingWeight);
        edgeCrossNoDelta = Normalize(edgeCrossNoDelta);

        nodeOverlapWeight = Normalize(nodeOverlapWeight);
        nodeOverlapNoDelta = Normalize(nodeOverlapNoDelta);

        edgeLenWeight = Normalize(edgeLenWeight);
        edgeLenNoDelta = Normalize(edgeLenNoDelta);

        angResWeight = Normalize(angResWeight);
        angResNoDelta = Normalize(angResNoDelta);

        edgeCrossResWeight = Normalize(edgeCrossResWeight);
        edgeCrossAngNoDelta = Normalize(edgeCrossAngNoDelta);

        //multiply to current values of quality metric weights
        edgeCrossingWeight = edgeCrossingWeight * _viewport.edge.qualityFactor;
        edgeCrossNoDelta = edgeCrossNoDelta * _viewport.edge.qualityFactor;

        nodeOverlapWeight = nodeOverlapWeight * _viewport.node.qualityFactor;
        nodeOverlapNoDelta = nodeOverlapNoDelta * _viewport.node.qualityFactor;

        edgeLenWeight = edgeLenWeight * _viewport.edgeLength.qualityFactor;
        edgeLenNoDelta = edgeLenNoDelta * _viewport.edgeLength.qualityFactor;

        angResWeight = angResWeight * _viewport.angRes.qualityFactor;
        angResNoDelta = angResNoDelta * _viewport.angRes.qualityFactor;

        edgeCrossResWeight = edgeCrossResWeight * _viewport.edgeCrossRes.qualityFactor;
        edgeCrossAngNoDelta = edgeCrossAngNoDelta * _viewport.edgeCrossRes.qualityFactor;

        //normalize new values so their sum is 5
        sum = edgeCrossingWeight + nodeOverlapWeight + edgeLenWeight + angResWeight + edgeCrossResWeight;
        sumNoDelta = edgeCrossNoDelta + nodeOverlapNoDelta + edgeLenNoDelta + angResNoDelta + edgeCrossAngNoDelta;

        sum = 5 / sum;
        sumNoDelta = 5 / sumNoDelta;

        edgeCrossingWeight *= sum;
        edgeCrossNoDelta *= sum;

        nodeOverlapWeight *= sum;
        nodeOverlapNoDelta *= sum;

        edgeLenWeight *= sum;
        edgeLenNoDelta *= sum;

        angResWeight *= sum;
        angResNoDelta *= sum;

        edgeCrossResWeight *= sum;
        edgeCrossAngNoDelta *= sum;
        //update current values
        _viewport.edge.qualityFactor = edgeCrossingWeight;
        _viewport.node.qualityFactor = nodeOverlapWeight;
        _viewport.edgeLength.qualityFactor = edgeLenWeight;
        _viewport.angRes.qualityFactor = angResWeight;
        _viewport.edgeCrossRes.qualityFactor = edgeCrossResWeight;

        //update sliders
        _viewport.edge.MoveSlideFromValue(edgeCrossingWeight);
        _viewport.node.MoveSlideFromValue(nodeOverlapWeight);
        _viewport.edgeLength.MoveSlideFromValue(edgeLenWeight);
        _viewport.angRes.MoveSlideFromValue(angResWeight);
        _viewport.edgeCrossRes.MoveSlideFromValue(edgeCrossResWeight);

        //Rescan tree again
        _viewport.LocalScan();
    }

    float Normalize(float value)
    {
        return ((value - min) / (max - min)) * 2;
    }

    public void menueChanged(GenericMenueComponent changedComponent)
    {
        ReadjustMetrics();
    }

    public void CloseAllMenus()
    {

    }
    void OnEnable()
    {
        if(!listener.getListeners().Contains(this)) listener.addListener(this);
    }
}
