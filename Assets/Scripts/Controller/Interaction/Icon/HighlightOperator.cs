using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Assets.Scripts.Model;
using Controller.Interaction.Icon;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighlightOperator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

	public GameObject HighlightPlane;
	
	private GenericOperator CurrentHover;

	

	public void OnPointerEnter(PointerEventData eventData)
	{
		CurrentHover = gameObject.GetComponent<GenericOperator>();
		
		HighlightPlane.SetActive(true);

//		Debug.Log("ID " + SpawnHandler.Operators[0].Id);
//		Debug.Log("ID " + SpawnHandler.Operators[1].Id);

		var operators = Observer._operators;
		foreach (var genericOperator in operators)
		{
//			Debug.Log(genericOperator.Id);
			Debug.Log(gameObject.GetComponent<GenericOperator>().Parents[0]);
			
			if (CurrentHover.Parents.Contains(genericOperator))
			{
				Debug.Log(genericOperator.Id);
			}
		}
		
		
//		Debug.Log("Parents 0 " + SpawnHandler.Operators[1].Parents.ToString());

//		var clickedOP = NewIconInteractionController.ClickedOp;

//		var currentParent = GetOperator().Parents[0];
//		Debug.Log(currentParent);

//		for (var i = 0; i < Observer.transform.GetChildCount(); i++)
//		{
//			var tempChild = Observer.transform.GetChild(i);
//			var tempChildParent = tempChild.GetComponent<GenericOperator>().Parents[0];
//			
//		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		HighlightPlane.SetActive(false);
	}
}
