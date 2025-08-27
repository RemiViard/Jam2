using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    [SerializeField] BoxCollider2D boxCollider;
    public void InitBoxSize(Vector4 size)
    {
        boxCollider.offset = new Vector2(size.x, size.y);
        boxCollider.size = new Vector2(size.z, size.w);
    }

}
