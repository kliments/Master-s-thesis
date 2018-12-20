using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
 * Counts the number of node overlaps by
 * raycasting from the camera to the 4 vertices of a node
 * starting from the furthest one
 */
public class NodeOverlapping : MonoBehaviour {

    public bool calculateNodeOverlapping;
    public float count;
    public List<GenericOperator> sortedList;

    private int layerMask;
    private Vector3 dir, center;
    private Observer observer;
    private GameObject camera, currentIcon;
    private RaycastHit[] hits, hitsCenter;
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

    public float CalculateNodeOverlapping()
    {
        SortListOfNodes();
        /*
         * List that adds every time an object covers a vertex
         */
        List<GameObject> allHitObjects = new List<GameObject>();

        /*
         * list that contains only one time the objects that are hit (cover a vertex),
         * for later comparison with above list, and see how many vertices
         * does the current object cover
         */
        List<GameObject> justHitObjects = new List<GameObject>();

        /*
         * List that contains all the objects that are hit when raycasting in the center
         * If an object is hit, means that covers more than 50% of the current icon
         * which increases the coefficient of the current hit object towards the current icon
         */
        List<GameObject> centerHitObjects = new List<GameObject>();

        count = 0;
        foreach(var op in sortedList)
        {
            float coef = 0;
            if (op.GetType() == typeof(NewOperator)) continue;
            foreach (Transform child in op.GetIcon().transform)
            {
                if (child.gameObject.activeSelf)
                {
                    currentIcon = child.gameObject;
                    break;
                }
            }
            allHitObjects = new List<GameObject>();
            justHitObjects = new List<GameObject>();
            centerHitObjects = new List<GameObject>();
            center = new Vector3();
            //raycast to each corner of the icon and detect other colliders if hit
            for(int i=0; i<4; i++)
            {
                dir = currentIcon.transform.TransformPoint(currentIcon.GetComponent<MeshFilter>().mesh.vertices[i]) - camera.transform.position;
                center += currentIcon.transform.TransformPoint(currentIcon.GetComponent<MeshFilter>().mesh.vertices[i]);
                hits = Physics.RaycastAll(camera.transform.position, dir, 100, layerMask);
                foreach(var hit in hits)
                {
                    if (hit.transform.tag != "NodeIcon") continue;
                    if (hit.transform.parent.GetComponent<GenericIcon>().Op.GetType() == typeof(NewOperator)) continue;
                    if (hit.transform.gameObject != currentIcon)
                    {
                        if (!justHitObjects.Contains(hit.transform.gameObject)) justHitObjects.Add(hit.transform.gameObject);
                        allHitObjects.Add(hit.transform.gameObject);
                    }
                }
            }

            //raycast in the center of the icon, for coefficient calculation
            center /= 4;
            dir = center - camera.transform.position;
            hitsCenter = Physics.RaycastAll(camera.transform.position, dir, 100, layerMask);
            foreach (var hit in hitsCenter)
            {
                if (hit.transform.tag != "NodeIcon") continue;
                if (hit.transform.parent.GetComponent<GenericIcon>().Op.GetType() == typeof(NewOperator)) continue;
                if (hit.transform.gameObject != currentIcon)
                {
                    centerHitObjects.Add(hit.transform.gameObject);
                }
            }

            foreach(var justHitObj in justHitObjects)
            {
                bool centerIsHit = false;
                int verticesCovered = 0;
                foreach(var allHitObj in allHitObjects)
                {
                    if (allHitObj == justHitObj) verticesCovered++;
                }
                if (verticesCovered == 4) coef += 1;
                else if (verticesCovered == 2)
                {
                    foreach (var centerHit in centerHitObjects)
                    {
                        if (centerHit == justHitObj)
                        {
                            coef += 0.75f;
                            centerIsHit = true;
                        }
                    }
                    if (!centerIsHit) coef += 0.5f;
                }
                else if (verticesCovered == 1)
                {
                    foreach (var centerHit in centerHitObjects)
                    {
                        if (centerHit == justHitObj)
                        {
                            coef += 0.25f;
                            centerIsHit = true;
                        }
                    }
                    if (!centerIsHit) coef += 0.125f;
                }
            }

            count += coef;

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
        int sum = 0;
        for(int i = 1; i <= observer.GetOperators().Count; i++)
        {
            sum += i;
        }

        return count/sum;
    }

    //Sorting nodes by distance, starting with the furthest one
    void SortListOfNodes()
    {
        sortedList = observer.GetOperators().OrderBy(node => -Vector3.Distance(camera.transform.position, node.GetIcon().transform.position)).ToList();
    }
}
