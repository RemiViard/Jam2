using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractableDetector : MonoBehaviour
{
    [SerializeField] InputActionAsset actions;
    [SerializeField] Player player;

    private IInteractable interactableInRange = null;

    private void Start()
    {
        actions.FindAction("Interact").performed += OnInteract;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
            interactableInRange = interactable;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (interactableInRange != null)
            interactableInRange = null;
    }
    void OnInteract(InputAction.CallbackContext callbackContext)
    {
        Debug.Log("wesh wesh");
        if (interactableInRange != null)
            interactableInRange.Interact(player);
    }
}
