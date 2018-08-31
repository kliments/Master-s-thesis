using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Model.Operators;
using UnityEngine;
using Random = UnityEngine.Random;
using Accord.Statistics.Analysis;
using Accord.Statistics.Models.Regression.Linear;

public enum glyphStyles { line, snake, star, stickFigure };
public enum positionIndicator {PC1, PC2 , PC3, value};

public class GlyphVisualization : GenericVisualization
{

    public abstractGlyph[] glyphPrefabs;

    private List<abstractGlyph> currentGlyphList = new List<abstractGlyph>();
    private float[] maxValues = null;
    private float[] minValues = null;
    private float[][] currentData;  // First datapoint index, then dimension index
    private float[][] normalizedCurrentData;
    private float[][] currentPCAValues;
    private float[][] normalizedCurrentPCAValues;


    //For menue
    public glyphStyles glyphIndex;
    public int datapointLimit;
    public float glyphSize;
    public float glyphSpaceSize;
    public bool showMiddlePoint;
    public bool glyphsFacePlayer;
    public positionIndicator xPosition;
    public positionIndicator yPosition;
    public positionIndicator zPosition;
    public int xPositionIndex;
    public int yPositionIndex;
    public int zPositionIndex;

    public bool update = false;

    // Use this for initialization
    void Start()
    {
        var viz = GameObject.Find("Visualization");
        viz.AddComponent<GlyphInteractionController>();
        viz.AddComponent<BoxCollider>();


    }

    // Update is called once per frame
    void Update()
    {
        if (update)
        {
            // Replace everything in this if  statement with menue entries.
            removeGlyphs();
            placeGlyphs();
            updateGlyphSpaceScale();
            updateGlyphScale();
            updateGlyphPosition();
            updateGlyphMiddlePoints();
            update = false;
        }

        if (glyphsFacePlayer)
        {
            Transform player = Camera.main.transform;
            foreach (abstractGlyph g in currentGlyphList)
            {
                g.facePlayer(player);
            }
        }
    }

    private void removeGlyphs()
    {
        foreach (abstractGlyph g in currentGlyphList)
        {
            Destroy(g.gameObject);
        }
        currentGlyphList = new List<abstractGlyph>();
    }


    public override void CreateVisualization()
    {
        if (GetOperator().GetRawInputData() == null)
            return;
        removeGlyphs();
        getData();
        placeGlyphs();
        updateGlyphSpaceScale();
        updateGlyphScale();
        updateGlyphPosition();
        updateGlyphMiddlePoints();

        return;
    }

    public void getData()
    {
        List<Vector3> dataPoints = ((SimpleDatamodel)GetOperator().GetRawInputData()).GetCoords();
        transferVectorToFloat(dataPoints);
        normalizedCurrentData = normalizeData(currentData);
        //calculateMinMaxValues();
        getPcaValues(3);
        normalizedCurrentPCAValues = normalizeData(currentPCAValues);
    }

    public void getPcaValues(int nrOfComponents)
    {
        float[][] data = currentData;
        double[][] doubleData = new double[data.Length][];
        for (int x = 0; x < data.Length; x++)
        {
            doubleData[x] = new double[data[x].Length];
            for (int y = 0; y < data[x].Length; y++)
            {
                doubleData[x][y] = data[x][y];
            }
        }
        PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis()
        {
            Method = PrincipalComponentMethod.Center,
            Whiten = true
        };

        MultivariateLinearRegression transform = pca.Learn(doubleData);

        pca.NumberOfOutputs = nrOfComponents;

        double[][] output1 = pca.Transform(doubleData);


        float[][] outAsFloat = new float[output1.Length][];
        for (int x = 0; x < output1.Length; x++)
        {
            outAsFloat[x] = new float[output1[x].Length];
            for (int y = 0; y < output1[x].Length; y++)
            {
                outAsFloat[x][y] = (float)output1[x][y];
            }
        }

        currentPCAValues = outAsFloat;
    }

    public void placeGlyphs()
    {
        for (var index = 0; index < currentData.Length && index < datapointLimit; index++)
        {
            abstractGlyph datapoint = Instantiate(glyphPrefabs[(int)glyphIndex]);
            datapoint.transform.parent = GetOperator().Visualization.gameObject.transform;
            datapoint.transform.localPosition = new Vector3(0, 0, 0);
            /* Since the glyphs are based on the sticky figure glyphs, they expect up to 10 values. The only exception is the star glyph which can take any number of values.
             * The values are always used in this order:
             * 0: x-rotation of middle part
             * 1: x-rotation of first arm
             * 2: x-rotation of second arm
             * 3: x-rotation of first leg
             * 4: x-rotation of second leg
             * 
             * 5: z-rotation of middle part
             * 6: z-rotation of first arm
             * 7: z-rotation of second arm
             * 8: z-rotation of first leg
             * 9: z-rotation of second leg
             * 
             * Since rotating a stick around y does nothing at all, there is no value for y rotation.
             * Also, it is better to ignore the z rotation, as rotation is not invariant. (rotating around x first and then z results in a different rotation as rotating around z first.)
             * 
             * Note that Line glyphs only consist of the main arm and therefore only use value 0 and 5.
             * If you do not want to use a value, just set it to 0.
             */
            float[] values = new float[10];
            switch (glyphIndex)
            {
                case glyphStyles.line: // 2 values needed
                    values[0] = currentData[index][0];
                    break;
                case glyphStyles.snake:
                    values[0] = currentData[index][0];
                    values[1] = currentData[index][1];
                    values[2] = currentData[index][2];
                    values[3] = currentData[index][1];
                    values[4] = currentData[index][2];
                    break;
                case glyphStyles.star:
                    values = new float[] { currentData[index][0], currentData[index][1], currentData[index][2] };
                    break;
                case glyphStyles.stickFigure:
                    values[0] = currentData[index][0];
                    values[1] = currentData[index][1];
                    values[2] = currentData[index][2];
                    values[3] = currentData[index][1];
                    values[4] = currentData[index][2];
                    break;
            }
            datapoint.setValues(values);
            currentGlyphList.Add(datapoint);
        }
    }

