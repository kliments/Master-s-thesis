using System.Collections;
using System.Collections.Generic;
using Controller.Interaction.Icon;
using UnityEngine;

public class Spawning : NewIconInteractionController {

	public void SpawnDataLoader()
	{
		op.Observer.CreateOperator(0);
		GetOperator().Observer.CreateOperator(1);
		
	}

	public void SpawnScatterPlot()
	{
		GetOperator().Observer.CreateOperator(2);
		GetOperator().Observer.CreateOperator(1);
	}
}
