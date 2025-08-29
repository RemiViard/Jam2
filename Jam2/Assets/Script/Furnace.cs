using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Furnace : MonoBehaviour, IInteractable
{
    public UnityEvent onInteract;
    public UnityEvent onBiscuitAdded;
    [SerializeField] GameObject prefabBiscuit;

    [SerializeField] Transform HUDPos;
    [SerializeField] Transform TargetPos;

    public UnityEvent onCanInteract;
    public UnityEvent onStopInteract;

    public List<Fish> _fishesToSell = new List<Fish>();

    private void Start()
    {
        onInteract.AddListener(OnInteractEvent);
        onBiscuitAdded.AddListener(OnBiscuitAdded);
        onCanInteract.AddListener(OnCanInteract);
        onStopInteract.AddListener(OnStopInteract);
    }
    public bool CanInteract()
    {
        return true;
    }
    public void Interact(Player player)
    {
        onInteract.Invoke();

        int nbBiscuits = player.nbBiscuits;

        foreach (Fish fish in _fishesToSell)
        {
            //Sell
            StartCoroutine(WaitAndGainCookie(fish)); // change : sell only one biscuit,give fishspecies 
        }

        // empty stockage
    }
    IEnumerator WaitAndGainCookie(Fish fish)
    {
        onBiscuitAdded?.Invoke();
        yield return new WaitForSeconds(1.0f);
    }
    private void OnInteractEvent()
    {
    }
    private void OnCanInteract()
    {

    }
    private void OnStopInteract()
    {

    }
    private void OnBiscuitAdded()
    {
        GameObject go = Instantiate(prefabBiscuit, HUDPos);
        go.transform.localPosition = Vector3.zero;
        StartCoroutine(TryGoToHUD(go));
    }
    IEnumerator TryGoToHUD(GameObject _biscuit)
    {
        float startTime = Time.time;

        Vector3 basePos = Camera.main.WorldToScreenPoint(transform.position);
        _biscuit.GetComponent<RectTransform>().position = basePos;

        float t = 0;

        while (t <= 2f)
        {
            _biscuit.transform.position = Vector3.Lerp(basePos, TargetPos.position, t);
            t += Time.deltaTime;

            yield return 0;
        }
        _biscuit.GetComponent<Animator>().SetTrigger("Destroy");
        Destroy(_biscuit);
    }
}
