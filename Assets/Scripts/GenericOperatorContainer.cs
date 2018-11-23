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
}
