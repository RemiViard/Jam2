using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_Shop : MonoBehaviour
{
    [SerializeField] Player player;
    public UnityEvent onNotBuy;
    [SerializeField] List<GameObject> list = new List<GameObject>();

    private void Start()
    {
        onNotBuy?.AddListener(NotEnoughBiscuits);
    }
    public void TryToBuy(int i)
    {
        Upgrade _dataUpgrade = list[i].GetComponent<UI_DisplayItem>().upgrade; 

        if (player.nbBiscuits >= _dataUpgrade.cost)
        {
            OnBuy(_dataUpgrade);
            player.nbBiscuits -= _dataUpgrade.cost;
            list[i].GetComponent<Button>().enabled = false;
        }
        else
        {
            onNotBuy?.Invoke();
        }
    }
    void OnBuy(Upgrade upgrade)
    {
        upgrade.Action(player);
    }

    void NotEnoughBiscuits()
    {

    }
}
