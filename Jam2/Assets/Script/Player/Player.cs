using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour, IHurtable, ICanHit
{
    [Header("Reference")]
    [SerializeField] InputActionAsset actions;
    [SerializeField] Animator animator;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Hitbox attackHitBox;
    [SerializeField] Hurtbox hurtbox;
    [SerializeField] Transform pivot;
    [SerializeField] WaterZone waterZone;
    [SerializeField] CapsuleCollider2D capsuleCollider;
    [SerializeField] Camera mainCamera;
    [SerializeField] Transform deepTrans;
    [SerializeField] Transform verydeepTrans;
    [SerializeField] GameObject shopUI;
    [SerializeField] GameObject menuPauseUI;
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
    Quaternion pivotBaseRot;
    Vector3 lastDirection = Vector3.zero;
    Vector2 movementInput = new Vector2();
    InputAction moveAction;
    float baseGravityScale;

    public int maxO2 = 10;
    int baseMaxO2;
    float O2;
    public int nbBiscuits; // v2 : stocker poissons pour faire un type de biscuit par poisson
    public UnityEvent<float> OnO2ValueChange = new UnityEvent<float>(); // v2 : event pour la barre d'oxygene
    public UnityEvent OnDeath = new UnityEvent(); // v2 :event pour la mort du joueur
    bool OnLand = true;
    bool isActive = true;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI TextCountBiscuits;

    [Header("Audio Setup")]
    AudioSource audiosource;

    [Header("Sounds")]
    [SerializeField] AudioClip SplashSound;
    [SerializeField] AudioClip SwimSound;
    [SerializeField] AudioClip PunchSound;
    public float minimumSwimTime = 0.8f;
    public float maximumSwimTime = 1.2f;
    private float SwimTimer = 0;

    [Header("Fx")]
    [SerializeField] ParticleSystem dashFX;
    [SerializeField] ParticleSystem punchFX;
    [SerializeField] ParticleSystem splashFX;
    [Header("DeepMultiplicators")]
    [SerializeField] float deepMultiplicator = 5f;
    [SerializeField] float veryDeepMultiplicator = 10f;

    enum DepthLevel
    {
        Mid,
        Deep,
        VeryDeep
    }
    DepthLevel currentDepth = DepthLevel.Mid;
    PlayerState state = PlayerState.OnLand;
    enum PlayerState
    {
        OnLand,
        InWater,
    }
    public static Transform playerTransform = null;// static reference to the player transform for easy access(FishBehavior)

    [SerializeField] List<Upgrade> O2Upgrade = new List<Upgrade>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        moveAction = actions.FindAction("Move");
        actions.FindAction("Attack").performed += OnAttack;
        actions.FindAction("Dash").performed += OnDash;
        actions.FindAction("Escape").performed += OnEscape;
        actions.Enable();
        nbBiscuits = 0;
        UpdateUI();
        O2 = maxO2;
        baseMaxO2 = maxO2;
        OnO2ValueChange.Invoke(O2 / maxO2);
        baseGravityScale = rb.gravityScale;
        pivotBaseRot = pivot.localRotation;
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
        if (!isActive)
            movementInput = Vector2.zero;
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
                Vector2 translate = movementInput;
                if (movementInput != Vector2.zero && dashReleaseTimer == 0)
                {
                    if (movementInput.y > 0 && transform.position.y >= waterZone.waterTopY - 0.1f)
                    {
                        translate.y = 0;
                    }
                    SwimTimer += Time.deltaTime;
                    if (SwimTimer >= Random.Range(minimumSwimTime, maximumSwimTime))
                    {

                        SwimTimer = 0;
                    }
                    transform.Translate(new Vector3(translate.x, translate.y, 0) * Time.deltaTime * movementSpeed);
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

                if (transform.position.y < deepTrans.position.y)
                {
                    if (transform.position.y < verydeepTrans.position.y)
                    {
                        if (currentDepth != DepthLevel.VeryDeep)
                            ChangeDept(DepthLevel.VeryDeep);
                    }
                    else
                    {
                        if (currentDepth != DepthLevel.Deep)
                            ChangeDept(DepthLevel.Deep);
                    }
                }
                else
                    if (currentDepth != DepthLevel.Mid)
                    ChangeDept(DepthLevel.Mid);
                int deptValue = CheckDept();
                float multiplicator = 1;
                switch (currentDepth)
                {
                    case DepthLevel.Deep:
                        if (deptValue >= 2)
                            multiplicator = 1;
                        else if (deptValue >= 1)
                            multiplicator = 3;
                        else
                            multiplicator = deepMultiplicator;
                        break;
                    case DepthLevel.VeryDeep:
                        if (deptValue == 3)
                            multiplicator = 1;
                        else if (deptValue >= 2)
                            multiplicator = 5;
                        else
                            multiplicator = veryDeepMultiplicator;
                        break;
                }
                O2Change(O2 - Time.deltaTime * multiplicator);
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
        if (!isActive) return;
        if (PunchCooldownTimer <= 0 && (state == PlayerState.InWater))
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));
            if (mouseWorldPos.x == pivot.position.x)
            {
                mouseWorldPos.x = pivot.position.x + 0.01f;
            }
            pivot.LookAt(mouseWorldPos, Vector3.up);
            animator.SetTrigger("Punch");
            animator.SetBool("OnAction", true);
            PunchCooldownTimer = punchCooldown;
            punchFX.Play();
            audiosource.clip = PunchSound;
            audiosource.Play();
        }
    }

    void OnDash(InputAction.CallbackContext callbackContext)
    {
        if (!isActive) return;
        switch (state)
        {
            case PlayerState.OnLand:
                if (CheckGround())
                {
                    if (isActive)
                    {
                        rb.linearVelocityY = 0;
                        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    }
                }
                break;
            case PlayerState.InWater:
                if (dashCooldownTimer == 0)
                {
                    dashCooldownTimer = dashCooldown;
                    dashReleaseTimer = dashDuration;
                    Vector2 direction = movementInput != Vector2.zero ? movementInput : new Vector2(isFlip ? -1 : 1, 0);
                    dashForceInitial = direction * dashSpeed;
                    audiosource.clip = SwimSound;
                    audiosource.Play();
                    dashFX.Play();
                }
                break;
        }
    }
    void OnEscape(InputAction.CallbackContext callbackContext)
    {
        if(isActive)
        {
            if (shopUI.activeSelf)
                shopUI.SetActive(false);
            else
                menuPauseUI.SetActive(!menuPauseUI.activeSelf);
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
    int CheckDept()
    {
        int test = baseMaxO2;
        for (int i = 0; i <= 3; i++)
        {
            if (test == maxO2)
                return i;
            test += O2Upgrade[i].value;
        }
        return -1;
    }
    public void Attack()
    {
        attackHitBox.ActivateHitBoxOnce();
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
        SwimTimer = 0;
        state = PlayerState.InWater;
        diveForceInitial = new Vector2(movementInput.x * movementSpeed, rb.linearVelocityY);
        diveForce = diveForceInitial;
        diveFadeTimer = diveFadeTime;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        animator.SetBool("isSwimming", true);
        audiosource.clip = SplashSound;
        audiosource.Play();
        splashFX.Play();
    }
    public void ExitWater()
    {
        SwimTimer = 0;
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
        currentDepth = DepthLevel.Mid;
        animator.SetBool("isSwimming", false);
        if (isActive)
            splashFX.Play();
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
    private void ChangeDept(DepthLevel depthLevel)
    {
        switch (depthLevel)
        {
            case DepthLevel.Mid:

                break;
            case DepthLevel.Deep:

                break;
            case DepthLevel.VeryDeep:

                break;
        }
        currentDepth = depthLevel;
    }
    private void Die()
    { 
        isActive = false;
        waterZone.enabled = false;
        StartCoroutine(WaitRespawn());
    }
    IEnumerator WaitRespawn()
    {
        yield return new WaitForSeconds(0.1f);
        OnDeath.Invoke();
        
        hurtbox.DesactivateHurtbox();
        transform.position = spawnPoint.position;
        pivot.localRotation = pivotBaseRot;
        O2Change(maxO2);
        hurtbox.ActivateHurtbox();
        splashFX.Stop();
        isActive = true;
        waterZone.enabled = true;
    }
    public void OnHurt(int damage)
    {
        O2Change(O2 - damage);
        hurtbox.PauseGameEffect(0.1f);
    }
    public void SetActive(bool value)
    {
        isActive = value;
    }
}
