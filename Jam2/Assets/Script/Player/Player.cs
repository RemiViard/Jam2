using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] InputActionAsset actions;
    [SerializeField] Animator animator;
    [SerializeField] Transform spawnPoint;
    [SerializeField] BoxCollider2D attackHitBox;
    [SerializeField] Transform pivot;

    [Header("Stats")]
    [SerializeField] int movementSpeed;
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
    [SerializeField] int punchDamage = 1;
    [SerializeField] float punchCooldown = 1f;
    float PunchCooldownTimer = 0;

    [Header("Dive")]
    //Dive variables
    [SerializeField] float diveFadeTime;
    Vector2 diveForceInitial = new Vector2(0, 0);
    Vector2 diveForce = new Vector2(0, 0);
    float diveFadeTimer = 0;
   
    Rigidbody2D rb;
    bool currentDirection = false;
    Vector2 movementInput = new Vector2();
    InputAction moveAction;
    float baseGravityScale;

    [SerializeField] int maxO2 = 10;
    float O2;
    public int nbBiscuits; // v2 : stocker poissons pour faire un type de biscuit par poisson
    public UnityEvent<float> OnO2ValueChange = new UnityEvent<float>(); // v2 : event pour la barre d'oxygene
    public UnityEvent OnDeath = new UnityEvent(); // v2 :event pour la mort du joueur

    PlayerState state = PlayerState.OnLand;
    enum PlayerState
    {
        OnLand,
        InWater,
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveAction = actions.FindAction("Move");
        actions.FindAction("Attack").performed += OnAttack;
        actions.FindAction("Dash").performed += OnDash;
        actions.Enable();
        O2 = maxO2;
        OnO2ValueChange.Invoke(O2 / maxO2);
        baseGravityScale = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {

        //Movement
        movementInput = moveAction.ReadValue<Vector2>();

        if (Mathf.Abs(movementInput.x) > 0)
        {
            bool newDirection = movementInput.x < 0;
            if (newDirection != currentDirection)
            {
                currentDirection = newDirection;
                pivot.localRotation = Quaternion.Euler(0, currentDirection ? -90 : 90, 0);
            }
        }
        switch (state)
        {
            case PlayerState.OnLand:
                if (movementInput != Vector2.zero)
                {
                    transform.Translate(new Vector3(movementInput.x, 0, 0) * Time.deltaTime * movementSpeed);
                    if (!animator.GetBool("isWalking"))
                        animator.SetBool("isWalking", true);
                }
                else
                {
                    if (animator.GetBool("isWalking"))
                        animator.SetBool("isWalking", false);
                }
                break;
            case PlayerState.InWater:
                //Swimming Logic
                if (movementInput != Vector2.zero && dashReleaseTimer == 0)
                {
                    transform.Translate(new Vector3(movementInput.x, movementInput.y, 0) * Time.deltaTime * movementSpeed);
                    if (!animator.GetBool("isSwimming"))
                        animator.SetBool("isSwimming", true);
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
                else
                {
                    if (animator.GetBool("isSwimming"))
                        animator.SetBool("isSwimming", false);
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
        if (PunchCooldownTimer <= 0)
        {
            animator.SetTrigger("Punch");
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
                    animator.SetTrigger("Jump");
                }
                break;
            case PlayerState.InWater:
                if (dashCooldownTimer == 0)
                {
                    dashCooldownTimer = dashCooldown;
                    dashReleaseTimer = dashDuration;
                    Vector2 direction = movementInput != Vector2.zero ? movementInput : new Vector2(currentDirection ? -1 : 1, 0);
                    dashForceInitial = direction * dashSpeed;
                }
                break;
        }
    }
    #endregion
    bool CheckGround()
    {
        float _distanceToTheGround = GetComponent<CapsuleCollider2D>().size.y / 2;

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
    void ActivateHitbox(bool value)
    {
        attackHitBox.enabled = value;

    }
    #region WaterZone
    public void EnterWater()
    {
        state = PlayerState.InWater;
        diveForceInitial = new Vector2(movementInput.x * movementSpeed, rb.linearVelocityY);
        diveForce = diveForceInitial;
        diveFadeTimer = diveFadeTime;
        Debug.Log(rb.linearVelocity);
        rb.linearVelocity = Vector2.zero;
        
        rb.gravityScale = 0f;
    }
    public void ExitWater()
    {
        state = PlayerState.OnLand;
        rb.gravityScale = baseGravityScale;
        
        O2Change(maxO2);
    }
    #endregion
    private void O2Change(float value)
    {
        O2 = value;
        OnO2ValueChange.Invoke(O2 / maxO2);
        if(O2 <= 0)
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
}
