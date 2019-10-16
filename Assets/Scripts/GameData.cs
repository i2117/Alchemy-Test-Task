using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;
    public static bool shouldLoadFromSave = true;

    // Ingredients data from JSON file
    public static Dictionary<IngredientType, IngredientData> ingredientDict =
        new Dictionary<IngredientType, IngredientData>();

    // Combo data from JSON
    public static ComboData comboData;

    // Sprites for UI
    public static Dictionary<IngredientType, Sprite> ingredientSpritesDict =
        new Dictionary<IngredientType, Sprite>();

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Reading JSONs
        var ingredientDataJSONs = JsonHelper.FromJson<IngredientDataJSON>(StringFromFile("ingredients"));

        comboData = JsonUtility.FromJson<ComboData>(StringFromFile("combos"));

        // Filling dictionaries
        foreach (var element in ingredientDataJSONs)
        {
            var ingredientData = new IngredientData(element);
            ingredientDict[ingredientData.type] = ingredientData;

            var sprite = Resources.Load<Sprite>
                ($"IngredientSprites/Ingredient{ingredientData.type.ToString()}") as Sprite;
            ingredientSpritesDict[ingredientData.type] = sprite;
        }
    }

    private string StringFromFile(string filename)
    {
        TextAsset file = Resources.Load<TextAsset>(filename) as TextAsset;
        return file.ToString();
    }

    public static void WriteToFile(List<string> content)
    {
        var path = "Assets/Resources/combinations.txt";
        path = Application.streamingAssetsPath + "/combinations.txt";

        StreamWriter writer = new StreamWriter(path, false);

        foreach (var line in content)
        {
            writer.WriteLine(line);
        }
        
        writer.Close();
        Debug.Log($"{content.Count} lines have been written to {path}");
    }
}

#region Ingredient structures

public enum IngredientType { Null, Potato, Carrot, Pepper, Onion, Meat};

[Serializable]
public class IngredientDataJSON
{
    public string type;
    public string displayedName;
    public int points;
}

public class IngredientData : IngredientDataJSON
{
    public new IngredientType type;

    public IngredientData (IngredientDataJSON ingredientDataJSON)
    {
        type = JsonHelper.EnumFromString<IngredientType>(ingredientDataJSON.type);
        displayedName = ingredientDataJSON.displayedName;
        points = ingredientDataJSON.points;
    }
}

[Serializable]
public struct ComboData
{
    public float _allDifferent;
    public float _2similar;
    public float _3similar;
    public float _4similar;
    public float _5similar;
}

#endregion
