﻿using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using Controller.Interaction.Icon;
using UnityEngine;

public class SpawnHandler : NewIconInteractionController {

	public void SpawnDataLoader()
	{
		var Parents0 = new List<GenericOperator>();
		var Parents1 = new List<GenericOperator>();
		
		Parents0.Add(ClickedOp.transform.GetComponent<GenericOperator>());
		Parents1.Add(ClickedOp.transform.GetComponent<GenericOperator>());
		
		ClickedOp.Observer.CreateOperator(0, Parents0);
		ClickedOp.Observer.CreateOperator(1, Parents1);
	}

	public void SpawnScatterPlot()
	{
		var Parents0 = new List<GenericOperator>();
		var Parents1 = new List<GenericOperator>();
		
		Parents0.Add(ClickedOp.transform.GetComponent<GenericOperator>());
		Parents1.Add(ClickedOp.transform.GetComponent<GenericOperator>());
		
		ClickedOp.Observer.CreateOperator(2, Parents0);
		ClickedOp.Observer.CreateOperator(1, Parents1);
	}
}
