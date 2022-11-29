using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Catalog
{
    public int current;
    public string item_id;
}

public class JsonCatalog : MonoBehaviour
{
    public void Start()
    {
        Catalog myCatalog = new Catalog();
        myCatalog.current = 1;
        myCatalog.item_id = "wooden_sword";

        string jsonCatalog = JsonUtility.ToJson(myCatalog);
        Debug.Log(jsonCatalog);

        File.ReadAllText("Assets/title-1B50D-FirstCatalog.json");
        File.WriteAllText("Assets/MyCatalog", jsonCatalog);

        myCatalog = JsonUtility.FromJson<Catalog>(jsonCatalog);
        Debug.Log(myCatalog);

        Debug.Log(File.ReadAllText("Assets/title-1B50D-FirstCatalog.json"));
    }
}
