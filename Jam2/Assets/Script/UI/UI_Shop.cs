using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_Shop : MonoBehaviour
{
    [SerializeField] Player player;
    public UnityEvent onNotBuy;
    public UnityEvent onBuy;
    [SerializeField] List<GameObject> list = new List<GameObject>();
    [SerializeField] Sprite cross;
    private void Start()
    {
        onNotBuy?.AddListener(NotEnoughBiscuits);
        onBuy?.AddListener(OnBuy);
    }
    public void TryToBuy(int i)
    {
        Upgrade _dataUpgrade = list[i].GetComponent<UI_DisplayItem>().upgrade; 

        if (player.nbBiscuits >= _dataUpgrade.cost)
        {
            Upgrade(_dataUpgrade);
            player.nbBiscuits -= _dataUpgrade.cost;
            player.UpdateUI();

            list[i].GetComponent<Button>().enabled = false;
            list[i].GetComponent<Image>().sprite = cross;
            onBuy?.Invoke();
        }
        else
        {
            onNotBuy?.Invoke();
        }
    }
    void Upgrade(Upgrade upgrade)
    {
        upgrade.Action(player);
    }

    void OnBuy()
    {

    }

    void NotEnoughBiscuits()
    {

    }
}
