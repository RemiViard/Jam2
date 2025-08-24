using UnityEngine;

public interface IInteractable
{
    bool CanInteract();
    void Interact(Player player);
}
