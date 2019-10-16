using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    
    public Image[] currentIngredients;
    public Text scoreText;
    public Text lastDishText;
    public Text bestDishText;

    private Dish lastDish;
    private Dish LastDish
    {
        get
        {
            return lastDish;
        }
        set
        {
            lastDish = value;
            lastDishText.text = "Последнее блюдо: " + value.fullDisplayedInfo;
        }
    }

    private Dish bestDish;
    private Dish BestDish
    {
        get
        {
            return bestDish;
        }
        set
        {
            bestDish = value;
            bestDishText.text = "Лучшее блюдо: " + value.fullDisplayedInfo;
        }
    }

    private int totalScore;
    private int TotalScore
    {
        get
        {
            return totalScore;
        }
        set
        {
            totalScore = value;
            scoreText.text = $"Счёт: {value.ToString()}";
        }
    }

    private void OnEnable()
    {
        Pan.onCurrentChanged += SetPanIngredient;
        Pan.onCooked += AddDish;
    }

    private void OnDisable()
    {
        Pan.onCurrentChanged -= SetPanIngredient;
        Pan.onCooked -= AddDish;
    }

    private void Awake()
    {
        // Loading from save if it wasn't after restart
        if (GameData.shouldLoadFromSave)
            LoadGame();
        else
            TotalScore = 0;

        GameData.shouldLoadFromSave = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown("r"))
            Restart();

        if (Input.GetKeyDown("l"))
            LoadGame();

        if (Input.GetKeyDown("t"))
        {
            var stringList = Dish.GenerateAllDifferentDishes().ConvertAll(el => el.fullDisplayedInfo);
            GameData.WriteToFile(stringList);
        } 
    }

    private void AddDish(Dish dish)
    {
        TotalScore += dish.points;
        LastDish = dish;
        
        if (BestDish == null || LastDish.points > BestDish.points)
            // Replacing best dish by new dish
            BestDish = lastDish;

        SaveGame();
        ClearPanStatus();

        Debug.Log($"Added {dish.points} points");
    }

    private void ClearPanStatus()
    {
        for (int i = 0; i < currentIngredients.Length; i++)
        {
            SetPanIngredient(i, IngredientType.Null);
        }
    }

    private void SetPanIngredients(List<IngredientData> ingredientDatas)
    {
        for (int i = 0; i < currentIngredients.Length; i++)
        {
            var type = ingredientDatas[i] != null ?
                ingredientDatas[i].type :
                IngredientType.Null;

            SetPanIngredient(i, type);
        }
    }

    private void SetPanIngredient(int position, IngredientType type)
    {
        if (currentIngredients.Length < position - 1)
            return;

        currentIngredients[position].sprite = GameData.ingredientSpritesDict[type];
    }

    #region Saving and loading

    public void Restart()
    {
        GameData.shouldLoadFromSave = false;
        SceneManager.LoadScene(0);
    }

    private void SaveGame()
    {
        PlayerPrefs.SetInt("totalScore", totalScore);

        PlayerPrefs.SetString("lastDishInfo", LastDish.fullDisplayedInfo);
        PlayerPrefs.SetInt("lastDishPoints", LastDish.points);

        PlayerPrefs.SetString("bestDishInfo", BestDish.fullDisplayedInfo);
        PlayerPrefs.SetInt("bestDishPoints", BestDish.points);
    }

    private void LoadGame()
    {
        TotalScore = PlayerPrefs.GetInt("totalScore", 0);

        LastDish = new Dish(
            PlayerPrefs.GetString("lastDishInfo", "Пока ещё ничего не приготовлено :("),
            PlayerPrefs.GetInt("lastDishPoints", 0));

        BestDish = new Dish(
            PlayerPrefs.GetString("bestDishInfo", "пока ничего"),
            PlayerPrefs.GetInt("bestDishPoints", 0));
    }

    #endregion

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/ResetPrefs")]
    public static void ResetPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
#endif
}
