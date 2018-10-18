using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("GenericOperatorCollection")]
public class GenericOperatorContainer {

	[XmlArray("GenericOperators")]
    [XmlArrayItem("GenericOperator")]
    public List<OperatorData> operators = new List<OperatorData>();

    public bool Contains(OperatorData data)
    {
        foreach(var d in operators)
        {
            if(d.ID == data.ID &&
               d.name == data.name &&
               d.posX == data.posX &&
               d.posY == data.posY &&
               d.posZ == data.posZ &&
               d.parent == data.parent)
            {
                return true;
            }
        }
        return false;
    }
}
