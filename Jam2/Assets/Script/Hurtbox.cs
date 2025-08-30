using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hurtbox : MonoBehaviour
{
    public BoxCollider2D boxCollider;
    public UnityEvent<int> OnHurt;
    [SerializeField] List<SpriteRenderer> spriteRenderers;
    Color baseColor;
    [SerializeField] Color hitColor;
    [SerializeField] float hitColorDuration = 0.1f;
    private void Start()
    {

        baseColor = spriteRenderers[0].color;
    }
    private void Update()
    {
        if (spriteRenderers[0].color != baseColor)
        {
            hitColorDuration -= Time.deltaTime;
            if (hitColorDuration <= 0)
            {
                foreach (var spriteRenderer in spriteRenderers)
                {
                    spriteRenderer.color = baseColor;
                }
                hitColorDuration = 0.1f;
            }
        }
    }
    public void InitBoxSize(Vector4 size)
    {
        boxCollider.offset = new Vector2(size.x, size.y);
        boxCollider.size = new Vector2(size.z, size.w);
    }
    public void Hit(int damage)
    {
        OnHurt.Invoke(damage);
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = hitColor;
        }
    }
    public void ActivateHurtbox()
    {
        boxCollider.enabled = true;
    }
    public void DesactivateHurtbox()
    {
        boxCollider.enabled = false;
    }
}
