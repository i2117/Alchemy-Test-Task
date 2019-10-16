using System;
using System.Collections.Generic;
using System.Linq;

public class Dish
{
    public string name;
    public string ingredientsString;
    public int points;
    public string fullDisplayedInfo;

    private List<IngredientData> ingredients;

    private Dictionary<IngredientType, int> ingredientCountDict =
        new Dictionary<IngredientType, int>();

    // Constructor for loading from prefs
    public Dish(string fullDisplayedInfo, int points)
    {
        this.fullDisplayedInfo = fullDisplayedInfo;
        this.points = points;
    }

    // Constructor for instantiating from pan's content
    public Dish(List<IngredientData> ingredients)
    {
        this.ingredients = ingredients;
        
        // Creating dictionary with ingredients count
        foreach (IngredientType type in Enum.GetValues(typeof(IngredientType)))
        {
            ingredientCountDict[type] = ingredients.Where(el => el.type == type).Count();
        }

        // Creating string of ingredients
        ingredientsString = IngredientString();
        // Creating name 
        name = Name();
        // Counting points
        points = CountPoints();

        fullDisplayedInfo = $"{name} ({ingredientsString}) [{points}]";
    }

    private string IngredientString()
    {
        var _ingredientsString = "";
        foreach (var item in ingredientCountDict.OrderByDescending(k => k.Value))
        {
            if (item.Value == 0)
                break;

            _ingredientsString += item.Value.ToString() + " " + GameData.ingredientDict[item.Key].displayedName + ", ";
        }

        if (_ingredientsString.Last() == ' ')
            _ingredientsString = _ingredientsString.Remove(_ingredientsString.Length - 2);

        return _ingredientsString;
    }

    private string Name()
    {
        var _name = "";

        var meatCount = ingredientCountDict[IngredientType.Meat];
        var onionCount = ingredientCountDict[IngredientType.Onion];
        var potatoCount = ingredientCountDict[IngredientType.Potato];

        if (meatCount == 5)
            _name = "Мясо в собственном соку";
        else if (meatCount == 4)
            _name = "Мясо с гарниром";
        else if (meatCount == 2 || meatCount == 3)
            _name = "Рагу";
        else if (onionCount == 4 || onionCount == 5)
            _name = "Луковый суп";
        else if (potatoCount == 4 || potatoCount == 5)
            _name = "Картофельное пюре";
        else if (meatCount == 0)
            _name = "Овощное рагу";
        else
            _name = "Суп";

        return _name;
    }

    private int CountPoints()
    {
        float sum = 0;
        // If all ingredients are different
        if (ingredientCountDict.Where(el => el.Value > 0).Count() == 5)
            sum = ingredients.Sum(e => e.points) * 2;
        else
        {
            foreach (var usedType in ingredientCountDict)
            {
                // Counting sums for each ingredient type
                sum += CountSumWithCoeff(ingredients.Where(e => e.type == usedType.Key).ToList());
            }
        }

        return (int)sum;
    }

    private float CountSumWithCoeff(List<IngredientData> ingredients)
    {
        float coeff = 1;
        switch (ingredients.Count)
        {
            case 2:
                coeff = GameData.comboData._2similar;
                break;

            case 3:
                coeff = GameData.comboData._3similar;
                break;

            case 4:
                coeff = GameData.comboData._4similar;
                break;

            case 5:
                coeff = GameData.comboData._5similar;
                break;

            default:
                break;
        }

        return ingredients.Sum(e => e.points) * coeff;
    }

    private static List<IngredientData> IngredientListFromIndeces (int[] indecesArray)
    {
        // Mapping indeces to Ingredient Dictionary without Null ingredient type
        var result = indecesArray.ToList().ConvertAll(i => GameData.ingredientDict.Values.ToList()[i + 1]);
        return result;
    }

    public static List<Dish> GenerateAllDifferentDishes()
    {
        // Getting all data about ingredients
        var allIngredientsList = GameData.ingredientDict.Values.ToList();
        allIngredientsList.RemoveAll(el => el.type == IngredientType.Null);

        // Generating all combinations without repeating
        var intCombinations = CombinationsGenerator.GetAllCombinationsWithRepeats(5, allIngredientsList.Count-1);

        // Creating list with all generated dishes
        var unsortedResult = intCombinations.ConvertAll(indArray => new Dish(IngredientListFromIndeces(indArray)));
        // Returning sorted by points list
        return unsortedResult.OrderBy(el => el.points).ToList();
    }
}
