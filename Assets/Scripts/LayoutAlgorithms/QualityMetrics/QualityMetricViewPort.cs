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
    public float edgeLength, edgeCrossAngle, angResRM, overallGrade;
    public float _ZnodeOverlap, _ZedgeCross, _ZminAngle, _ZangRes, _ZedgeLength;
    public Vector3 cameraPosition;
    public GeneralLayoutAlgorithm algorithm;
}
