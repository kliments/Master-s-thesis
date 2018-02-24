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
//	public GameObject Observer;
	

	public void OnPointerEnter(PointerEventData eventData)
	{
		HighlightPlane.SetActive(true);

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
