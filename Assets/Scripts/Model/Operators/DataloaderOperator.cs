using System;
using System.IO;
using Assets.Scripts.Model;
using UnityEngine;

namespace Model.Operators
{
    public class DataloaderOperator : GenericOperator
    {

        private String _path = "/Datasets/";
        private String _filename = "points.csv";
        private bool _hasHeader = false; 
        private string _delimiter = " ";

        public override bool Process()
        {
            var dataModel = ReadCsv();
            SetRawInputData(dataModel);
            SetOutputData(dataModel);

            return true;
        }

        public override bool ValidateIfOperatorPossibleForParents(GenericOperator parent)
        {
            //data loader operator can be spawned even if no datamodel exists yet
            return true;
        }

        private void Awake()
        {
            _path = Application.streamingAssetsPath + _path;
        }

        public override void Start()
        {
            base.Start();
        }

        private GenericDatamodel ReadCsv()
        {
            GenericDatamodel dataModel = new SimpleDatamodel();

            var pathToData = _path + _filename;
            if (File.Exists(pathToData))
            {
                var fileContent = System.IO.File.ReadAllLines(pathToData);

                if (fileContent.Length == 0)
                {
                    throw new FileLoadException("Empty file!");
                }

                var start = 0;
                var attributeTitles = TrimStringArray(fileContent[0].Split(_delimiter.ToCharArray()));
                if (!_hasHeader)
                {
                    for (var i = 0; i < attributeTitles.Length; i++)
                    {
                        attributeTitles[i] = "Column_" + (i+1);
                    }
                }
                else
                {
                    start++;
                }

                var datatypes = new DataAttribute.Valuetype[attributeTitles.Length];
                var firstRow = TrimStringArray(fileContent[start].Split(_delimiter.ToCharArray()));
                for (var i = 0; i<firstRow.Length; i++)
                {
                    datatypes[i] = DataAttribute.GetDataType(firstRow[i]);
                }

                for (var i=start; i < fileContent.Length; i++)
                {
                    var dataItem = new DataItem();
                    var attributes = TrimStringArray(fileContent[i].Split(_delimiter.ToCharArray()));
                    if (attributes.Length != attributeTitles.Length) { throw new FileLoadException("Can not load " + pathToData + ". Row " + i + " does not contain the same amount of columns than the first row(" + attributeTitles.Length + ")."); };

                    for(var j = 0; j<attributes.Length; j++)
                    {
                        var dataAttribute = new DataAttribute();
                        dataAttribute.Init(j,attributeTitles[j], attributes[j], datatypes[j]);
                        dataItem.Add(dataAttribute);
                    }
                    dataModel.Add(dataItem);
                }
           
                if ((_hasHeader && fileContent.Length-1 != dataModel.GetDataItems().Count) || (!_hasHeader && fileContent.Length != dataModel.GetDataItems().Count)) { throw new FileLoadException("Incomplete Parsing! Not all rows were transformed imported as data items!"); };
        
                return dataModel;
            }
            else
            {
                throw new FileLoadException("Did not find file '" + pathToData + "'.");
            }
        }
    
        private string[] TrimStringArray(string[] toTrim)
        {
            for (var i = 0; i < toTrim.Length; i++)
            {
                toTrim[i] = toTrim[i].Trim();
            }
            return toTrim;
        }
    }
}
