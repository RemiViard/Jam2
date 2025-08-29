using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Upgrade")]
public class Upgrade : ScriptableObject
{
    public enum eStat
    {
        O2,
        Damages,
        SwimSpeed
    }

    public eStat stat;
    public int cost;
    public int value;
    public string m_name;
    public string m_description;

    public void Action(Player player)
    {
        switch (stat)
        {
            case eStat.O2:
                player.maxO2 += value;
                break;
            case eStat.Damages:
                player.punchDamage += value;
                break;
            case eStat.SwimSpeed:
                player.movementSpeed += value;
                break;
        }
    }
}