    private void transferVectorToFloat(List<Vector3> vectors)
    {
        currentData = new float[vectors.Count][];
        for (int v = 0; v < vectors.Count; v++)
        {
            currentData[v] = new float[] { vectors[v].x, vectors[v].y, vectors[v].z};
        }
    }
    
    /*
    private void calculateMinMaxValues()
    {
        minValues = new float[currentData[0].Length];
        maxValues = new float[currentData[0].Length];
        for(int d = 0; d < currentData[0].Length; d++)
        {
            float dMin = float.MaxValue;
            float dMax = float.MinValue;
            for(int v = 0; v < currentData.Length; v++)
            {
                if(dMin > currentData[v][d])
                {
                    dMin = currentData[v][d];
                }
                if (dMax < currentData[v][d])
                {
                    dMax = currentData[v][d];
                }
            }
            minValues[d] = dMin;
            maxValues[d] = dMax;
        }
    }

    private float normalizeValueInDimension(float value, int dimension)
    {
        return (value - minValues[dimension]) / (maxValues[dimension] - minValues[dimension]);
    }*/

    public static float[][] normalizeData(float[][] input)
    {
        float[][] output = new float[input.Length][];
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = new float[input[i].Length];
        }
        for (int j = 0; j < input[0].Length; j++)
        {
            float min = float.MaxValue;
            float max = float.MinValue;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i][j] < min)
                {
                    min = input[i][j];
                }
                if (input[i][j] > max)
                {
                    max = input[i][j];
                }
            }

            float diff = (max - min);


            for (int i = 0; i < input.Length; i++)
            {
                output[i][j] = (input[i][j] - min) / diff;
            }
        }
        return output;
    }

    private void updateGlyphPosition()
    {
        for(int i = 0; i < currentGlyphList.Count; i++)
        {
            Transform t = currentGlyphList[i].transform;
            float newX = 0f;
            float newY = 0f;
            float newZ = 0f;

            switch (xPosition)
            {
                case (positionIndicator.PC1):
                    newX = normalizedCurrentPCAValues[i][0];
                    break;
                case (positionIndicator.PC2):
                    newX = normalizedCurrentPCAValues[i][1];
                    break;
                case (positionIndicator.PC3):
                    newX = normalizedCurrentPCAValues[i][2];
                    break;
                case (positionIndicator.value):
                    newX = normalizedCurrentData[i][xPositionIndex];
                    break;
            }

            switch (yPosition)
            {
                case (positionIndicator.PC1):
                    newY = normalizedCurrentPCAValues[i][0];
                    break;
                case (positionIndicator.PC2):
                    newY = normalizedCurrentPCAValues[i][1];
                    break;
                case (positionIndicator.PC3):
                    newY = normalizedCurrentPCAValues[i][2];
                    break;
                case (positionIndicator.value):
                    newY = newX = normalizedCurrentData[i][yPositionIndex];
                    break;
            }

            switch (zPosition)
            {
                case (positionIndicator.PC1):
                    newZ = normalizedCurrentPCAValues[i][0];
                    break;
                case (positionIndicator.PC2):
                    newZ = normalizedCurrentPCAValues[i][1];
                    break;
                case (positionIndicator.PC3):
                    newZ = normalizedCurrentPCAValues[i][2];
                    break;
                case (positionIndicator.value):
                    newZ = newX = normalizedCurrentData[i][zPositionIndex];
                    break;
            }

            Vector3 newPos = new Vector3(newX, newY, newZ);
            t.localPosition = newPos;
        }
    }

    private void updateGlyphScale()
    {
        foreach (abstractGlyph g in currentGlyphList)
        {
            g.transform.localScale = new Vector3(glyphSize, glyphSize, glyphSize);
        }
    }

    private void updateGlyphSpaceScale()
    {
        this.transform.localScale = new Vector3(glyphSpaceSize, glyphSpaceSize, glyphSpaceSize);
    }

    private void updateGlyphMiddlePoints()
    {
        GameObject[] markers = GameObject.FindGameObjectsWithTag("middle_marker");
        foreach (GameObject mark in markers)
        {
            if (showMiddlePoint)
            {
                mark.GetComponent<middle_marker>().visible = true;
            }
            else
            {
                mark.GetComponent<middle_marker>().visible = false;
            }
            mark.GetComponent<middle_marker>().updateSize();
        }
    }
}
