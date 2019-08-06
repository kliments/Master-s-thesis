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
    public float initialNodeOverlaps, initialEdgeCrossings, initialEdgeLength, initialEdgeCrossAngle, initialAngRes, initialTotalEdgeLength, initialDistinctAngles, initialTotalArea;
    public float normalizedNodeOverlaps, normalizedEdgeCrossings, normalizedEdgeLength, normalizedTotalEdgeLength, normalizedDistinctAngles, normalizedTotalArea, edgeLength, edgeCrossAngle, angRes, totalEdgeLength, distinctAngles, overallGrade, maximizeAngleRM, totalArea;
    public Vector3 cameraPosition;
    public GeneralLayoutAlgorithm algorithm;
    public Vector3[] listPos;
    public int index;
    public int clusterID;

    //distance for kMeans clustering
    public float totalDistance;

    public void AssignValues(QualityMetricViewPort values)
    {
        nrEdgeCrossings = values.nrEdgeCrossings;
        nrNodeOverlaps = values.nrNodeOverlaps;
        normalizedEdgeCrossings = values.normalizedEdgeCrossings;
        normalizedEdgeLength = values.normalizedEdgeLength;
        normalizedNodeOverlaps = values.normalizedNodeOverlaps;
        normalizedTotalEdgeLength = values.normalizedTotalEdgeLength;
        normalizedDistinctAngles = values.normalizedDistinctAngles;
        normalizedTotalArea = values.normalizedTotalArea;

        edgeLength = values.edgeLength;
        totalEdgeLength = values.totalEdgeLength;
        distinctAngles = values.distinctAngles;
        maximizeAngleRM = values.maximizeAngleRM;
        totalArea = values.totalArea;
        edgeCrossAngle = values.edgeCrossAngle;
        angRes = values.angRes;
        overallGrade = values.overallGrade;
        cameraPosition = values.cameraPosition;
        algorithm = values.algorithm;
        listPos = values.listPos;
        index = values.index;
    }
}
