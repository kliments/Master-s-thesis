using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;

public class GraphSpaceController : MonoBehaviour {
    private Vector3 _scale = new Vector3(0.03f, 0.06f, 0);

    private Observer observer;

    private List<GameObject> graphEdges;
    private GameObject Container;


    private void Awake()
    {
        graphEdges = new List<GameObject>();
        Container = new GameObject("GraphEdges");
    }

    public void InstallNewIcon(GenericOperator op)
    {
        op.GetIcon().gameObject.transform.parent = GameObject.Find("ControlWall").transform;
        op.GetIcon().gameObject.transform.localPosition = new Vector3(-0.4f, 0, -0.6f);
        op.GetIcon().gameObject.transform.localScale = _scale;
    }

    public void moveToSpawnPosition(GenericOperator op)
    {
        if (op.GetType().Equals((typeof(NewOperator))) && op.Parents != null)
        {
            op.GetIcon().transform.position = op.Parents[0].GetIcon().transform.position + new Vector3(1, 0, 0);
        }

        if (op.Parents != null && op.Parents[0].Children != null)
        {
            op.GetIcon().transform.position += new Vector3(0, (op.Parents[0].Children.Count - 1) * 0.3f, 0);
        }
    }

    private void Update()
    {
        drawGraphConnections();
    }

    public void drawGraphConnections()
    {
        foreach (GameObject edge in graphEdges)
        {
            Destroy(edge);
        }


        foreach (GenericOperator go in observer.GetOperators())
        {
            drawConnectionToChildren(go);
        }
    }

    private void drawConnectionToChildren(GenericOperator go)
    {
        if (go.Children == null || go.Children.Count == 0) return; 
        foreach (GenericOperator child in go.Children)
        {
            if (go.GetIcon() == null || child == null || child.GetIcon() == null) continue;

            GameObject line = new GameObject();
            LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
            lineRenderer.SetWidth(0.01f, 0.01f);
           
            lineRenderer.SetPositions(new Vector3[] {
                go.GetIcon().transform.position+new Vector3(0,0,0.001f), child.GetIcon().transform.position+new Vector3(0,0,0.001f)
            });
            line.transform.parent = Container.transform;
            lineRenderer.transform.parent = Container.transform;
            graphEdges.Add(line);
        }

        
    }

    public void setObserver(Observer observer)
    {
        this.observer = observer;
    }
}
