using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Fish : MonoBehaviour, IHurtable, ICanHit
{
    public FishSpecies species;

    public float MinTimeBeforeChangingDirection = 3f;
    public float MaxTimeBeforeChangingDirection = 6f;
    public bool isAlive;

    int Hp;
    private Rigidbody2D m_Rigidbody;
    private float m_currentTimer;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Hurtbox hurtbox;
    public UnityEvent onDeath = new UnityEvent();

    void Start()
    {
        isAlive = true;
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_currentTimer = Random.Range(MinTimeBeforeChangingDirection, MaxTimeBeforeChangingDirection);
        // Binding Events
        onDeath.AddListener(OnDeath);
    }
    public void Spawn()
    {
        Hp = species.Hp;
        spriteRenderer.sprite = species.fishSprite;
        gameObject.name = species.speciesName;
        hurtbox.InitBoxSize(species.hurtBox);
    }
    void Update()
    {
        
        if (isAlive)
        {
            Move();
            m_currentTimer -= Time.deltaTime;
            if (m_currentTimer <= 0)
            {
                ChangeDirection();
                m_currentTimer = Random.Range(MinTimeBeforeChangingDirection, MaxTimeBeforeChangingDirection);
            }
        }

    }
    private void ChangeDirection()
    {
        Vector2 rot = Random.insideUnitCircle.normalized;
        transform.LookAt(transform.position + new Vector3(rot.x, rot.y, 0));
    }

    private void Move()
    {
        m_Rigidbody.linearVelocity = transform.forward * species.speed;
    }
    public void OnHurt(int damage)
    {
        Hp -= damage;
        if (Hp <= 0)
        {
            onDeath.Invoke();
            isAlive = false;
        }
    }
    // Events
    private void OnDeath()
    {
        // VFX OU SFX de la mort qui tue (litteralement)
    }

    public void OnTouch(List<Collider2D> hits)
    {
        throw new System.NotImplementedException();
    }
}
