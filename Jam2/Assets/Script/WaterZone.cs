using UnityEngine;

public class WaterZone : MonoBehaviour
{
    [HideInInspector] public float waterTopY; // Y coordinate of the water surface
    private void Start()
    {
        waterTopY = transform.position.y + (GetComponent<BoxCollider2D>().size.y * transform.localScale.y) / 2;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.EnterWater();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.ExitWater();
            }
        }
    }

}
