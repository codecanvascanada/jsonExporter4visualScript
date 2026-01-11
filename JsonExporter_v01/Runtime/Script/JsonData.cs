using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;


[Serializable]
[Inspectable]
public class VariableData
{
    public string key;
    public string type;
    public string value;
}


[Serializable]
[Inspectable]
public class ObjectData
{
    public string objectName;
    public List<VariableData> properties = new List<VariableData>();
}


[Serializable]
[Inspectable]
public class JsonData
{
    public List<ObjectData> objects = new List<ObjectData>();
}