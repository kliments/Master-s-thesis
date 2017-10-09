using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NumericDatamodel : MonoBehaviour {

    public string nameOfDatafile;
    public bool hasheader;
    public string delimiter;

    private float[][] myData;
    private string[] myHeaders;
    private Vector2[] myFilterValues;               // Do not directly read this. Use getCurrentFilterValues() instead. May be inconsistent otherwise.
    private bool[] currentlyFiltered;               // Do not directly read this. Use getCurrentlyFiltered() instead. May be inconsistent otherwise.
    private Vector2[] filteredMinMaxValues;         // Do not directly read this. Use getMinMaxValues(bool ignoreFilter) instead. May be inconsistent otherwise.
    private bool minMaxIsDirty = true;
    private bool filteredRowsIsDirty = true;
    private Vector2[] unfilteredMinMaxValues;       // Do not directly read this. Use getMinMaxValues(bool ignoreFilter) instead. May be inconsistent otherwise.
    private int amountOfRows;
    private int amountOfCols;
    private int amountOfUnfilteredDatapoints;
    private List<IDatamodelListener> myListeners = new List<IDatamodelListener>();

    // Use this for initialization
    void Start () {
        if (nameOfDatafile.EndsWith(".csv"))
        {
            myData = DatasetImporter.readNumericCSV(nameOfDatafile, hasheader, delimiter);
            amountOfRows = myData.Length;
            amountOfCols = myData[0].Length;
            myHeaders = DatasetImporter.getHeader(nameOfDatafile, delimiter);
            initFilter();
            test();
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

    /**
     * Adds an Object that implements IDatamodelListener as a listener to this datamodel.
     * It will receive an update containing this model whenever there is a biger change in the model.
     * This likely means that the view should be redrawn.
     * */
    public void addListener(IDatamodelListener listener)
    {
        myListeners.Add(listener);
    }

    /**
     * Removes an object from the list of listeners. Opposite of addListener(IDatamodelListener listener).
     * */
    public void removeListener(IDatamodelListener listener)
    {
        myListeners.Remove(listener);
    }

    /**
     * Sends a signal to each listener that the filters changed.
     * */
    private void notifyListenersOnFilterChange()
    {
        foreach(IDatamodelListener listener in myListeners)
        {
            listener.datamodelFilterChange(this);
        }
    }

    /**
     * Sends a signal to each listener that the min/max values changed.
     * */
    private void notifyListenersOnMinMaxChange()
    {
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
        float[][] returnvalue = myData;
        if (!ignoreFilters) {
            int amountOfUnfilteredDP = getNumberOfUnfilteredDatapoints();
            bool[] filterArray = getCurrentlyFiltered();
            returnvalue = new float[amountOfUnfilteredDP][];
            int currentIndex = 0;
            for(int i = 0; i < amountOfUnfilteredDP; i++)
            {
                if (!filterArray[i])
                {
                    returnvalue[currentIndex] = myData[i];
                    currentIndex++;
                }
            }
        };
        return returnvalue;
    }

    /**
     * Returns the number of datapoints that are not disabled by filters.
     * */ 
    public int getNumberOfUnfilteredDatapoints()
    {
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
        myFilterValues[dimension] = new Vector2(min, max);
        filteredRowsIsDirty = true;
        minMaxIsDirty = true;
        if (instantlyRecalculateFilteredRows) { checkForDirtyFilter(); };
    }

    private void recalculateFilteredRows()
    {
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
        notifyListenersOnFilterChange();
    }

    

    private void checkForDirtyFilter()
    {
        if (filteredRowsIsDirty)
        {
            recalculateFilteredRows();
        }
    }

    private void checkForDirtyMinMax()
    {
        if (minMaxIsDirty)
        {
            recalculateMinMaxValues();
        }
    }

    /**
     * Forces a recalculation of the filtered rows.
     * This is automatically done if they are needed for another calculation. 
     * However, if you want to implement a "filter now" button use this method.
     * */
    public void forceFilterRecalculation()
    {
        filteredRowsIsDirty = true;
        checkForDirtyFilter();
    }

    public void forceMinMaxRecalculation()
    {
        minMaxIsDirty = true;
        checkForDirtyMinMax();
    }

    /**
     * Returns an boolean array containing the information on which data points are currently filtered.
     * If the boolean at index i is true, the data points at index i when using getData(false) is currently disabled by filters.
     * */
    public bool[] getCurrentlyFiltered()
    {
        checkForDirtyFilter();
        return currentlyFiltered;
    }

    /**
     * Returns a vector2 array containing the current min and max values allowed by the filters in each dimension.
     * The ordering of the dimensions is the same as in getData(bool).
     * */
    public Vector2[] getCurrentFilterValues()
    {
        return myFilterValues;
    }
}
