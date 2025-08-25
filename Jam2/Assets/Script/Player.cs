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
    [SerializeField] int dashCooldown;

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
        if(Mathf.Abs(movementInput.x) > 0)
        {
            bool newDirection = movementInput.x < 0;
            if(newDirection != currentDirection)
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
                    rb.totalForce = Vector2.zero;
                    transform.Translate(new Vector3(movementInput.x, movementInput.y, 0) * Time.deltaTime * movementSpeed);
                    
                }
                else
                    if(rb.totalForce == Vector2.zero)
                        rb.AddForceY(1 * Time.deltaTime, ForceMode2D.Force);
                break;
        }
        Debug.Log(rb.totalForce);
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
    void OnAttack(InputAction.CallbackContext callbackContext)
    {

    }
    void OnDash(InputAction.CallbackContext callbackContext)
    {
        switch (state)
        {
            case PlayerState.OnLand:
                if(CheckGround())
                {
                    rb.linearVelocityY = jumpForce;
                    //rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
                    
                break;
            case PlayerState.InWater:
                if (dashTimer == 0)
                {
                    //movementSpeed *= 2;
                    dashTimer = dashCooldown;
                }
                break;
        }
        
    }
    bool CheckGround()
    {
        float _distanceToTheGround = GetComponent<CapsuleCollider2D>().size.y/2;

        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position + Vector3.down * _distanceToTheGround, Vector3.down,  + 0.3f);
        Debug.Log(hit.Length);
        
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
        rb.linearVelocity /= 2f;
        rb.gravityScale = 0f;
    }
    public void ExitWater()
    {
        state = PlayerState.OnLand;
        rb.gravityScale = 1f;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position + Vector3.down * 0.1f, transform.position + Vector3.down * 0.1f + Vector3.down * 0.01f);
    }
}
