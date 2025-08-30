using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InteractInRange : MonoBehaviour
{
    [SerializeField] List<Sprite> sprites = new List<Sprite>();
    [SerializeField] SpriteRenderer spriteRenderer;
    bool isActive = false;
    float timer = 0f;
    int currentSpriteIndex = 0;
    Vector3 offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        offset = transform.position;
    }
    public void Activate(Vector3 worldPos)
    {
        isActive = true;
        spriteRenderer.enabled = true;
        transform.position = worldPos + offset;
    }
    public void Deactivate()
    {
        isActive = false;
        spriteRenderer.enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            timer += Time.deltaTime;
            if (timer >= 0.2f)
            {
                timer = 0f;
                currentSpriteIndex = (currentSpriteIndex + 1) % sprites.Count;
                spriteRenderer.sprite = sprites[currentSpriteIndex];
            }
        }
    }
}
