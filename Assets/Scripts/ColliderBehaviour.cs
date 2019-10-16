using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderBehaviour : MonoBehaviour
{
    public delegate void TriggerMethod(Collider2D collision);
    public event TriggerMethod _OnTriggerEnter2D;

    public delegate void CollisionMethod(Collision2D collision);
    public event CollisionMethod _OnCollisionEnter2D;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _OnCollisionEnter2D?.Invoke(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _OnTriggerEnter2D?.Invoke(collision);
    }
}
