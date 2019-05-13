using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Calculates KMeans algorithm on each viewpoint's quality metrics
 * Clusters them into 3 clusters, out of which 3 views will be chosen
 * for showing to the user
 */
public class MultiDimensionalKMeansClustering : MonoBehaviour {

    public bool calculatee, calculate;

    private ViewPortOptimizer viewportOpt;
    private List<QualityMetricViewPort> centroids;
    private int[] oldSizeOfClusters;
    private QualityMetricViewPort[] oldCentroidValues;
	// Use this for initialization
	void Start () {
        viewportOpt = GetComponent<ViewPortOptimizer>();
	}
	
	// Update is called once per frame
	void Update () {
            
	}

    public List<QualityMetricViewPort> Clusters(List<QualityMetricViewPort> viewpointList)
    {
        List<List<QualityMetricViewPort>> clusters = new List<List<QualityMetricViewPort>>(3);
        List<QualityMetricViewPort> chosenViewpoints = new List<QualityMetricViewPort>();
        List<List<int>> clusterCounters = new List<List<int>>();
        List<int> randomIndexes = new List<int>(3);
        centroids = new List<QualityMetricViewPort>();
        int random, index, counter = 0;
        float distance;
        oldSizeOfClusters = new int[3];
        oldCentroidValues = new QualityMetricViewPort[3];

        for(int i=0; i<3; i++)
        {
            clusters.Add(new List<QualityMetricViewPort>());
            clusterCounters.Add(new List<int>());
        }
        //choose 3 random points as centroid
        for(int i=0; i<3; i++)
        {
            random = Random.Range(0, viewpointList.Count);
            while(randomIndexes.Contains(random))
            {
                random = Random.Range(0, viewpointList.Count);
            }
            randomIndexes.Add(random);
            QualityMetricViewPort temp = CopyOf(viewpointList[random]);
            centroids.Add(temp);
            oldCentroidValues[i] = new QualityMetricViewPort();
        }
        calculate = true;
        while(calculate)
        {
            calculate = false;
            //if old size is different than last size, re-run algorithm again
            for(int i=0; i<3; i++)
            {
                if ((oldSizeOfClusters[i] != clusterCounters[i].Count) || counter == 0)
                {
                    calculate = true;
                    break;
                }
            }
            //update old size of clusters to last one
            for (int i = 0; i < 3; i++)
            {
                oldSizeOfClusters[i] = clusterCounters[i].Count;
            }

            //compute distance from each point to centroid
            foreach (var viewPoint in viewpointList)
            {
                distance = Distance5D(centroids[0],viewPoint);
                index = 0;
                for(int i=0; i<3; i++)
                {
                    if(distance > Distance5D(centroids[i],viewPoint))
                    {
                        index = i;
                        distance = Distance5D(centroids[i], viewPoint);
                    }
                }
                //remove if it was in another cluster
                if (clusterCounters[viewPoint.clusterID].Contains(viewPoint.index)) clusterCounters[viewPoint.clusterID].Remove(viewPoint.index);
                viewPoint.clusterID = index;
                clusterCounters[viewPoint.clusterID].Add(viewPoint.index);
            }

            //if old position is different than new position of centroid, re-run algorithm
            for(int i=0; i<3; i++)
            {
                if((oldCentroidValues[i].angResRM != centroids[i].angResRM || oldCentroidValues[i].edgeCrossAngle != centroids[i].edgeCrossAngle ||
                   oldCentroidValues[i].normalizedEdgeCrossings != centroids[i].normalizedEdgeCrossings || oldCentroidValues[i].normalizedNodeOverlaps != centroids[i].normalizedNodeOverlaps
                   || oldCentroidValues[i].normalizedEdgeLength != centroids[i].normalizedEdgeLength) || counter == 0)
                {
                    calculate = true;
                    break;
                }
            }
            //update old centroid values to the last ones
            for(int i=0; i<3; i++)
            {
                oldCentroidValues[i] = CopyOf(centroids[i]);
            }

            //calculate new position of centroids
            for (int i=0; i<3; i++)
            {
                QualityMetricViewPort temp = new QualityMetricViewPort();
                foreach(var indexView in clusterCounters[i])
                {
                    temp.angResRM += viewpointList[indexView].angResRM;
                    temp.edgeCrossAngle += viewpointList[indexView].edgeCrossAngle;
                    temp.normalizedEdgeCrossings += viewpointList[indexView].normalizedEdgeCrossings;
                    temp.normalizedEdgeLength += viewpointList[indexView].normalizedEdgeLength;
                    temp.normalizedNodeOverlaps += viewpointList[indexView].normalizedNodeOverlaps;
                }
                
                temp.angResRM /= clusterCounters[i].Count;
                temp.edgeCrossAngle /= clusterCounters[i].Count;
                temp.normalizedEdgeCrossings /= clusterCounters[i].Count;
                temp.normalizedEdgeLength /= clusterCounters[i].Count;
                temp.normalizedNodeOverlaps /= clusterCounters[i].Count;
                centroids[i] = CopyOf(temp);
            }

            counter++;
        }

        for(int i=0; i<3; i++)
        {
            foreach(var viewIndex in clusterCounters[i])
            {
                clusters[i].Add(viewpointList[viewIndex]);
            }
        }

        //choose viewpoints that are closest to the centroids
        for(int i=0; i<3; i++)
        {
            Debug.Log("Cluster nr " + i.ToString());
            float dist = 0;
            QualityMetricViewPort temp = new QualityMetricViewPort();
            dist = Distance5D(clusters[i][0], centroids[i]);
            foreach (var point in clusters[i])
            {
                if(dist > Distance5D(point, centroids[i]))
                {
                    dist = Distance5D(point, centroids[i]);
                    temp = CopyOf(point);
                }
                Debug.Log(" overallGrade " + point.overallGrade + " EdgeCrossAngle: " + point.edgeCrossAngle + " AngResRM: " + point.angResRM
                        + " normalizedEdgeLength: " + point.normalizedEdgeLength + " normalizedNodeOverlaps: " + point.normalizedNodeOverlaps
                        + " normalizedEdgeCrossings: " + point.normalizedEdgeCrossings);
            }
            chosenViewpoints.Add(temp);
        }

        return chosenViewpoints;
    }

    float Distance5D(QualityMetricViewPort centroid, QualityMetricViewPort point)
    {
        return Mathf.Sqrt(Mathf.Pow(centroid.normalizedEdgeCrossings - point.normalizedEdgeCrossings, 2) + Mathf.Pow(centroid.normalizedNodeOverlaps - point.normalizedNodeOverlaps, 2) +
            Mathf.Pow(centroid.normalizedEdgeLength - point.normalizedEdgeLength, 2) + Mathf.Pow(centroid.angResRM - point.angResRM, 2) + Mathf.Pow(centroid.edgeCrossAngle - point.edgeCrossAngle, 2));
    }

    QualityMetricViewPort CopyOf(QualityMetricViewPort point)
    {
        QualityMetricViewPort temp = new QualityMetricViewPort();
        temp.angResRM = point.angResRM;
        temp.edgeCrossAngle = point.edgeCrossAngle;
        temp.normalizedEdgeCrossings = point.normalizedEdgeCrossings;
        temp.normalizedEdgeLength = point.normalizedEdgeLength;
        temp.normalizedNodeOverlaps = point.normalizedNodeOverlaps;
        temp.algorithm = point.algorithm;
        temp.cameraPosition = point.cameraPosition;
        temp.clusterID = point.clusterID;
        temp.index = point.index;
        temp.listPos = point.listPos;
        temp.overallGrade = point.overallGrade;
        return temp;
    }
}
