using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BiscuitDiscoveryUI : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] InputActionAsset action;
    [SerializeField] Image fishSprite;
    [SerializeField] Animation spawnAnim;
    [SerializeField] AnimationClip appearAnimName;
    [SerializeField] AnimationClip disapearingAnimName;
    [SerializeField] List<FishSpecies> listSprites = new List<FishSpecies>();
    bool isDisapearing = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        action.FindAction("Attack").performed += OnInput;
        spawnAnim.AddClip(appearAnimName, appearAnimName.name);
        spawnAnim.AddClip(disapearingAnimName, disapearingAnimName.name);
    }
    private void Update()
    {
        if (!isDisapearing && spawnAnim.IsPlaying(disapearingAnimName.name))
            isDisapearing = true;
        else if (isDisapearing && !spawnAnim.isPlaying)
        {
            CheckEndList();
        }
    }
    public void SetSprite(List<FishSpecies> sprite)
    {
        listSprites = sprite;
    }
    public void Init()
    {
        spawnAnim.Rewind();
        fishSprite.sprite = listSprites[0].fishBiscuit;
        listSprites.RemoveAt(0);
        spawnAnim.Play(appearAnimName.name);
    }
    void CheckEndList()
    {
        isDisapearing = false;
        if (listSprites.Count > 0)
            Init();
        else
            gameObject.SetActive(false);
    }
    void OnInput(InputAction.CallbackContext callbackContext)
    {
        spawnAnim.Play(disapearingAnimName.name);
    }
    private void OnEnable()
    {
        player.SetActive(false);
    }
    private void OnDisable()
    {
        player.SetActive(true);
        
    }
}
