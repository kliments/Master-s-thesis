using System;
using System.IO;
using Assets.Scripts.Model;
using UnityEngine;

public class DatageneratorOperator : GenericOperator
{

    private bool threeDim = true;
    private string type = "normal";
    private int correlationDim1 = 0;
    private int correlationDim2 = 1;


    public override bool Process()
    {
        float[][]  data = generateData();
        var dataModel = new SimpleDatamodel(); // ReadCsv().MergeDatamodels(_rawInputData);
        dataModel.addFloatMatrixColwise(data);
        SetOutputData(dataModel);

        // Transform data into dataModel here.

        // add here data generation
        // Analog to Dataloader: 
        // var dataModel = ReadCsv().MergeDatamodels(_rawInputData);
        // SetOutputData(dataModel);

        return true;
    }

    public override bool ValidateIfOperatorPossibleForParents(GenericOperator parent)
    {
        return true;
    }

    public override void Start()
    {
        base.Start();
    }
	
	private float[][] generateData()
    {
        // Setting some variables that affect the data generation. It is likely that some of them contradict with each other (for example: some variances for normal distributions are likely not possible depending on the min and max values in each dimension.)
        float[] minValues;                      // The minimal value a data point should have in each dimension. Should not throw errors if not defined for a dimension but instead assume 0.
        float[] maxValues;                      // The maximal value a data point should have in each dimension. Should not throw errors if not defined for a dimension but instead assume 1.
        int numberOfDataPoints;                 // Number of dta points to create.
        int numberOfDimensions;                 // Number of dimensions each data point has.

        //bool correlation;                       // If true, two dimensions which are specified in another variable should correlate with each other. It would be nice if this can be extended to several dimensions correlating with each toher but for now two dimensions are fine.
        float correlationStrengths;             // If correlations are used, this contains the target correlation coeficient. Possible value are between -1 and 1.
        //int correlationDim1;                    // Contains the idnex of the first correlating dimension.
        //int correlationDim2;                    // Contains the idnex of the second correlating dimension.

        //string[] distributionType = { "equal" };         // Set the data distribution type. Should include normal distribution (e.g. bell shaped) and equal distribution (e.g. completely random).
        string distributionType;
        float[] normalDistributionCenters;      // Contains the middle points of equal distribution. Should not throw errors if not initialized and no normal distribution is used.
        //float[] normalDistributionVariance;     // Contains the variance of equal distribution. If you want, you can instead save the standard deviation. Should not throw errors if not initialized and no normal distribution is used.
        float stddev; // Standard deviation of normal distribution. Default value is 1.0f.
        float mean; // Meand of normal distribution. Default value is 0.0f


        //Set various parameters manually

        

        //Mean and Standard deviation for normal distribution
        stddev = 1f;
        mean = 0f;


        //initialize minValues Array 
        //for positions x, y and z 
        minValues = new float[3];
        minValues[0] = 0f;
        minValues[1] = 0f;
        minValues[2] = 0f;

        //initialize maxValues Array  
        //for positions x, y, z
        maxValues = new float[3];
        maxValues[0] = 1f;
        maxValues[1] = 1f;
        maxValues[2] = 1f;

        

        //set numberOfDataPoints 
        numberOfDataPoints = 5000;

        //set numberOfDimensions 
        numberOfDimensions = 3;

        //set distributionType manually [equal, correlated, normal]
        distributionType = this.type;
        
        
        correlationStrengths = 0.25f;

        //set normal distribution centers
        normalDistributionCenters = new float[3];
        normalDistributionCenters[0] = 0.75f;
        normalDistributionCenters[1] = 0.75f;
        normalDistributionCenters[2] = 0.75f;



        return getSampleData(minValues, maxValues, numberOfDataPoints, numberOfDimensions, distributionType, correlationStrengths, normalDistributionCenters, mean, stddev);
    }

