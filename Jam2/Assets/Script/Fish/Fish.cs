using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Fish : MonoBehaviour, IHurtable, ICanHit
{
    public FishSpecies species;

    public float MinTimeBeforeChangingDirection = 3f;
    public float MaxTimeBeforeChangingDirection = 6f;
    [HideInInspector] public bool isAlive;
    
    int Hp;
    private Rigidbody2D rb;
    private float m_currentTimer;
    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField] Hurtbox hurtbox;
    public UnityEvent onDeath = new UnityEvent();
    Vector3 deathPos = Vector3.zero;
    float deathTimer = 0f;
    [SerializeField] float rotDuration;

    void Start()
    {
        isAlive = true;
        rb = GetComponent<Rigidbody2D>();
        m_currentTimer = Random.Range(MinTimeBeforeChangingDirection, MaxTimeBeforeChangingDirection);
        // Binding Events
    }
    public void Spawn()
    {
        Hp = species.Hp;
        spriteRenderer.sprite = species.fishSprite;
        gameObject.name = species.speciesName;
        hurtbox.InitBoxSize(species.hurtBox);
        if (species.scale != 0)
            transform.localScale = Vector3.one * species.scale;
    }
    void Update()
    {
        if (isAlive)
        {
            rb.linearVelocity = transform.forward * species.speed;
            m_currentTimer -= Time.deltaTime;
            if (m_currentTimer <= 0)
            {
                ChangeDirection();
                m_currentTimer = Random.Range(MinTimeBeforeChangingDirection, MaxTimeBeforeChangingDirection);
            }
        }
        else
        {
            deathTimer += Time.deltaTime;
            rb.linearVelocity = Vector2.zero;
            transform.position = deathPos + Vector3.up * Mathf.Sin(deathTimer)*0.4f;
            if (deathTimer >= rotDuration)
                Destroy(gameObject);
        }
    }
    private void ChangeDirection()
    {
        Vector2 rot = Random.insideUnitCircle.normalized;
        transform.LookAt(transform.position + new Vector3(rot.x, rot.y, 0));
    }
    public void OnHurt(int damage)
    {
        Hp -= damage;
        if (Hp <= 0)
        {
            onDeath.Invoke();
            deathPos = transform.position;
            transform.rotation = Quaternion.Euler(180, 90, 0);
            hurtbox.DesactivateHurtbox();
            isAlive = false;
        }
    }
    // Events
    public void OnTouch(List<Collider2D> hits)
    {
        throw new System.NotImplementedException();
    }
}
