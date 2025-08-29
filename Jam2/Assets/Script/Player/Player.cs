using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour, IHurtable, ICanHit
{
    [Header("Reference")]
    [SerializeField] InputActionAsset actions;
    [SerializeField] Animator animator;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Hitbox attackHitBox;
    [SerializeField] Transform pivot;
    [SerializeField] WaterZone waterZone;
    [SerializeField] CapsuleCollider2D capsuleCollider;
    [SerializeField] Camera mainCamera;

    [Header("Stats")]
    public int movementSpeed;
    [SerializeField] int jumpForce;

    [Header("Dash")]
    [SerializeField] int dashCooldown;
    [SerializeField] int dashSpeed;
    [SerializeField] float dashDuration;
    Vector2 dashForceInitial = new Vector2(0, 0);
    Vector2 dashForce = new Vector2(0, 0);
    float dashCooldownTimer = 0;
    float dashReleaseTimer = 0;

    [Header("Punch")]
    public int punchDamage = 1;
    [SerializeField] float punchCooldown = 1f;
    float PunchCooldownTimer = 0;

    [Header("Dive")]
    //Dive variables
    [SerializeField] float diveFadeTime;
    Vector2 diveForceInitial = new Vector2(0, 0);
    Vector2 diveForce = new Vector2(0, 0);
    float diveFadeTimer = 0;

    Rigidbody2D rb;
    public bool isFlip = false;
    Vector3 lastDirection = Vector3.zero;
    Vector2 movementInput = new Vector2();
    InputAction moveAction;
    float baseGravityScale;

    public int maxO2 = 10;
    float O2;
    public int nbBiscuits; // v2 : stocker poissons pour faire un type de biscuit par poisson
    public UnityEvent<float> OnO2ValueChange = new UnityEvent<float>(); // v2 : event pour la barre d'oxygene
    public UnityEvent OnDeath = new UnityEvent(); // v2 :event pour la mort du joueur
    bool OnLand = true;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI TextCountBiscuits;

    PlayerState state = PlayerState.OnLand;
    enum PlayerState
    {
        OnLand,
        InWater,
    }
    public static Transform playerTransform = null;// static reference to the player transform for easy access(FishBehavior)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveAction = actions.FindAction("Move");
        actions.FindAction("Attack").performed += OnAttack;
        actions.FindAction("Dash").performed += OnDash;
        actions.Enable();
        nbBiscuits = 0;
        UpdateUI();
        O2 = maxO2;
        OnO2ValueChange.Invoke(O2 / maxO2);
        baseGravityScale = rb.gravityScale;
        if (playerTransform == null)
            playerTransform = transform;
    }

    public void UpdateUI()
    {
        TextCountBiscuits.text = nbBiscuits.ToString();
    }

    // Update is called once per frame
    void Update()
    {

        //Movement
        movementInput = moveAction.ReadValue<Vector2>();
        switch (state)
        {
            case PlayerState.OnLand:
                if (movementInput.x != 0)
                {
                    transform.Translate(new Vector3(movementInput.x, 0, 0) * Time.deltaTime * movementSpeed);
                    if (!animator.GetBool("isWalking"))
                        animator.SetBool("isWalking", true);
                    if (movementInput.x < 0 != isFlip)
                    {
                        isFlip = !isFlip;
                        pivot.localRotation = Quaternion.Euler(0, isFlip ? -90 : 90, 0);
                    }

                }
                else
                {
                    if (animator.GetBool("isWalking"))
                        animator.SetBool("isWalking", false);
                }
                OnLand = CheckGround();
                animator.SetBool("isGrounded", OnLand);
                if (OnLand && rb.linearVelocityX != 0)
                    rb.linearVelocityX = 0;


                break;
            case PlayerState.InWater:
                //Swimming Logic
                if (movementInput != Vector2.zero && dashReleaseTimer == 0)
                {
                    if (movementInput.y > 0 && transform.position.y >= waterZone.waterTopY - 0.1f)
                    {
                        movementInput.y = 0;
                    }
                    transform.Translate(new Vector3(movementInput.x, movementInput.y, 0) * Time.deltaTime * movementSpeed);
                    //Rotate Logic
                    if (!animator.GetBool("OnAction"))
                    {
                        if (movementInput.x != 0)
                        {
                            pivot.transform.LookAt(pivot.position + new Vector3(movementInput.x, movementInput.y));
                            pivot.transform.Rotate(Vector3.right * 90);
                            if (movementInput.x < 0 != isFlip)
                                isFlip = !isFlip;
                        }
                        else
                        {
                            int rotationy = isFlip ? -90 : 90;
                            if (movementInput.y < 0)
                                pivot.transform.localRotation = Quaternion.Euler(180, rotationy, 0);
                            else
                                pivot.transform.localRotation = Quaternion.Euler(0, rotationy, 0);
                        }
                    }
                }
                //Dash Logic
                else if (dashReleaseTimer > 0)
                {
                    transform.Translate(dashForce * Time.deltaTime * dashSpeed);
                    dashForce = Vector2.Lerp(dashForceInitial, Vector2.zero, 1 - dashReleaseTimer / dashDuration);
                    dashReleaseTimer -= Time.deltaTime;
                    if (dashReleaseTimer <= 0)
                    {
                        dashReleaseTimer = 0;
                        dashForce = Vector2.zero;
                        dashForceInitial = Vector2.zero;
                    }
                }
                //Diving Logic
                if (diveFadeTimer > 0)
                {
                    transform.Translate(diveForce * Time.deltaTime);
                    diveForce = Vector2.Lerp(diveForceInitial, Vector2.zero, 1 - diveFadeTimer / diveFadeTime);
                    diveFadeTimer -= Time.deltaTime;
                }
                else
                {
                    diveFadeTimer = 0;
                    diveForce = Vector2.zero;
                    diveForceInitial = Vector2.zero;
                }
                //O2 Logics
                O2Change(O2 - Time.deltaTime);
                break;
        }
        //Dash Cooldown
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0)
            {
                dashCooldownTimer = 0;
            }
        }
        //Punch Cooldown
        if (PunchCooldownTimer > 0)
        {
            PunchCooldownTimer -= Time.deltaTime;
            if (PunchCooldownTimer <= 0)
            {
                PunchCooldownTimer = 0;
            }
        }
    }
    #region Input Callbacks
    void OnAttack(InputAction.CallbackContext callbackContext)
    {
        if (PunchCooldownTimer <= 0 && (state == PlayerState.InWater))
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));
            if(mouseWorldPos.x == pivot.position.x)
            {
                mouseWorldPos.x = pivot.position.x+0.01f;
            }
            pivot.LookAt(mouseWorldPos, Vector3.up);
            animator.SetTrigger("Punch");
            animator.SetBool("OnAction", true);
            PunchCooldownTimer = punchCooldown;
        }
    }

    void OnDash(InputAction.CallbackContext callbackContext)
    {
        switch (state)
        {
            case PlayerState.OnLand:
                if (CheckGround())
                {

                    rb.linearVelocityY = 0;
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
                break;
            case PlayerState.InWater:
                if (dashCooldownTimer == 0)
                {
                    dashCooldownTimer = dashCooldown;
                    dashReleaseTimer = dashDuration;
                    Vector2 direction = movementInput != Vector2.zero ? movementInput : new Vector2(isFlip ? -1 : 1, 0);
                    dashForceInitial = direction * dashSpeed;
                }
                break;
        }
    }
    #endregion
    bool CheckGround()
    {
        float _distanceToTheGround = capsuleCollider.size.y / 3;

        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position + Vector3.down * _distanceToTheGround, Vector3.down, +0.3f);

        foreach (RaycastHit2D h in hit)
        {
            if (h.collider.gameObject.CompareTag("Ground"))
            {
                return true;
            }
        }
        return false;

    }
    public void Attack()
    {
        attackHitBox.ActivateHitBox();
    }
    public void OnTouch(List<Collider2D> hits)
    {
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<Hurtbox>(out Hurtbox hurtbox))
            {
                hurtbox.Hit(punchDamage);
            }
        }
    }
    public void EndAction()
    {
        animator.SetBool("OnAction", false);
    }
    #region WaterZone
    public void EnterWater()
    {
        state = PlayerState.InWater;
        diveForceInitial = new Vector2(movementInput.x * movementSpeed, rb.linearVelocityY);
        diveForce = diveForceInitial;
        diveFadeTimer = diveFadeTime;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        animator.SetBool("isSwimming", true);
    }
    public void ExitWater()
    {
        state = PlayerState.OnLand;
        rb.gravityScale = baseGravityScale;
        //isDashing
        if (dashReleaseTimer > 0)
        {
            rb.linearVelocity = new Vector2(movementInput.x, movementInput.y) * movementSpeed;
            rb.linearVelocity += dashForce;
            dashReleaseTimer = 0;
            pivot.localRotation = Quaternion.Euler(0, isFlip ? -90 : 90, 0);
        }
        animator.SetBool("isSwimming", false);
        O2Change(maxO2);
    }
    #endregion
    private void O2Change(float value)
    {
        O2 = value;
        OnO2ValueChange.Invoke(O2 / maxO2);
        if (O2 <= 0)
        {
            O2 = 0;
            Die();
        }
        else if (O2 > maxO2)
        {
            O2 = maxO2;
        }
    }
    private void Die()
    {
        OnDeath.Invoke();
        transform.position = spawnPoint.position;
        O2Change(maxO2);
    }

    public void OnHurt(int damage)
    {
        O2Change(O2 - damage);
    }
}
