using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using Model.Operators;
using UnityEngine;

public class GraphSpaceController : MonoBehaviour {
    private Vector3 _scale = new Vector3(0.3f, 0.3f, 0.3f);

    private Observer observer;

    public List<LineRenderer> graphEdges;
    private GameObject Container;

    private float counter = 1;

    private GeneralLayoutAlgorithm currentAlgorithm;

    private void Awake()
    {
        graphEdges = new List<LineRenderer>();
        Container = new GameObject("GraphEdges");
    }

    public void InstallNewIcon(GenericOperator op)
    {
        currentAlgorithm = ((LayoutAlgorithm)FindObjectOfType(typeof(LayoutAlgorithm))).currentLayout;
        op.GetIcon().gameObject.transform.parent = GameObject.Find("ControlWall").transform;
        //shift icon one position more to the right than the previous operator
        if (op.Parents != null && op.Parents.Count > 0)
        {
            if (currentAlgorithm.GetType().Equals(typeof(DefaultAlgorithm)) || (currentAlgorithm.GetType().Equals(typeof(ForceDirectedAlgorithm))&& currentAlgorithm.GetTemporal()))
            {
                op.GetIcon().gameObject.transform.localPosition = new Vector3(counter, op.Parents[0].GetIcon().transform.localPosition.y, 0);
            }
            else
            {
                Vector3 temp = op.Parents[0].GetIcon().transform.localPosition;
                temp.x += 1;
                op.GetIcon().gameObject.transform.localPosition = temp;
            }
            if (!op.GetType().Equals((typeof(NewOperator))))
            {
                if (!op.Parents[0].GetType().Equals(typeof(SplitDatasetOperator))) counter++;
                else counter += 0.5f;
            }
        }
        else
        {
            op.GetIcon().gameObject.transform.localPosition = new Vector3(0, 0, 0);
        }
        op.GetIcon().gameObject.transform.localScale = _scale;
    }

    public void moveToSpawnPosition(GenericOperator op)
    {
        if (op.GetType().Equals((typeof(NewOperator))) && op.Parents != null)
        {
            op.GetIcon().transform.position = op.Parents[0].GetIcon().transform.position + new Vector3(1, 0, 0);
        }

        if (op.Parents != null && op.Parents.Count > 0 && op.Parents[0].Children != null)
        {
            //op.GetIcon().transform.position += new Vector3(0, (op.Parents[0].Children.Count - 1) * 0.3f, 0);
            op.GetIcon().transform.position += new Vector3(0, (op.Parents[0].Children.Count - 1) * 0.3f, 0);
        }
        op.GetIcon().GetComponent<IconProperties>().newPos = op.GetIcon().transform.position;
    }

    private void Update()
    {
        //drawGraphConnections();
    }

    /*public void drawGraphConnections()
    {
        foreach (GameObject edge in graphEdges)
        {
            Destroy(edge);
        }

        foreach (GenericOperator go in observer.GetOperators())
        {
            drawConnectionToChildren(go);
        }
    }*/

    public void DrawEdge(GenericOperator parent, GenericOperator op)
    {
        LineRenderer lr;
        if (parent == null) return;
        if (op.gameObject.GetComponent<LineRenderer>() != null) lr = op.gameObject.GetComponent<LineRenderer>();
        else lr = op.gameObject.AddComponent<LineRenderer>();
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        lr.SetPositions(new Vector3[] { parent.GetIcon().transform.position + new Vector3(0, 0, 0.001f), op.GetIcon().transform.position + new Vector3(0, 0, 0.001f) });
        if(!graphEdges.Contains(lr)) graphEdges.Add(lr);
    }

    public void DestroyEdge()
    {
        Destroy(graphEdges[graphEdges.Count - 1].gameObject);
    }

    /*private void drawConnectionToChildren(GenericOperator go)
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
    }*/

    public void setObserver(Observer observer)
    {
        this.observer = observer;
    }
}
