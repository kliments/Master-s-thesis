using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NumericDatamodel : MonoBehaviour {

    

    public string nameOfDatafile;
    public bool hasheader;
    public string delimiter;
    public bool debugMode;

    private float[][] myData;
    private float[][] unfilteredNormalizedData;
    private float[][] filteredNormalizedData;
    private float[][] unfilteredGlobalyNormalizedData;
    private float[][] filteredGlobalyNormalizedData;
    private string[] myHeaders;
    private Vector2[] myFilterValues;               // Do not directly read this. Use getCurrentFilterValues() instead. May be inconsistent otherwise.
    private bool[] currentlyFiltered;               // Do not directly read this. Use getCurrentlyFiltered() instead. May be inconsistent otherwise.
    private Vector2[] filteredMinMaxValues;         // Do not directly read this. Use getMinMaxValues(bool ignoreFilter) instead. May be inconsistent otherwise.
    private bool minMaxIsDirty = true;
    private bool filteredRowsIsDirty = true;
    private bool filteredNormalizationIsDirty = true;
    private bool filteredGlobalNormalizationIsDirty = true;
    private Vector2[] unfilteredMinMaxValues;       // Do not directly read this. Use getMinMaxValues(bool ignoreFilter) instead. May be inconsistent otherwise.
    private int amountOfRows;
    private int amountOfCols;
    private int amountOfUnfilteredDatapoints;
    private List<IDatamodelListener> myListeners = new List<IDatamodelListener>();

    // TODO create seperate method for getting global normalized data. 
    // TODO add a debug bool that will create console prints of which method is called.

    // Use this for initialization
    void Awake () {
        if (debugMode)
        {
            Debug.Log("Awake NumericalDatamodel");
        }
        if (nameOfDatafile.EndsWith(".csv"))
        {
            myData = DatasetImporter.readNumericCSV(nameOfDatafile, hasheader, delimiter);
            amountOfRows = myData.Length;
            amountOfCols = myData[0].Length;
            myHeaders = new string[]{ };
            if (hasheader)
            {
                myHeaders = DatasetImporter.getHeader(nameOfDatafile, delimiter);
            }
            initFilter();
            recalculateUnfilteredNormalizedData();
            recalculateUnfilteredGlobalyNormalizedData();
            //test();
            if (debugMode)
            {
                Debug.Log("Finished awaking NumericalDatamodel");
            }
            return;
        }

        throw new FileLoadException("Unsupported data format of file '" + nameOfDatafile + "'. Can not load data into datamodel.");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void test()
    {
        Debug.Log("Global Min Max: " + getGlobalMinMaxValues(false));
        this.setFilter(0, 0, 4, false);
        Debug.Log("Set Filter...");
        Debug.Log("Global Min Max: " + getGlobalMinMaxValues(false));
        Debug.Log("Global Min Max ignoring filters: " + getGlobalMinMaxValues(true));
        Debug.Log(getCurrentData(false).Length);
    }

    private void recalculateFilteredNormalizedData()
    {
        if (debugMode)
        {
            Debug.Log("Recalculating filtered normalized data in numerical Datamodel.");
        }
        filteredNormalizedData = new float[amountOfRows][];
        Vector2[] minmax = getMinMaxValues(false);
        for (int i = 0; i < amountOfRows; i++)
        {
            filteredNormalizedData[i] = new float[amountOfCols];
            for (int j = 0; j < amountOfCols; j++)
            {
                filteredNormalizedData[i][j] = (myData[i][j] - minmax[j].x) / (minmax[j].y - minmax[j].x);
            }
        }
    }

    private void recalculateFilteredGlobalyNormalizedData()
    {
        if (debugMode)
        {
            Debug.Log("Recalculating global filtered normalized data in numerical Datamodel.");
        }
        filteredGlobalyNormalizedData = new float[amountOfRows][];
        Vector2 minmax = getGlobalMinMaxValues(false);
        for (int i = 0; i < amountOfRows; i++)
        {
            filteredGlobalyNormalizedData[i] = new float[amountOfCols];
            for (int j = 0; j < amountOfCols; j++)
            {
                filteredGlobalyNormalizedData[i][j] = (myData[i][j] - minmax.x) / (minmax.y - minmax.x);
            }
        }
    }

    


    private void recalculateUnfilteredNormalizedData()
    {
        if (debugMode)
        {
            Debug.Log("Recalculating unfiltered normalized data in numerical Datamodel.");
        }
        unfilteredNormalizedData = new float[amountOfRows][];
        Vector2[] minmax = getMinMaxValues(true);
        for(int i = 0; i < amountOfRows; i++)
        {
            unfilteredNormalizedData[i] = new float[amountOfCols];
            for(int j = 0; j < amountOfCols; j++)
            {
                unfilteredNormalizedData[i][j] = (myData[i][j] - minmax[j].x) / (minmax[j].y - minmax[j].x);
            }
        }
    }

    private void recalculateUnfilteredGlobalyNormalizedData()
    {
        if (debugMode)
        {
            Debug.Log("Recalculating global unfiltered normalized data in numerical Datamodel.");
        }
        unfilteredGlobalyNormalizedData = new float[amountOfRows][];
        Vector2 minmax = getGlobalMinMaxValues(true);
        for (int i = 0; i < amountOfRows; i++)
        {
            unfilteredGlobalyNormalizedData[i] = new float[amountOfCols];
            for (int j = 0; j < amountOfCols; j++)
            {
                unfilteredGlobalyNormalizedData[i][j] = (myData[i][j] - minmax.x) / (minmax.y - minmax.x);
            }
        }
    }

    /**
     * Adds an Object that implements IDatamodelListener as a listener to this datamodel.
     * It will receive an update containing this model whenever there is a biger change in the model.
     * This likely means that the view should be redrawn.
     * */
    public void addListener(IDatamodelListener listener)
    {
        if (debugMode)
        {
            Debug.Log("Adding a listener to numerical Datamodel.");
        }
        myListeners.Add(listener);

        if (debugMode)
        {
            Debug.Log("Now containing " + myListeners.Count + " listeners.");
        }
        
    }

    /**
     * Removes an object from the list of listeners. Opposite of addListener(IDatamodelListener listener).
     * */
    public void removeListener(IDatamodelListener listener)
    {
        if (debugMode)
        {
            Debug.Log("Removing a Listener to numerical Datamodel.");
        }
        myListeners.Remove(listener);

        if (debugMode)
        {
            Debug.Log("Now containing " + myListeners.Count + " listeners.");
        }
    }

    /**
     * Sends a signal to each listener that the filters changed.
     * */
    private void notifyListenersOnFilterChange()
    {
        if (debugMode)
        {
            Debug.Log("Notifying Listeners of numerical Datamodel about filter change. " + myListeners.Count + " listeners are found.");
        }
        foreach (IDatamodelListener listener in myListeners)
        {
            listener.datamodelFilterChange(this);
        }
    }

    /**
     * Sends a signal to each listener that the min/max values changed.
     * */
    private void notifyListenersOnMinMaxChange()
    {
        if (debugMode)
        {
            Debug.Log("Notifying Listeners of numerical Datamodel about minmax change. " + myListeners.Count + " listeners are found.");
        }
        foreach (IDatamodelListener listener in myListeners)
        {
            listener.datamodelFilterChange(this);
        }
    }

    /**
     * Initializes the filter system.
     * */
    private void initFilter()
    {
        if (debugMode)
        {
            Debug.Log("Initializing filters of numreical datamodel.");
        }
        currentlyFiltered = new bool[amountOfRows];
        for (int i = 0; i < amountOfRows; i++)
        {
            currentlyFiltered[i] = false;
        }

        
        myFilterValues = new Vector2[amountOfCols];
        
        for (int i = 0; i < amountOfCols; i++)
        {
            myFilterValues[i] = new Vector2(float.MinValue, float.MaxValue);
        }
        
    }

    /**
     * Returns the minimal and maximal values in the dataset.
     * If you want the min and max values for each column, use getMinMaxValues(bool ignoreFilter) instead.
     * If ignoreFilter is false, only non filtered rows are used for this calculation.
     * */
    private Vector2 getGlobalMinMaxValues(bool ignoreFilter)
    {
        if (debugMode)
        {
            Debug.Log("Getting global MinMax values of numreical datamodel.");
        }
        Vector2[] minMax = getMinMaxValues(ignoreFilter);
        float min = float.MaxValue;
        float max = float.MinValue;
        for(int i = 0; i < minMax.Length; i++)
        {
            if(minMax[i].x < min)
            {
                min = minMax[i].x;
            }

            if (minMax[i].y > max)
            {
                max = minMax[i].y;
            }
        }
        return new Vector2(min, max);
    }

    /**
     * Recalculates the min and max values and saves them in a variable.
     * */
    private void recalculateMinMaxValues()
    {
        if (debugMode)
        {
            Debug.Log("Recalculating MinMax values of numreical datamodel.");
        }
        unfilteredMinMaxValues = new Vector2[amountOfCols];
        filteredMinMaxValues = new Vector2[amountOfCols];
        bool[] filteredValues = getCurrentlyFiltered();
        for (int j = 0; j < amountOfCols; j++)
        {
            float unfilteredMin = float.MaxValue;
            float unfilteredMax = float.MinValue;
            float filteredMin = float.MaxValue;
            float filteredMax = float.MinValue;
            for (int i = 0; i < amountOfRows; i++)
            {
                if(myData[i][j] < unfilteredMin)
                {
                    unfilteredMin = myData[i][j];
                }
                if(myData[i][j] > unfilteredMax)
                {
                    unfilteredMax = myData[i][j];
                }
                if (!filteredValues[i])
                {
                    if (myData[i][j] < filteredMin)
                    {
                        filteredMin = myData[i][j];
                    }
                    if (myData[i][j] > filteredMax)
                    {
                        filteredMax = myData[i][j];
                    }
                }
            }
            unfilteredMinMaxValues[j] = new Vector2(unfilteredMin, unfilteredMax);
            filteredMinMaxValues[j] = new Vector2(filteredMin, filteredMax);
        }
        minMaxIsDirty = false;
        notifyListenersOnMinMaxChange();
    }

    /**
     * Returns the minimal and maximal values for each column in the dataset.
     * If you want the global min and max values, use getGlobalMinMaxValues(bool ignoreFilter) instead.
     * If ignoreFilter is false, only non filtered rows are used for this calculation.
     * */
    private Vector2[] getMinMaxValues(bool ignoreFilter)
    {
        if (debugMode)
        {
            Debug.Log("Getting MinMax values per column of numreical datamodel.");
        }
        checkForDirtyMinMax();
        if (ignoreFilter)
        {
            return unfilteredMinMaxValues;
        }
        else
        {
            return filteredMinMaxValues;
        }
        
    }

    /**
     * Returns the data represented by this datamodel.
     * If ignoreFilters is true, all data will be returned.
     * If ignoreFilters is false, filters get applied to the data and only the remaining data is returned. In this case, the index of datapoints may change if the number of returned data points changes.
     * If you want to keep track of the position of specific datapoints, use ignoreFilters=true and get the
     * information on which data points are filtered with getCurrentlyFiltered().
     * */
    public float[][] getCurrentData(bool ignoreFilters)
    {
        if (debugMode)
        {
            Debug.Log("Getting current data of numreical datamodel.");
        }
        // TODO implement normalization
        float[][] returnvalue = myData;
        if (!ignoreFilters) {
            returnvalue = doFiltering(returnvalue);
        };
        return returnvalue;
    }

    /**
     * Returns the data normalized. If globalyNormalized the global min and max values are used for normalization.
     * If globalyNormalized = false, the min and max values of each column is used.
     * If ignoreFiltersForNormalization = true, the normalization will be done without filtering the data.
     * If ignoreFiltersForNormalization = false, the normalization will be done by using the max and min values among the non-filtered rows.
     * For documentation on ignoreFilters see the getCurrentData(bool ignoreFilters) method.
     * Calling this function several times without the normalized values changing (for example by changing the filter)
     * will not recalculate the normalization again, so it should be performant.
     * */
    public float[][] getNormalizedData(bool ignoreFilters, bool ignoreFiltersForNormalization, bool globalyNormalized)
    {
        if (debugMode)
        {
            Debug.Log("Getting normalized MinMax values of numreical datamodel.");
        }
        float[][] returnvalue = myData;
        if(ignoreFiltersForNormalization && globalyNormalized)
        {
            returnvalue = unfilteredGlobalyNormalizedData;
        }
        if (!ignoreFiltersForNormalization && globalyNormalized)
        {
            checkForDirtyFilteredGlobalNormalization();
            returnvalue = filteredGlobalyNormalizedData;
        }
        if (ignoreFiltersForNormalization && !globalyNormalized)
        {
            returnvalue = unfilteredNormalizedData;
        }
        if (!ignoreFiltersForNormalization && !globalyNormalized)
        {
            checkForDirtyFilteredNormalization();
            returnvalue = filteredNormalizedData;
        }
        if (!ignoreFilters)
        {
            returnvalue = doFiltering(returnvalue);
        };
        return returnvalue;
    }

    private float[][] doFiltering(float[][] unfilteredData)
    {
        if (debugMode)
        {
            Debug.Log("Find out which data is filtered in numerical Datamodel.");
        }
        float[][] returnvalue = unfilteredData;
        int amountOfUnfilteredDP = getNumberOfUnfilteredDatapoints();
        bool[] filterArray = getCurrentlyFiltered();
        returnvalue = new float[amountOfUnfilteredDP][];
        int currentIndex = 0;
        for (int i = 0; i < amountOfUnfilteredDP; i++)
        {
            if (!filterArray[i])
            {
                returnvalue[currentIndex] = unfilteredData[i];
                currentIndex++;
            }
        }
        return returnvalue;
    }

    /**
     * Returns the number of datapoints that are not disabled by filters.
     * */ 
    public int getNumberOfUnfilteredDatapoints()
    {
        if (debugMode)
        {
            Debug.Log("Checking number of unfiltered points in numerical datamodel.");
        }
        checkForDirtyFilter();
        return amountOfUnfilteredDatapoints;
    }

    /**
     * Sets the filter in the given dimension to be between min and max.
     * All Data point smaller than min or higher than max will be filtered.
     * If instantlyRecalculateFilteredRows is true, the calculation of which rows are filtered will also be done.
     * Otherwise, this calculation will be done the next time it is needed (for example when getData() without ignoring filters is called).
     * When several filters are set at once, this may save a lot of processing power.
     * */
    public void setFilter(int dimension, float min, float max, bool instantlyRecalculateFilteredRows)
    {
        if (debugMode)
        {
            Debug.Log("Setting a filter in a numerical datamodel.");
        }
        myFilterValues[dimension] = new Vector2(min, max);
        filteredRowsIsDirty = true;
        minMaxIsDirty = true;
        if (instantlyRecalculateFilteredRows) { checkForDirtyFilter(); };
    }

    private void recalculateFilteredRows()
    {
        if (debugMode)
        {
            Debug.Log("Recalculating filtered rows of numerical datamodel.");
        }
        amountOfUnfilteredDatapoints = amountOfRows;
        for(int i = 0; i < amountOfRows; i++)
        {
            currentlyFiltered[i] = false;
            for (int j = 0; j < amountOfCols; j++)
            {
                if(!(myData[i][j] > myFilterValues[j].x && myData[i][j] < myFilterValues[j].y))
                {
                    currentlyFiltered[i] = true;
                    amountOfUnfilteredDatapoints--;
                    break;
                }
            }
        }
        minMaxIsDirty = true;
        filteredNormalizationIsDirty = true;
        filteredGlobalNormalizationIsDirty = true;
        notifyListenersOnFilterChange();
    }

    

    private void checkForDirtyFilter()
    {
        if (debugMode)
        {
            Debug.Log("Check if filter needs an update in numerical datamodel.");
        }
        if (filteredRowsIsDirty)
        {
            recalculateFilteredRows();
        }
    }

    private void checkForDirtyMinMax()
    {
        if (debugMode)
        {
            Debug.Log("Check if MinMax needs an update in numerical datamodel.");
        }
        if (minMaxIsDirty)
        {
            recalculateMinMaxValues();
        }
    }

    private void checkForDirtyFilteredNormalization()
    {
        if (debugMode)
        {
            Debug.Log("Check if filtered normalization needs an update in numerical datamodel.");
        }
        if (filteredNormalizationIsDirty)
        {
            recalculateFilteredNormalizedData();
        }
    }

    private void checkForDirtyFilteredGlobalNormalization()
    {
        if (debugMode)
        {
            Debug.Log("Check if filtered global normalization needs an update in numerical datamodel.");
        }
        if (filteredGlobalNormalizationIsDirty)
        {
            recalculateFilteredGlobalyNormalizedData();
        }
    }


    

    /**
     * Forces a recalculation of the filtered rows.
     * This is automatically done if they are needed for another calculation. 
     * However, if you want to implement a "filter now" button use this method.
     * */
    public void forceFilterRecalculation()
    {
        if (debugMode)
        {
            Debug.Log("Forcing a filter recalculation in numerical datamodel.");
        }
        filteredRowsIsDirty = true;
        checkForDirtyFilter();
    }

    public void forceMinMaxRecalculation()
    {
        if (debugMode)
        {
            Debug.Log("Forcing a MinMax recalculation in numerical datamodel.");
        }
        minMaxIsDirty = true;
        checkForDirtyMinMax();
    }

    public void forceFilteredNormalizationRecalculation()
    {
        if (debugMode)
        {
            Debug.Log("Forcing a filtered normalization recalculation in numerical datamodel.");
        }
        filteredNormalizationIsDirty = true;
        checkForDirtyFilteredNormalization();
    }

    public void forceFilteredGlobalNormalizationRecalculation()
    {
        if (debugMode)
        {
            Debug.Log("Forcing a filtered global normalization recalculation in numerical datamodel.");
        }
        filteredGlobalNormalizationIsDirty = true;
        checkForDirtyFilteredGlobalNormalization();
    }

    /**
     * Returns an boolean array containing the information on which data points are currently filtered.
     * If the boolean at index i is true, the data points at index i when using getData(false) is currently disabled by filters.
     * */
    public bool[] getCurrentlyFiltered()
    {
        if (debugMode)
        {
            Debug.Log("Get current filters of a numerical datamodel.");
        }
        checkForDirtyFilter();
        return currentlyFiltered;
    }

    /**
     * Returns a vector2 array containing the current min and max values allowed by the filters in each dimension.
     * The ordering of the dimensions is the same as in getData(bool).
     * */
    public Vector2[] getCurrentFilterValues()
    {
        if (debugMode)
        {
            Debug.Log("Get currently filtered values of a numerical datamodel.");
        }
        return myFilterValues;
    }

    public string[] getheader()
    {
        return myHeaders;
    }
    
}
