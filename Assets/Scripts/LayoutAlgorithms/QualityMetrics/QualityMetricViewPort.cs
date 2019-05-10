using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class that contains quality metrics for current viewport
 * It contains number of node overlaps, number of edge crossings,
 * angles between them and its camera position
 */
public class QualityMetricViewPort : MonoBehaviour {
    public float nrNodeOverlaps, nrEdgeCrossings;
    public float normalizedNodeOverlaps, normalizedEdgeCrossings, normalizedEdgeLength, edgeLength, edgeCrossAngle, angResRM, overallGrade;
    public Vector3 cameraPosition;
    public GeneralLayoutAlgorithm algorithm;
    public Vector3[] listPos;
    public int index;
    public int clusterID;

    public void AssignValues(QualityMetricViewPort values)
    {
        nrEdgeCrossings = values.nrEdgeCrossings;
        nrNodeOverlaps = values.nrNodeOverlaps;
        normalizedEdgeCrossings = values.normalizedEdgeCrossings;
        normalizedEdgeLength = values.normalizedEdgeLength;
        normalizedNodeOverlaps = values.normalizedNodeOverlaps;
        edgeLength = values.edgeLength;
        edgeCrossAngle = values.edgeCrossAngle;
        angResRM = values.angResRM;
        overallGrade = values.overallGrade;
        cameraPosition = values.cameraPosition;
        algorithm = values.algorithm;
        listPos = values.listPos;
        index = values.index;
    }
}