    /*
     * Creates some 3D sample data using random numbers.
     */
    private float[][] getSampleData(float[] min, float[] max, int amount, int dims, string type, float correlationStrength, float[] origins, float mean, float stddev)
    {

        //check whether min and max arrays contain meaningful values.
        for (int i = 0; i < 3; i++)
        {
            if (min[i] >= max[i])
            {
                min[i] = 0f;
                max[i] = 1f;
                Debug.Log("MaxValues should be larger than minValues. Set min to default 0 and max to default 1");

            }
        }



        //in case of correlation 0, distribution is same as "equal"
        if (type == "correlated" && correlationStrength == 0)
        {
            type = "equal";
        }


        //check correlationStrength for valid entries. Set to allowed minimum or maximum respectively if they are not in the valid range
        if (correlationStrength < -1)
        {
            correlationStrength = -1;
            Debug.Log("Set correlation strength to lowest valid value (-1.0!");
        }
        else if (correlationStrength > 1)
        {
            correlationStrength = 1;
            Debug.Log("Set correlation strength to largest valid value (1.0!");
        }

        bool showCorners = false;       // If true, overwrites some points to be in the very corners of a 3D space.



        // Set min and max values for all dimensions to the values given in the parameter settings
        float minX = min[0];
        float maxX = max[0];

        float minY = min[1];
        float maxY = max[1];

        float minZ = min[2];
        float maxZ = max[2];



        // Set dimensions that should be correlated 
        int correlatedDim1 = this.correlationDim1;
        int correlatedDim2 = this.correlationDim2;

        float minValue = 1f;
        float maxValue = 0f;
        

        float[][] sampleData = new float[amount][];
        for (int i = 0; i < amount; i++)
        {
            sampleData[i] = new float[3];
            for (int j = 0; j < 3; j++)
            {
                if (type.Equals("normal"))
                {
                    
                    //generate normally distributed random numbers
                    //using Box-Muller-method
                    // https://de.wikipedia.org/wiki/Box-Muller-Methode

                    double rand1 = (float)UnityEngine.Random.Range(0f, 1f);
                    double rand2 = (float)UnityEngine.Random.Range(0f, 1f);

                    

                    float norm1 =  (float) (System.Math.Sqrt((-2 * System.Math.Log(rand1))) * System.Math.Cos(2*System.Math.PI*rand2));
                    float norm2 = (float)(System.Math.Sqrt((-2 * System.Math.Log(rand1))) * System.Math.Sin(2 * System.Math.PI * rand2));

                    
                    //transform data based on given mean and standard deviation
                    float z1 = mean + (stddev * norm1);
                    float z2 = mean + (stddev * norm2);

                    if (j == 0)
                    {
                        sampleData[i][j] = z1;
                    }
                    else if (j == 1)
                    {
                        sampleData[i][j] = z2;
                    }
                    else if (j == 2)
                    {
                        //set sampleData[i][j] = z2 for 3d case and to 0 for 2d case
                        if (threeDim == true)
                        {
                            sampleData[i][j] = z2;
                        }
                        else
                        {
                            sampleData[i][j] = 0;
                        }
                        
                    }


                }


                else if (type.Equals("correlated"))
                {

                    if (j != correlatedDim2)
                    {
                        if (j == 0)
                        {
                            if (j!= correlatedDim1 && threeDim == false)
                            {
                                //TODO: Expand here for multiple correlation case
                                //initialize first dimension randomly
                                //other two dimensions should correlate with either first dimension or with each other
                                //
                                //__________ EXAMPLE ___________
                                //
                                // dim0 : random
                                // dim1 : correlated with dim0 
                                // dim2 : correlated with dim1
                                //

                                sampleData[i][j] = 0f;
                            }
                            else
                            {
                                sampleData[i][j] = UnityEngine.Random.Range(minX, maxX);
                            }
                            
                        }
                        else if (j == 1)
                        {
                            if (j != correlatedDim1 && threeDim == false)
                            {
                                sampleData[i][j] = 0f;
                            }
                            else
                            {
                                sampleData[i][j] = UnityEngine.Random.Range(minY, maxY);
                            }
                            
                        }
                        else if (j == 2)
                        {
                            if (j != correlatedDim1 && threeDim == false)
                            {
                                sampleData[i][j] = 0f;
                            }
                            else
                            {
                                sampleData[i][j] = UnityEngine.Random.Range(minZ, maxZ);
                            }
                        }


                    }
                    else
                    {
                        // generate correlated numbers based on formula COMPUTE Y=X*r+Y*SQRT(1-r**2)
                        // found on https://www.uvm.edu/~dhowell/StatPages/More_Stuff/Gener_Correl_Numbers.html

                        sampleData[i][j] = (sampleData[i][correlatedDim1] * correlationStrength) + (UnityEngine.Random.Range(min[j], max[j]) * Mathf.Sqrt(1 - (Mathf.Pow(correlationStrength, 2))));



                        if (sampleData[i][j] < minValue && j == correlatedDim2)
                        {
                            minValue = sampleData[i][j];
                        }
                        if (sampleData[i][j] > maxValue && j == correlatedDim2)
                        {
                            maxValue = sampleData[i][j];
                        }
                    }






                }


                //generation method in case of equal data distribution
                else if (type.Equals("equal"))
                {
                    if (j == 0)
                    {
                        sampleData[i][j] = UnityEngine.Random.Range(minX, maxX);
                    }
                    else if (j == 1)
                    {
                        sampleData[i][j] = UnityEngine.Random.Range(minY, maxY);
                    }
                    else
                    {
                        if (threeDim == true)
                        {
                            sampleData[i][j] = UnityEngine.Random.Range(minZ, maxZ);
                        }
                        else
                        {
                            sampleData[i][j] = 0f;
                        }
                        
                    }
                }


            }
        }





        //calculate actual correlation coefficient in case of type being correlated distribution
        if (type == "correlated")
        {
            determineCorrelation(sampleData, correlatedDim1, correlatedDim2, correlationStrength);
        }

        //calculate actual mean and standard deviation in case of type being normal distribution
        else if (type == "normal")
        {
            determineNormalDistribution(sampleData, mean, stddev);
        }


        //call normalization method with cube length as parameters
        sampleData = normalize(min, max, sampleData);


        //call move method to move distribution to given center in case of normal distribution
        sampleData = move(origins, sampleData, max[0]);



        if (showCorners)
        {
            if (amount > 8)
            {
                sampleData[0][0] = 0;
                sampleData[0][1] = 0;
                sampleData[0][2] = 0;

                sampleData[1][0] = 1;
                sampleData[1][1] = 0;
                sampleData[1][2] = 0;

                sampleData[2][0] = 0;
                sampleData[2][1] = 1;
                sampleData[2][2] = 0;

                sampleData[3][0] = 1;
                sampleData[3][1] = 1;
                sampleData[3][2] = 0;

                sampleData[4][0] = 0;
                sampleData[4][1] = 0;
                sampleData[4][2] = 1;

                sampleData[5][0] = 1;
                sampleData[5][1] = 0;
                sampleData[5][2] = 1;

                sampleData[6][0] = 0;
                sampleData[6][1] = 1;
                sampleData[6][2] = 1;

                sampleData[7][0] = 1;
                sampleData[7][1] = 1;
                sampleData[7][2] = 1;
            }
            else
            {
                Debug.Log("Can not show borders. Not enought points created that could be overwritten!");
            }
        }



        //return Array containing data points after normalization 
        return sampleData;
    }
	
