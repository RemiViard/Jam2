using System.Collections;
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
    [SerializeField] AudioSource audioSource;
    [SerializeField] ParticleSystem hurtFX;
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
        if (!audioSource.isPlaying)
            audioSource.Play();
        hurtFX.Play();
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
    public void PauseGameEffect(float duration)
    {
        if (Time.timeScale == 0f)
            return;
        StartCoroutine(pauseGameEffect(duration));
    }
    IEnumerator pauseGameEffect(float duration)
    {
        float timer = 0f;
        Time.timeScale = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = 1f;
    }
}
