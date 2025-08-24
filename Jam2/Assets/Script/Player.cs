using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{
    [SerializeField] InputActionAsset actions;
    [SerializeField] int baseMovementSpeed;
    float movementSpeed;
    Vector2 movementInput = new Vector2();
    InputAction moveAction;
    [SerializeField] int dashCooldown;
    float dashTimer = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        if(movementInput != Vector2.zero)
        {
            transform.Translate(new Vector3(movementInput.x, movementInput.y, 0) * Time.deltaTime * movementSpeed);
        }
        //Reduce dash Speed and Cooldown
        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
            if(dashTimer <= 0)
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
        if(dashTimer == 0)
        {
            //movementSpeed *= 2;
            dashTimer = dashCooldown;
        }
    }

}