	//move points according to given offset
    private float[][] move(float[] normalCenters, float[][] data, float max)
    {

        float[][] sampleData = data;

        //do the following calculations only in case of normal distribution. Otherwise, return the unchanged array
        if(type != "normal")
        {
            return sampleData;
        }

        for(int i = 0; i < sampleData.Length; i++)
        {
            for(int j = 0; j < sampleData[i].Length; j++)
            {
                sampleData[i][j] = sampleData[i][j] - ((max/2) - normalCenters[j]);
                if(threeDim == false && j == 2)
                {
                    sampleData[i][j] = 0f;
                }
            }
        }

        return sampleData;
    }

    //normalize data to cube constraints
    private float[][] normalize (float[] min, float[] max, float[][] points)
    {
        //set min and max to values that cannot occur in the real data so they will be overwritten in the following nested loop
        float observedMaxX = -2f;
        float observedMinX = 2f;
        float observedMaxY = -2f;
        float observedMinY = 2f;
        float observedMaxZ = -2f;
        float observedMinZ = 2f;



 

        for (int i = 0; i < points.Length; i++)
        {
            for (int j = 0; j < points[i].Length; j++)
            {
                if(j == 0)
                {
                    if (points[i][j] < observedMinX)
                    {
                        observedMinX = points[i][j];
                    }
                    if (points[i][j] > observedMaxX)
                    {
                        observedMaxX = points[i][j];
                    }
                }
                else if (j == 1)
                {
                    if (points[i][j] < observedMinY)
                    {
                        observedMinY = points[i][j];
                    }
                    if (points[i][j] > observedMaxY)
                    {
                        observedMaxY = points[i][j];
                    }
                }
                else if (j == 2)
                {
                    if (points[i][j] < observedMinZ)
                    {
                        observedMinZ = points[i][j];
                    }
                    if (points[i][j] > observedMaxZ)
                    {
                        observedMaxZ = points[i][j];
                    }
                }




            }
        }

        for (int i = 0; i < points.Length; i++)
        {
            for (int j = 0; j < points[i].Length; j++)
            {
                if (j == 0)
                {
                    points[i][j] = (max[0] - min[0]) / (observedMaxX - observedMinX) * (points[i][j] - observedMaxX) + max[0];
                }
                else if (j == 1)
                {
                    points[i][j] = (max[1] - min[1]) / (observedMaxY - observedMinY) * (points[i][j] - observedMaxY) + max[1];
                }
                else if (j == 2)
                {
                    points[i][j] = (max[2] - min[2]) / (observedMaxZ - observedMinZ) * (points[i][j] - observedMaxZ) + max[2];

                    //set z values to 0 in 2D case
                    if (threeDim == false) 
                    {
                        points[i][j] = 0f;
                    }

                }

                
            }
        }



        return points;
    }


