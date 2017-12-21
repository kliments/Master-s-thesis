using System;
using System.IO;
using Assets.Scripts.Model;
using UnityEngine;

namespace Model.Operators
{
    public class DataloaderOperator : GenericOperator
    {

        private String path = "/Datasets/";
        private String filename = "points.csv";
        private bool hasHeader = false; 
        private string delimiter = " ";

        public override bool process()
        {
            GenericDatamodel dataModel = readCSV();
            setRawInputData(dataModel);
            setOutputData(dataModel);

            return true;
        }

        public override bool validateIfOperatorPossibleForParents(GenericOperator parent)
        {
            //data loader operator can be spawned even if no datamodel exists yet
            return true;
        }

        void Awake()
        {
            path = Application.streamingAssetsPath + path;
        }

        public override void Start()
        {
            base.Start();
        }

        private GenericDatamodel readCSV()
        {
            GenericDatamodel dataModel = new SimpleDatamodel();

            string pathToData = path + filename;
            if (File.Exists(pathToData))
            {
                string[] fileContent = System.IO.File.ReadAllLines(pathToData);

                if (fileContent.Length == 0)
                {
                    throw new FileLoadException("Empty file!");
                }

                int start = 0;
                string[] attributeTitles = trimStringArray(fileContent[0].Split(delimiter.ToCharArray()));
                if (!hasHeader)
                {
                    for (int i = 0; i < attributeTitles.Length; i++)
                    {
                        attributeTitles[i] = "Column_" + (i+1);
                    }
                }
                else
                {
                    start++;
                }

                DataAttribute.valuetype[] datatypes = new DataAttribute.valuetype[attributeTitles.Length];
                string[] firstRow = trimStringArray(fileContent[start].Split(delimiter.ToCharArray()));
                for (int i = 0; i<firstRow.Length; i++)
                {
                    datatypes[i] = DataAttribute.getDataType(firstRow[i]);
                }

                for (int i=start; i < fileContent.Length; i++)
                {
                    DataItem dataItem = new DataItem();
                    string[] attributes = trimStringArray(fileContent[i].Split(delimiter.ToCharArray()));
                    if (attributes.Length != attributeTitles.Length) { throw new FileLoadException("Can not load " + pathToData + ". Row " + i + " does not contain the same amount of columns than the first row(" + attributeTitles.Length + ")."); };

                    for(int j = 0; j<attributes.Length; j++)
                    {
                        DataAttribute dataAttribute = new DataAttribute();
                        dataAttribute.init(j,attributeTitles[j], attributes[j], datatypes[j]);
                        dataItem.add(dataAttribute);
                    }
                    dataModel.add(dataItem);
                }
           
                if ((hasHeader && fileContent.Length-1 != dataModel.getDataItems().Count) || (!hasHeader && fileContent.Length != dataModel.getDataItems().Count)) { throw new FileLoadException("Incomplete Parsing! Not all rows were transformed imported as data items!"); };
        
                return dataModel;
            }
            else
            {
                throw new FileLoadException("Did not find file '" + pathToData + "'.");
            }
        }
    
        private string[] trimStringArray(string[] toTrim)
        {
            for (int i = 0; i < toTrim.Length; i++)
            {
                toTrim[i] = toTrim[i].Trim();
            }
            return toTrim;
        }
    }
}
