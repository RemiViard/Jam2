using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hitbox : MonoBehaviour
{
    [SerializeField] BoxCollider2D box;
    [SerializeField] Player attacker;
    List<Collider2D> hits = new List<Collider2D>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }
    IEnumerator SendHits()
    {
        yield return new WaitForFixedUpdate();
        if (hits.Count > 0)
            attacker.OnTouch(hits);
            DeactivateHitBox();
    }
    public void ActivateHitBox()
    {
        box.enabled = true;
        StartCoroutine(SendHits());
    }
    public void DeactivateHitBox()
    {
        box.enabled = false;
        hits.Clear();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(gameObject.tag))
        {
            if (!hits.Contains(other))
                hits.Add(other);
        }
    }
}
