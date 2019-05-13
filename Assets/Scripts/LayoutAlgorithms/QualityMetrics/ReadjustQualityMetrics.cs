using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadjustQualityMetrics : MonoBehaviour, IMenueComponentListener {
    public List<GameObject> other;
    public bool readjust;
    public GenericMenueComponent listener;
    public float delta;
    public Transform rightController;

    private QualityMetricViewPort qualityMetricsValues;
    private ViewPortOptimizer viewport;
    private float edgeCrossingWeight, nodeOverlapWeight, edgeLenWeight, angResWeight, edgeCrossResWeight, min, max, sum, sumNoDelta, edgeCrossNoDelta, nodeOverlapNoDelta, edgeLenNoDelta, angResNoDelta, edgeCrossAngNoDelta;
    private RaycastHit _hit;
    private Ray ray;
    private Camera mainCamera;
    private Vector3 smallSize, bigSize, vrSize, backPos, frontPos;
    // Use this for initialization
    void Start () {
        viewport = (ViewPortOptimizer)FindObjectOfType(typeof(ViewPortOptimizer));
        other = new List<GameObject>();
        qualityMetricsValues = GetComponent<QualityMetricViewPort>();
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
                if (_hit.collider.gameObject == transform.GetChild(0).gameObject && transform.localScale != vrSize)
                {
                    transform.localScale = vrSize;
                    transform.localPosition = frontPos;
                }
                else if (_hit.collider.gameObject != transform.GetChild(0).gameObject && transform.localScale == vrSize)
                {
                    transform.localScale = smallSize;
                    transform.localPosition = backPos;
                }
            }
            else
            {
                if (transform.localScale == vrSize)
                {
                    transform.localScale = smallSize;
                    transform.localPosition = backPos;
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
            edgeCrossingWeight += (qualityMetricsValues.normalizedEdgeCrossings - temp.normalizedEdgeCrossings) * delta;
            edgeCrossNoDelta += (qualityMetricsValues.normalizedEdgeCrossings - temp.normalizedEdgeCrossings);

            nodeOverlapWeight += (qualityMetricsValues.normalizedNodeOverlaps - temp.normalizedNodeOverlaps) * delta;
            nodeOverlapNoDelta += (qualityMetricsValues.normalizedNodeOverlaps - temp.normalizedNodeOverlaps);

            edgeLenWeight += (qualityMetricsValues.normalizedEdgeLength - temp.normalizedEdgeLength) * delta;
            edgeLenNoDelta += (qualityMetricsValues.normalizedEdgeLength - temp.normalizedEdgeLength);

            angResWeight += (qualityMetricsValues.angResRM - temp.angResRM) * delta;
            angResNoDelta += (qualityMetricsValues.angResRM - temp.angResRM);

            edgeCrossResWeight += (qualityMetricsValues.edgeCrossAngle - temp.edgeCrossAngle) * delta;
            edgeCrossAngNoDelta += (qualityMetricsValues.edgeCrossAngle - temp.edgeCrossAngle);
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
        edgeCrossingWeight = edgeCrossingWeight * viewport.edge.qualityFactor;
        edgeCrossNoDelta = edgeCrossNoDelta * viewport.edge.qualityFactor;

        nodeOverlapWeight = nodeOverlapWeight * viewport.node.qualityFactor;
        nodeOverlapNoDelta = nodeOverlapNoDelta * viewport.node.qualityFactor;

        edgeLenWeight = edgeLenWeight * viewport.edgeLength.qualityFactor;
        edgeLenNoDelta = edgeLenNoDelta * viewport.edgeLength.qualityFactor;

        angResWeight = angResWeight * viewport.angRes.qualityFactor;
        angResNoDelta = angResNoDelta * viewport.angRes.qualityFactor;

        edgeCrossResWeight = edgeCrossResWeight * viewport.edgeCrossRes.qualityFactor;
        edgeCrossAngNoDelta = edgeCrossAngNoDelta * viewport.edgeCrossRes.qualityFactor;

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
        viewport.edge.qualityFactor = edgeCrossingWeight;
        viewport.node.qualityFactor = nodeOverlapWeight;
        viewport.edgeLength.qualityFactor = edgeLenWeight;
        viewport.angRes.qualityFactor = angResWeight;
        viewport.edgeCrossRes.qualityFactor = edgeCrossResWeight;

        //update sliders
        viewport.edge.MoveSlideFromValue(edgeCrossingWeight);
        viewport.node.MoveSlideFromValue(nodeOverlapWeight);
        viewport.edgeLength.MoveSlideFromValue(edgeLenWeight);
        viewport.angRes.MoveSlideFromValue(angResWeight);
        viewport.edgeCrossRes.MoveSlideFromValue(edgeCrossResWeight);

        //Rescan tree again
        viewport.LocalScan();
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
