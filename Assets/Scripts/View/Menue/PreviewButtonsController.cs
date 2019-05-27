using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewButtonsController : MonoBehaviour {
    public Transform child;
    public Texture2D[] textures;
    public QualityMetricViewPort[] views;
    public int[] order;
    public int trialNr;
	// Use this for initialization
	void Start () {
        textures = new Texture2D[3];
        views = new QualityMetricViewPort[3];
        order = new int[3] { 0, 1, 2 };
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void CallForCreatingButtons(List<string> list, float delta)
    {
        StartCoroutine(CreateButtons(list, delta));
    }

    IEnumerator CreateButtons(List<string> list, float delta)
    {
        yield return 0;

        int i = 0;
        foreach (Transform button in child)
        {
            button.GetComponent<RawImage>().texture = textures[i];
            button.GetComponent<QualityMetricViewPort>().AssignValues(views[i]);
            button.GetComponent<ReadjustQualityMetrics>().delta = delta;
            i++;
        }
    }

    void ShuffleOrder()
    {
        System.Random rand = new System.Random();

        // For each spot in the array, pick
        // a random item to swap into that spot.
        for (int i = 0; i < order.Length - 1; i++)
        {
            int j = rand.Next(i, order.Length);
            int temp = order[i];
            order[i] = order[j];
            order[j] = temp;
        }
    }
}
