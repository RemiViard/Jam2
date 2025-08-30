using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Hitbox : MonoBehaviour
{
    [SerializeField] BoxCollider2D box;
    public UnityEvent<List<Collider2D>> OnTouch = new UnityEvent<List<Collider2D>>();
    List<Collider2D> hits = new List<Collider2D>();
    bool activatedOnce = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }
    IEnumerator SendHitsAfterFixedUpdate()
    {
        yield return new WaitForFixedUpdate();
        if (hits.Count > 0)
            OnTouch.Invoke(hits);
        DeactivateHitBox();
    }
    public void ActivateHitBoxOnce()
    {
        box.enabled = true;
        activatedOnce = true;
        StartCoroutine(SendHitsAfterFixedUpdate());
    }
    public void ActivateHitBox()
    {
        box.enabled = true;
    }
    public void DeactivateHitBox()
    {
        box.enabled = false;
        hits.Clear();
        activatedOnce = false;
    }
    public void InitBoxSize(Vector4 size)
    {
        box.offset = new Vector2(size.x, size.y);
        box.size = new Vector2(size.z, size.w);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(gameObject.tag))
        {
            if (!hits.Contains(other))
                hits.Add(other);
            if (!activatedOnce)
                OnTouch.Invoke(hits);
        }
    }
}
