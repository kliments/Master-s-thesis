using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class that contains quality metrics for current viewport
 * It contains number of node overlaps, number of edge crossings,
 * angles between them and its camera position
 */
public class QualityMetricViewPort {
    public float nrNodeOverlaps, nrEdgeCrossings;
    public float normalizedNodeOverlaps, normalizedEdgeCrossings, normalizedEdgeLength, edgeLength, edgeCrossAngle, angResRM, overallGrade;
    public Vector3 cameraPosition;
    public GeneralLayoutAlgorithm algorithm;
    public Vector3[] listPos;
    public int index;
}
