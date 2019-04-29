using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatePreviewButtons : MonoBehaviour {
    public Transform child;
    public Texture2D[] textures;
    public QualityMetricViewPort[] views;
	// Use this for initialization
	void Start () {
        textures = new Texture2D[3];
        views = new QualityMetricViewPort[3];
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CallForCreatingButtons(List<string> list)
    {
        StartCoroutine(CreateButtons(list));
    }

    IEnumerator CreateButtons(List<string> list)
    {
        yield return 0;

        int i = 0;
        foreach (Transform button in child)
        {
            //Texture2D texture = Resources.Load<Texture2D>(list[i]);
            button.GetComponent<RawImage>().texture = textures[i];
            i++;
        }
    }
}
