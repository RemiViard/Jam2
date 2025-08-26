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
}
