using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class SpawnedIngredient : MonoBehaviour
{
    public IngredientData ingredientData;

    public Rigidbody2D _rigidbody2D;

    public Action onColliderEnter, onColliderExit;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Release()
    {
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        onColliderEnter?.Invoke();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        onColliderExit?.Invoke();
    }
}
