using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class Pan : MonoBehaviour
{
    public ColliderBehaviour triggerBehaviour;
    public ColliderBehaviour collisionBehaviour;

    private List<IngredientData> usedIngredients = new List<IngredientData>();

    private const int requiredIngredientCount = 5;

    // Events
    public static event Action<int, IngredientType> onCurrentChanged;
    public static event Action<Dish> onCooked;

    private void Awake()
    {
        // When an ingredient enters the trigger zone
        triggerBehaviour._OnTriggerEnter2D += (Collider2D collision) =>
        {
            if (collision.tag != "spawnedIngredient")
                return;

            var ingredientData = collision.GetComponent<SpawnedIngredient>().ingredientData;
            onCurrentChanged?.Invoke(usedIngredients.Count, ingredientData.type);
            usedIngredients.Add(ingredientData);

            Destroy(collision.gameObject);

            // When the pan contains all 5 ingredients
            if (usedIngredients.Count >= requiredIngredientCount)
            {
                transform.DOShakePosition(0.5F, 0.2F);
                onCooked?.Invoke(new Dish(usedIngredients));
                usedIngredients.Clear();
            }
        };

        // When an ingredient collides with the pan
        collisionBehaviour._OnCollisionEnter2D += (Collision2D collision) =>
        {
            // Maybe will be implemented later
        };
    }
}
