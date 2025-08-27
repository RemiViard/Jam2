using UnityEngine;

[CreateAssetMenu(fileName = "FishSpecies", menuName = "Scriptable Objects/FishSpecies")]
public class FishSpecies : ScriptableObject
{
    public enum FishBehavior
    {
        Fleeing,
        Aggressive,
    }
    public FishBehavior behavior;
    public string speciesName;
    public Sprite fishSprite;
    public int Hp;
    public float speed;
    public Vector4 hurtBox; // x, y, width, height
}
// This is a ScriptableObject that can be used to define different fish species in the game.
