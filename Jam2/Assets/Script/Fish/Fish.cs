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
    [SerializeField] float detectionRange;
    [SerializeField] Hurtbox hurtbox;
    public UnityEvent<FishSpecies> onDeath = new UnityEvent<FishSpecies>();
    Vector3 deathPos = Vector3.zero;
    float deathTimer = 0f;
    [SerializeField] float rotDuration;
    BehaviorState state = BehaviorState.Neutral;
    Vector3 currentDir = Vector3.zero;
    public enum FishBehavior
    {
        Fleeing,
        Neutral,
        Aggressive,
    }
    public enum BehaviorState
    {
        Fleeing,
        Neutral,
        Attacking,
        Charging,
    }
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
        onDeath.AddListener(Furnace.instance.OnDeadFish);
        if (species.scale != 0)
            transform.localScale = Vector3.one * species.scale;
    }
    void Update()
    {
        if (isAlive)
        {
            switch (state)
            {
                case BehaviorState.Fleeing:
                    Vector3 dir = (transform.position - Player.playerTransform.position).normalized;
                    transform.LookAt(transform.position + dir);
                    transform.position += dir * species.speed * Time.deltaTime;
                    if (Vector3.Distance(transform.position, Player.playerTransform.position) >= detectionRange)
                    {
                        state = BehaviorState.Neutral;
                    }
                        break;
                case BehaviorState.Neutral:
                    switch (species.behavior)
                    {
                        case FishBehavior.Fleeing:
                            if(Vector3.Distance(transform.position, Player.playerTransform.position) <= detectionRange)
                            {
                                state = BehaviorState.Fleeing;
                            }
                            break;
                        case FishBehavior.Neutral:
                            rb.linearVelocity = transform.forward * species.speed / 2 * Time.deltaTime;
                            break;
                        case FishBehavior.Aggressive:
                            if (Vector3.Distance(transform.position, Player.playerTransform.position) <= detectionRange)
                                state = BehaviorState.Attacking;
                            break;
                        default:
                            break;
                    }
                    break;
                case BehaviorState.Attacking:
                    
                default:
                    break;
            }
        }
        else
        {
            deathTimer += Time.deltaTime;
            rb.linearVelocity = Vector2.zero;
            transform.position = deathPos + Vector3.up * Mathf.Sin(deathTimer) * 0.4f;
            if (deathTimer >= rotDuration)
            {
                onDeath.RemoveAllListeners();
                Destroy(gameObject);
            }

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
        if (species.behavior == FishBehavior.Neutral)
            state = BehaviorState.Fleeing;
        if (Hp <= 0)
        {
            onDeath.Invoke(species);
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
