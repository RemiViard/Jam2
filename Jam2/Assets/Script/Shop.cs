using UnityEngine;
using UnityEngine.Events;

public class Shop : MonoBehaviour, IInteractable
{
    public UnityEvent onInteract;

    private void Start()
    {
        onInteract.AddListener(OnInteractEvent);
    }
    public bool CanInteract()
    {
        return true;
    }

    public void Interact(Player player)
    {
        onInteract.Invoke();
    }
    private void OnInteractEvent()
    {
    }
}
