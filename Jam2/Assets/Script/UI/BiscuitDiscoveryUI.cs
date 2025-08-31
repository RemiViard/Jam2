using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BiscuitDiscoveryUI : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] InputActionAsset action;
    Animation anim;
    bool isDisapearing = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        action.FindAction("Escape").performed += OnInput;
        action.FindAction("Interact").performed += OnInput;
    }
    private void Update()
    {
        if (anim.IsPlaying("Disapear") && !isDisapearing)
        {
            isDisapearing = true;
        }
        if(isDisapearing && !anim.isPlaying)
        {
            isDisapearing = false;
            gameObject.SetActive(false);
        }
    }
    void OnInput(InputAction.CallbackContext callbackContext)
    {
        anim.Play("Disapear");
    }
    private void OnEnable()
    {
        player.SetActive(false);
        anim.Play("Idle");
    }
    private void OnDisable()
    {
        player.SetActive(true);
    }
}
