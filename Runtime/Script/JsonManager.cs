using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System; 
using System.Collections;
using Unity.VisualScripting; 
using System.Globalization;

[IncludeInSettings(true)] 
public class JsonManager
{
    public static void SaveToFile(JsonData data)
    {
        if (data == null) 
        {
            Debug.LogError("Save Failed: Data is null.");
            return;
        }

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        File.WriteAllText(path, json);
        Debug.Log("File Saved: " + path);
    }

    public static JsonData LoadFromFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log("File Loaded: " + path);
            return JsonUtility.FromJson<JsonData>(json);
        }
        Debug.LogWarning("File not found, creating new data.");
        return new JsonData(); 
    }

    public static void SaveObjectVariables(JsonData data, GameObject targetObj, object uniqueID)
    {
        if (data == null || targetObj == null || uniqueID == null) 
        {
            Debug.LogError("SaveObject Failed: Missing Data, Target, or ID.");
            return;
        }

        string finalID = uniqueID.ToString();
        data.objects.RemoveAll(x => x.objectName == finalID);

        ObjectData newObj = new ObjectData();
        newObj.objectName = finalID;

        var variableDeclarations = Variables.Object(targetObj);

        foreach (var decl in variableDeclarations)
        {
            string varName = decl.name; 
            object rawValue = decl.value;
            
            VariableData vData = new VariableData();
            vData.key = varName;
            
            if (rawValue != null)
            {
                vData.type = rawValue.GetType().ToString(); 
                
                
                if (rawValue is GameObject)
                {
                    vData.value = ((GameObject)rawValue).name;
                }
                else if (rawValue is Sprite)
                {
                    vData.value = ((Sprite)rawValue).name; 
                }
                else
                {
                    vData.value = rawValue.ToString();
                }
            }
            else
            {
                vData.type = "Null";
                vData.value = "";
            }
            newObj.properties.Add(vData);
        }
        data.objects.Add(newObj);
        Debug.Log("Saved Object Variables: " + finalID);
    }

    public static void LoadVariablesToObject(GameObject targetObj, JsonData data, object uniqueID)
    {
        if (data == null || targetObj == null || uniqueID == null) 
        {
            Debug.LogError("LoadObject Failed: Missing Data, Target, or ID.");
            return;
        }

        string finalID = uniqueID.ToString();

        ObjectData foundObj = null;
        foreach (var obj in data.objects)
        {
            if (obj.objectName == finalID)
            {
                foundObj = obj;
                break;
            }
        }

        if (foundObj == null) 
        {
            Debug.LogWarning("Data not found for ID: " + finalID);
            return;
        }

        foreach (var prop in foundObj.properties)
        {
            object finalValue = ParseValue(prop.value, prop.type);
            Variables.Object(targetObj).Set(prop.key, finalValue);
        }
        Debug.Log("Loaded Object Variables: " + finalID);
    }

    private static object ParseValue(string value, string type)
    {
        try
        {
            if (type == "System.Int32") return int.Parse(value);
            if (type == "System.Single") return float.Parse(value);
            if (type == "System.Boolean") return bool.Parse(value);
            if (type == "System.String") return value;
            if (type == "UnityEngine.Color") return StringToColor(value);
            
            
            if (type == "UnityEngine.Sprite") 
            {
                Sprite sp = Resources.Load<Sprite>(value);
                if (sp == null) Debug.LogError("Sprite not found in Resources: " + value);
                return sp;
            }

            
            if (type == "UnityEngine.GameObject")
            {
                
                GameObject sceneObj = GameObject.Find(value);
                if (sceneObj != null) return sceneObj;

                
                GameObject prefab = Resources.Load<GameObject>(value);
                if (prefab != null) return prefab;

                Debug.LogError("GameObject not found (Scene or Resources): " + value);
                return null;
            }
        }
        catch 
        { 
            Debug.LogError("Parse Error: " + value + " to " + type);
        }
        return value; 
    }

    private static Color StringToColor(string colorString)
    {
        string cleanString = colorString.Replace("RGBA(", "").Replace(")", "");
        string[] parts = cleanString.Split(',');

        if (parts.Length == 4)
        {
            float r = float.Parse(parts[0].Trim(), CultureInfo.InvariantCulture);
            float g = float.Parse(parts[1].Trim(), CultureInfo.InvariantCulture);
            float b = float.Parse(parts[2].Trim(), CultureInfo.InvariantCulture);
            float a = float.Parse(parts[3].Trim(), CultureInfo.InvariantCulture);
            
            return new Color(r, g, b, a);
        }
        return Color.white;
    }
public static void OpenSaveDirectory()
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");

       
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.RevealInFinder(path);
        #else
        Application.OpenURL("file://" + Application.persistentDataPath);
        #endif

        Debug.Log("📂 Open Directory: " + path);
    }
}