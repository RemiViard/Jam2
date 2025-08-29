using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Furnace : MonoBehaviour, IInteractable
{
    public UnityEvent onInteract;
    [SerializeField] GameObject prefabBiscuit;

    [SerializeField] Transform HUDPos;
    [SerializeField] Transform TargetPos;

    [Header("Audio")]
    AudioSource audioSource;
    [SerializeField] AudioClip open;
    [SerializeField] AudioClip close;

    public UnityEvent onCanInteract;
    public UnityEvent onStopInteract;
    public static Furnace instance;
    public List<FishSpecies> waitingFishs = new List<FishSpecies>();
    public void OnDeadFish(FishSpecies fishSpecies)
    {
        waitingFishs.Add(fishSpecies);
    }
    public void ResetWaitingFishes()
    {
        waitingFishs.Clear();
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        onInteract.AddListener(OnInteractEvent);
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
        StartCoroutine(WaitAndGainCookie(player));
    }
    IEnumerator WaitAndGainCookie(Player player)
    {

        foreach (FishSpecies fish in waitingFishs)
        {
            //Sell
            OnBiscuitAdded(fish);

            player.nbBiscuits += fish.value;
            player.UpdateUI();

            yield return new WaitForSeconds(1.0f);
        }
        waitingFishs.Clear();
    }

    private void OnInteractEvent()
    {
    }
    private void OnCanInteract()
    {
        audioSource.clip = open;
        audioSource.Play();
    }
    private void OnStopInteract()
    {
        audioSource.clip = close;
        audioSource.Play();
    }
    private void OnBiscuitAdded(FishSpecies fish)
    {
        prefabBiscuit.GetComponent<Image>().sprite = fish.fishBiscuit;
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
