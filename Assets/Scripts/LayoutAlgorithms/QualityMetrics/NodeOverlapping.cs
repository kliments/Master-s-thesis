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
    public float count, totalArea;
    public List<GenericOperator> sortedList;
    private int layerMask;
    private Vector3 dir, center, currentPoint, startXY, endX, endY, tempX, tempY;
    private Observer observer;
    private GameObject currentIcon;
    private RaycastHit hit;
	// Use this for initialization
	void Start () {
        observer = (Observer)FindObjectOfType(typeof(Observer));
        layerMask = LayerMask.GetMask("NodeOverlapping");
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

        float coef = 0;
        //Distance for shifting point in X and 
        float dist = 0;
        foreach(var op in sortedList)
        {
            count = 0;
            if (op.GetType() == typeof(NewOperator)) continue;
            foreach (Transform child in op.GetIcon().transform)
            {
                if (child.gameObject.activeSelf)
                {
                    currentIcon = child.gameObject;
                    break;
                }
            }
            if(dist == 0)
            {
                dist = Vector3.Distance(currentIcon.transform.TransformPoint(currentIcon.GetComponent<MeshFilter>().mesh.vertices[0]), currentIcon.transform.TransformPoint(currentIcon.GetComponent<MeshFilter>().mesh.vertices[1]));
                dist /= 5;
            }
            /*
             * Start and end points for interpolation between them to get the points to raycast
             */
            startXY = currentIcon.transform.TransformPoint(currentIcon.GetComponent<MeshFilter>().mesh.vertices[0]);
            endX = currentIcon.transform.TransformPoint(currentIcon.GetComponent<MeshFilter>().mesh.vertices[1]);
            endY = currentIcon.transform.TransformPoint(currentIcon.GetComponent<MeshFilter>().mesh.vertices[2]);
            for (int i=0; i<=4; i++)
            {
                tempX = Vector3.Lerp(startXY, endY, (float)i/4);
                tempY = Vector3.Lerp(endX, currentIcon.transform.TransformPoint(currentIcon.GetComponent<MeshFilter>().mesh.vertices[3]), (float)i / 4);
                for(int j=0; j<=4;j++)
                {
                    currentPoint = Vector3.Lerp(tempX, tempY, (float)j / 4);
                    dir = currentPoint - Camera.main.transform.position;
                    if(Physics.Raycast(Camera.main.transform.position, dir, out hit, 100, layerMask))
                    {
                        if(hit.transform.gameObject != currentIcon)
                        {
                            count++;
                        }
                    }
                }
            }
            count /= 25;
            coef += count;
            //change layer so it will be ignored in the future raycasts / NodeOverlapChecked
            currentIcon.layer = 11;
        }

        //reset back the layers to default / NodeOverlapping
        foreach(var op in observer.GetOperators())
        {
            if (op.GetType() == typeof(NewOperator)) continue;
            foreach (Transform child in op.GetIcon().transform)
            {
                if (child.gameObject.activeSelf)
                {
                    currentIcon = child.gameObject;
                    currentIcon.layer = 10;
                    break;
                }
            }
        }
        int sum = 0;
        for(int i = 0; i < observer.GetOperators().Count; i++)
        {
            if (observer.GetOperators()[i].GetType().Equals(typeof(NewOperator))) continue;
            sum++;
        }
        totalArea = TotalAreaOfTree(sortedList);
        return coef / sum;
    }

    //Sorting nodes by distance, starting with the furthest one
    void SortListOfNodes()
    {
        sortedList = observer.GetOperators().OrderBy(node => -Vector3.Distance(Camera.main.transform.position, node.GetIcon().transform.position)).ToList();
    }

    float TotalAreaOfTree(List<GenericOperator> operators)
    {
        float mostLeft, mostRight, mostDown, mostUp;
        Vector3 iconPos = new Vector3();
        mostLeft = Camera.main.pixelWidth;
        mostRight = 0;
        mostDown = Camera.main.pixelHeight;
        mostUp = 0;

        foreach(var op in operators)
        {
            iconPos = Camera.main.WorldToScreenPoint(op.GetIcon().transform.position);
            if (iconPos.x < mostLeft) mostLeft = iconPos.x;
            if (iconPos.x > mostRight) mostRight = iconPos.x;
            if (iconPos.y < mostDown) mostDown = iconPos.y;
            if (iconPos.y > mostUp) mostUp = iconPos.y;
        }

        return (mostRight - mostLeft) * (mostUp - mostDown);
    }
}
