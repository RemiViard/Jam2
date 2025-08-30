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
    float detectionRange;
    [SerializeField] Hurtbox hurtbox;
    [SerializeField] Hitbox hitbox;
    public UnityEvent<FishSpecies> onDeath = new UnityEvent<FishSpecies>();
    Vector3 deathPos = Vector3.zero;
    float deathTimer = 0f;
    [SerializeField] float rotDuration;
    BehaviorState state = BehaviorState.Neutral;
    Vector3 currentDir = Vector3.zero;
    float zigzagTimer = 0f;
    float attackTimer = 0f;
    float chargeTime = 1f;
    [HideInInspector] public BoxCollider2D deptZone;
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
        hitbox.InitBoxSize(species.hurtBox);
        onDeath.AddListener(Furnace.instance.OnDeadFish);
        detectionRange = species.detectionRange;
        if (species.scale != 0)
            spriteRenderer.transform.localScale = Vector3.one * species.scale;
    }
    void Update()
    {
        if (isAlive)
        {
            switch (state)
            {
                case BehaviorState.Fleeing:
                    Move(species.speed);
                    float distanceToPlayer = Vector3.Distance(transform.position, Player.playerTransform.position);
                    if (distanceToPlayer >= detectionRange * 3)
                    {
                        state = BehaviorState.Neutral;
                        zigzagTimer = 0f;
                    }
                    else if (distanceToPlayer <= detectionRange * 0.5f && zigzagTimer <= 0)
                    {
                        Vector2 dir = currentDir;
                        currentDir = (transform.position - Player.playerTransform.position).normalized;
                        if (CheckBound())
                            currentDir = dir;
                        else
                        {
                            transform.LookAt(transform.position + currentDir);
                            zigzagTimer = 1f;
                        }
                    }
                    if (zigzagTimer > 0)
                    {
                        zigzagTimer -= Time.deltaTime;
                        if (zigzagTimer <= 0)
                            zigzagTimer = 0;
                    }

                    break;
                case BehaviorState.Neutral:
                    switch (species.behavior)
                    {
                        case FishBehavior.Fleeing:
                            if (Vector3.Distance(transform.position, Player.playerTransform.position) <= detectionRange)
                            {
                                ChangeDirection(transform.position - Player.playerTransform.position);
                                state = BehaviorState.Fleeing;
                            }
                            else
                            {
                                if (currentDir != Vector3.zero)
                                    transform.LookAt(transform.position + currentDir);
                                else
                                    ChangeDirection();
                                Move(species.speed / 2);
                            }   
                            break;
                        case FishBehavior.Neutral:
                            if (currentDir != Vector3.zero)
                                transform.LookAt(transform.position + currentDir);
                            else
                                ChangeDirection();
                            Move(species.speed / 2);
                            break;
                        case FishBehavior.Aggressive:
                            if (currentDir != Vector3.zero)
                                transform.LookAt(transform.position + currentDir);
                            else
                                ChangeDirection();
                            Move(species.speed / 2);
                            if (Vector3.Distance(transform.position, Player.playerTransform.position) <= detectionRange)
                            {
                                state = BehaviorState.Attacking;
                                attackTimer = 1f;
                            }

                            break;
                        default:
                            break;
                    }
                    break;
                case BehaviorState.Attacking:
                    ChangeDirection((Player.playerTransform.position - transform.position).normalized);
                    if (Vector3.Distance(transform.position, Player.playerTransform.position) >= detectionRange)
                    {
                        Move(species.speed);
                        if (Vector3.Distance(transform.position, Player.playerTransform.position) >= detectionRange * 3f)
                            state = BehaviorState.Neutral;
                    }
                    if (Vector3.Distance(transform.position, Player.playerTransform.position) <= detectionRange *1.5f)
                        attackTimer -= Time.deltaTime;
                    if (attackTimer <= 0)
                    {
                        state = BehaviorState.Charging;
                        hitbox.ActivateHitBox();
                    }
                    break;
                case BehaviorState.Charging:
                    attackTimer += Time.deltaTime;
                    if(CheckBound(2))
                        Move(species.speed * 2);
                    else
                        attackTimer = chargeTime;
                    if (attackTimer >= chargeTime)
                    {
                        state = BehaviorState.Attacking;
                        hitbox.DeactivateHitBox();
                        attackTimer = chargeTime;
                    }
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
        currentDir = Random.insideUnitCircle.normalized;
        transform.LookAt(transform.position + currentDir);
    }
    private void ChangeDirection(Vector2 dir)
    {
        currentDir = dir.normalized;
        transform.LookAt(transform.position + currentDir);
    }
    void Move(float speed)
    {
        if (!CheckBound(speed))
        {
            currentDir = -currentDir;
            transform.LookAt(transform.position + currentDir);
        }
        transform.position += currentDir * speed * Time.deltaTime;
    }
    bool CheckBound(float speed = 1f)
    {
        if (currentDir == null)
            ChangeDirection();
        return deptZone.bounds.Contains(transform.position + currentDir * speed* Time.deltaTime);
    }

    public void OnHurt(int damage)
    {
        Hp -= damage;
        hurtbox.PauseGameEffect(0.1f);
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
        foreach (var hit in hits)
        {
            if(hit != null)
            {
                if (hit.TryGetComponent(out Hurtbox hb) && hit.tag == "PlayerHurtbox")
                {
                    hb.Hit(species.damage);
                }
            }
        }
    }
}
