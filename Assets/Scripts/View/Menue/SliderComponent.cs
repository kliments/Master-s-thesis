using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderComponent : Targetable
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }
    private void OnEnable()
    {
        InputController.LeftClickOnTargetEvent += OnLeftClickOnTargetEvent;
        InputController.LeftClickReleaseEvent += OnLeftClickReleaseEvent;
    }

    private void OnDisable()
    {
        InputController.LeftClickOnTargetEvent -= OnLeftClickOnTargetEvent;
        InputController.LeftClickReleaseEvent -= OnLeftClickReleaseEvent;
    }
    protected override void OnLeftClickOnTargetEventAction()
    {
        GetComponent<QualityMetricSlider>().slide = true;
    }

    protected override void OnLeftClickReleaseEventAction()
    {
        GetComponent<QualityMetricSlider>().slide = false;
    }
}
