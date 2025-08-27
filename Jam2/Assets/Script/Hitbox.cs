using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hitbox : MonoBehaviour
{
    Collider2D collider2D;
    List<GameObject> touch = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Collider2D>();
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!touch.Contains(collision.gameObject))
        {
            touch.Add(collision.gameObject);
        }
    }

}