    //determine correlation coefficient
    private void determineCorrelation(float[][] data, int dim1, int dim2, float coefficient)
    {
        float[][] sampleData = data;
        int correlatedDim1 = dim1;
        int correlatedDim2 = dim2;
        float correlationStrength = coefficient;


        float meanX = 0f;
        float meanY = 0f;
        float meanZ = 0f;


        for (int i = 0; i < sampleData.Length; i++)
        {
            for (int j = 0; j < sampleData[i].Length; j++)
            {
                if (j == 0)
                {
                    meanX = meanX + sampleData[i][j];
                }
                if (j == 1)
                {
                    meanY = meanY + sampleData[i][j];
                }
                if (j == 2)
                {
                    meanZ = meanZ + sampleData[i][j];
                }
            }
        }

        meanX = meanX / sampleData.Length;
        meanY = meanY / sampleData.Length;
        meanZ = meanZ / sampleData.Length;


        float mean1 = 0f;
        float mean2 = 0f;

        if (correlatedDim1 == 0)
        {
            mean1 = meanX;
        }
        else if (correlatedDim1 == 1)
        {
            mean1 = meanY;
        }
        else
        {
            mean1 = meanZ;
        }

        if (correlatedDim2 == 0)
        {
            mean2 = meanX;
        }
        else if (correlatedDim2 == 1)
        {
            mean2 = meanY;
        }
        else
        {
            mean2 = meanZ;
        }


        float firstDiff = 0f;
        float secondDiff = 0f;
        float top = 0f;
        float bottomLeft = 0f;
        float bottomRight = 0f;
        float thirdDiff = 0f;
        float fourthDiff = 0f;
        float bottom = 0f;
        float pearson = 0f;


        for (int i = 0; i < sampleData.Length; i++)
        {
            for (int j = 0; j < sampleData[i].Length; j++)
            {
                if (j == correlatedDim1)
                {
                    firstDiff = sampleData[i][j] - mean1;
                    thirdDiff = Mathf.Pow((sampleData[i][j] - mean1), 2);
                }
                if (j == correlatedDim2)
                {
                    secondDiff = sampleData[i][j] - mean2;
                    fourthDiff = Mathf.Pow((sampleData[i][j] - mean2), 2);
                }
            }

            top = top + (firstDiff * secondDiff);
            bottomLeft = bottomLeft + thirdDiff;
            bottomRight = bottomRight + fourthDiff;

        }

        bottomLeft = Mathf.Sqrt(bottomLeft);
        bottomRight = Mathf.Sqrt(bottomRight);

        bottom = bottomLeft * bottomRight;

        pearson = top / bottom;


        Debug.Log("Correlation comparison... Desired value = " + correlationStrength + " , actual value = " + pearson);

    }

    //determine actual mean and standard deviation
    private void determineNormalDistribution(float[][] data, float givenMean, float givenStdDev)
    {
        float[][] sampleData = data;
        float mean = givenMean;
        float stddev = givenStdDev;

        float actualMean = 0f;
        float actualStdDv = 0f;

        for (int i = 0; i < sampleData.Length; i++)
        {
            for (int j = 0; j < sampleData[i].Length; j++)
            {
                if (j == 0)
                {
                    actualMean = actualMean + sampleData[i][j];
                }
            }
        }


        actualMean = actualMean / sampleData.Length;


        for (int i = 0; i < sampleData.Length; i++)
        {
            for (int j = 0; j < sampleData[i].Length; j++)
            {
                if (j == 0)
                {
                    actualStdDv = actualStdDv + (Mathf.Pow(sampleData[i][j] - actualMean, 2));
                }

            }
        }

        actualStdDv = Mathf.Sqrt(actualStdDv / sampleData.Length);


        Debug.Log("Comparison for normal distribution... Given Values: mean " + mean + " , standard deviation: " + stddev + "\n" + "actual mean: " + actualMean + " ,actual standard deviation: " + actualStdDv);

    }

    public override void StoreData()
    {
        data.name = gameObject.name.Replace("(Clone)", "");
        data.ID = Id;
        if (Parents == null || Parents.Count == 0) data.parent = -1;
        else data.parent = Parents[0].Id;
        data.posX = GetIcon().transform.position.x;
        data.posY = GetIcon().transform.position.y;
        data.posZ = GetIcon().transform.position.z;
    }

    public override void LoadSpecificData(OperatorData data)
    {

    }
}
