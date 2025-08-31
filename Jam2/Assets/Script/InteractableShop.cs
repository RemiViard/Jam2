using UnityEngine;
using UnityEngine.Events;

public class InteractableShop : MonoBehaviour, IInteractable
{
    public UnityEvent onInteract;
    public GameObject shopUI;
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
        shopUI.SetActive(!shopUI.activeSelf);
    }
    private void OnInteractEvent()
    {
    }
}
