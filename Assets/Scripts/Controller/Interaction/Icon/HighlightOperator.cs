using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighlightOperator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

	public GameObject HighlightPlane;
	

	public void OnPointerEnter(PointerEventData eventData)
	{
		HighlightPlane.SetActive(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		HighlightPlane.SetActive(false);
	}
}
