using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Furnace : MonoBehaviour, IInteractable
{
    public UnityEvent onInteract;
    public UnityEvent onBiscuitAdded;
    [SerializeField] GameObject prefabBiscuit;

    private void Start()
    {
        onInteract.AddListener(OnInteractEvent);
        onBiscuitAdded.AddListener(OnBiscuitAdded);
    }
    public bool CanInteract()
    {
        return true;
    }
    public void Interact(Player player)
    {
        Debug.Log("interact");
        onInteract.Invoke();
        
        int nbBiscuits = player.nbBiscuits;
        StartCoroutine(WaitAndGainCookie(nbBiscuits));
    }
    IEnumerator WaitAndGainCookie(int nb)
    {
        for (int i = 0; i <= nb; i++)
        {
            onBiscuitAdded?.Invoke();
            yield return new WaitForSeconds(1.0f);
        }
    }
    private void OnInteractEvent()
    {

    }
    private void OnBiscuitAdded()
    {
        GameObject go = Instantiate(prefabBiscuit, this.transform);
        StartCoroutine(TryGoToHUD(go));
    }
    IEnumerator TryGoToHUD(GameObject _go)
    {
        float startTime = Time.time; 

        while (Time.time - startTime <= 10f)
        { 
            _go.transform.position = Vector3.Lerp(_go.transform.position, new Vector3(30,10,5), Time.deltaTime); 
            yield return new WaitForEndOfFrame();
        }
        yield return DestroyCookies(_go);
    }
    IEnumerator DestroyCookies(GameObject _go)
    {
        Destroy(_go);
        yield return new WaitForEndOfFrame();
    }
}
