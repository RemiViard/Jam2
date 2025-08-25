using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] InputActionAsset actions;
    [SerializeField] SpriteRenderer spriteRenderer;
    [Header("Stats")]
    [SerializeField] int baseMovementSpeed;
    [SerializeField] int jumpForce;

    //Dive variables
    [SerializeField] int dashCooldown;
    [SerializeField] float diveFadeTime;
    Vector2 diveForceInitial = new Vector2(0, 0);
    Vector2 diveForce = new Vector2(0, 0);
    float diveFadeTimer = 0;
    
    Rigidbody2D rb;
    float movementSpeed;
    bool currentDirection = false;
    Vector2 movementInput = new Vector2();
    InputAction moveAction;
    
    
    float dashTimer = 0;

    public int nbBiscuits; // v2 : stocker poissons pour faire un type de biscuit par poisson

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
        movementSpeed = baseMovementSpeed;
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
                spriteRenderer.flipX = currentDirection;
            }
        }
        switch (state)
        {
            case PlayerState.OnLand:
                if (movementInput != Vector2.zero)
                {
                    transform.Translate(new Vector3(movementInput.x, 0, 0) * Time.deltaTime * movementSpeed);
                }
                break;
            case PlayerState.InWater:
                if (movementInput != Vector2.zero)
                {
                    transform.Translate(new Vector3(movementInput.x, movementInput.y, 0) * Time.deltaTime * movementSpeed);
                }
                //Diving Logic
                if (diveFadeTimer > 0)
                {
                    transform.Translate(diveForce * Time.deltaTime);
                    diveForce = Vector2.Lerp(diveForceInitial, Vector2.zero, 1 - diveFadeTimer/diveFadeTime);
                    diveFadeTimer -= Time.deltaTime;
                }
                else
                {
                    diveFadeTimer = 0;
                    diveForce = Vector2.zero;
                    diveForceInitial = Vector2.zero;
                }
                break;
        }
        //Reduce dash Speed and Cooldown
        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                dashTimer = 0;
            }
        }
    }
    #region Input Callbacks
    void OnAttack(InputAction.CallbackContext callbackContext)
    {

    }
    void OnDash(InputAction.CallbackContext callbackContext)
    {
        switch (state)
        {
            case PlayerState.OnLand:
                if (CheckGround())
                {
                    rb.linearVelocityY = jumpForce;
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }

                break;
            case PlayerState.InWater:
                if (dashTimer == 0)
                {
                    //movementSpeed *= 2;
                    rb.linearVelocity = Vector2.zero;
                    dashTimer = dashCooldown;
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
    public void EnterWater()
    {
        state = PlayerState.InWater;
        diveForceInitial = new Vector2(movementInput.x * movementSpeed, rb.linearVelocityY);
        diveForce = diveForceInitial;
        diveFadeTimer = diveFadeTime;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

    }
    public void ExitWater()
    {
        state = PlayerState.OnLand;
        rb.gravityScale = 1f;
    }
}
