using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DisplayItem : MonoBehaviour
{
    public Upgrade upgrade;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descText;
    [SerializeField] TextMeshProUGUI costText;

    private void Start()
    {
        if (upgrade != null)
        {
            nameText.text = upgrade.m_name;
            descText.text = upgrade.m_description;
            costText.text = upgrade.cost.ToString();
        }
    }

}
