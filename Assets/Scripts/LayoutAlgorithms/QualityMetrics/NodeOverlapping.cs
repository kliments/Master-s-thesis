using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NodeOverlapping : MonoBehaviour {

    public bool calculateNodeOverlapping;
    public int count;
    public List<GenericOperator> sortedList;

    private int layerMask;
    private Vector3 dir;
    private Observer observer;
    private GameObject camera, currentIcon;
    private RaycastHit[] hits;
	// Use this for initialization
	void Start () {
        camera = Camera.main.gameObject;
        observer = (Observer)FindObjectOfType(typeof(Observer));
        layerMask = ~(1 << 10);
        sortedList = new List<GenericOperator>();
	}
	
	// Update is called once per frame
	void Update () {
        if(calculateNodeOverlapping)
        {
            calculateNodeOverlapping = false;
            Debug.Log(CalculateNodeOverlapping());
        }
	}

    int CalculateNodeOverlapping()
    {
        SortListOfNodes();
        List<GameObject> hitObjects = new List<GameObject>();
        count = 0;
        foreach(var op in sortedList)
        {
            if (op.GetType() == typeof(NewOperator)) continue;
            foreach (Transform child in op.GetIcon().transform)
            {
                if (child.gameObject.activeSelf)
                {
                    currentIcon = child.gameObject;
                    break;
                }
            }
            hitObjects = new List<GameObject>();
            //raycast to each corner of the icon and detect other colliders if hit
            for(int i=0; i<4; i++)
            {
                dir = currentIcon.transform.TransformPoint(currentIcon.GetComponent<MeshFilter>().mesh.vertices[i]) - camera.transform.position;
                hits = Physics.RaycastAll(camera.transform.position, dir, 100, layerMask);
                foreach(var hit in hits)
                {
                    if (hit.transform.tag != "NodeIcon") continue;
                    if (hit.transform.parent.GetComponent<GenericIcon>().Op.GetType() == typeof(NewOperator)) continue;
                    if (hit.transform.gameObject != currentIcon && !hitObjects.Contains(hit.transform.gameObject)) hitObjects.Add(hit.transform.gameObject);
                }
            }
            count += hitObjects.Count;

            //change layer so it will be ignored in the future raycasts
            currentIcon.layer = 10;
        }

        //reset back the layers to default
        foreach(var op in observer.GetOperators())
        {
            if (op.GetType() == typeof(NewOperator)) continue;
            foreach (Transform child in op.GetIcon().transform)
            {
                if (child.gameObject.activeSelf)
                {
                    currentIcon = child.gameObject;
                    currentIcon.layer = 0;
                    break;
                }
            }
        }

        return count;
    }

    //
    void SortListOfNodes()
    {
        sortedList = observer.GetOperators().OrderBy(node => -Vector3.Distance(camera.transform.position, node.GetIcon().transform.position)).ToList();
    }
}
