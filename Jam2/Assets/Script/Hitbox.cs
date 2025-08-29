using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hitbox : MonoBehaviour
{
    BoxCollider2D box;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        box = GetComponent<BoxCollider2D>();
    }
    public RaycastHit2D[] ActivateHitBox()
    {
        Debug.Log("Hitbox Activated");
        return Physics2D.BoxCastAll(box.bounds.center, box.bounds.size, 0f, Vector2.zero, 800f, gameObject.layer);

    }
}
