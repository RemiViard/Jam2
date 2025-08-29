using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_Shop : MonoBehaviour
{
    [SerializeField] Player player;
    public UnityEvent onBuy;
    public UnityEvent onNotBuy;
    [SerializeField] List<GameObject> list = new List<GameObject>();

    private void Start()
    {
        onBuy?.AddListener(OnBuy);
        onNotBuy?.AddListener(NotEnoughBiscuits);
    }
    public void TryToBuy(int i)
    {
        Upgrade _dataUpgrade = list[i].GetComponent<UI_DisplayItem>().upgrade; 

        if (player.nbBiscuits >= _dataUpgrade.cost)
        {
            onBuy?.Invoke();
            player.nbBiscuits -= _dataUpgrade.cost;
            list[i].GetComponent<Button>().enabled = false;
        }
        else
        {
            onNotBuy?.Invoke();
        }
    }
    void OnBuy()
    {
        Debug.Log("buy");
    }

    void NotEnoughBiscuits()
    {
        Debug.Log("not buy");
    }
}
