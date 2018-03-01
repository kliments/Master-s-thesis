using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Assets.Scripts.Model;
using Controller.Interaction;
using Controller.Interaction.Icon;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighlightOperator : GenericIconInteractionController, IPointerEnterHandler, IPointerExitHandler
{

	public GameObject HighlightPlane;
	public static GenericOperator CurrentHover;
	public static HighlightOperator[] SpawnedIcons;
	public static int PublicCounter;
	private int _privateCounter;

	private void Awake()
	{
		PublicCounter++;
		_privateCounter = PublicCounter;
		Debug.Log(PublicCounter);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		HighlightPlane.SetActive(true);
		CurrentHover = GetOperator();
		SpawnedIcons = FindObjectsOfType<HighlightOperator>();

//		Debug.Log("ID " + SpawnHandler.Operators[0].Id);
//		Debug.Log("ID " + SpawnHandler.Operators[1].Id);

		var operators = Observer._operators;
		foreach (var genericOperator in operators)
		{
			if (CurrentHover.Parents.Contains(genericOperator))
			{
				var id = genericOperator.Id;
				id = id - 1;
				SpawnedIcons[id].HighlightPlane.SetActive(true);
				Debug.Log("Parent: " + SpawnedIcons[id].name + " mit ID " + genericOperator.Id);
			}
			
			
			
			
//			Debug.Log(genericOperator.Id);
//			Debug.Log(gameObject.GetComponent<GenericOperator>().Parents);
//			Debug.Log(CurrentHover);
			
			
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
		
		var operators = Observer._operators;
		foreach (var genericOperator in operators)
		{
			var id = genericOperator.Id;
			SpawnedIcons[id-1].HighlightPlane.SetActive(false);
		}
	}
}
