using UnityEngine;
using UnityEngine.Events;

public class Furnace : MonoBehaviour
{
    UnityEvent onInteract;

    private void Start()
    {
        onInteract.AddListener(OnInteractEvent);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("trigger");
        if (collision.CompareTag("Player"))
        {
            Debug.Log("player");
            Interact(collision.gameObject);
        }
    }
    private void Interact(GameObject _go)
    {
        Debug.Log("interact");
        onInteract.Invoke();
        // drop les poissons
        int nbBiscuits = _go.GetComponent<Player>().nbBiscuits;

        // gagne des cookies
        for (int i = 0; i < nbBiscuits; i++)
        {
            Debug.Log("biscuit " + i);
        }
    }

    private void OnInteractEvent()
    {

    }
}
