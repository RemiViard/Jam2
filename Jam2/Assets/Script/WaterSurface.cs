using UnityEngine;

public class WaterSurface : MonoBehaviour
{
    [SerializeField] Player player;
    float offsetx;
    private void Start()
    {
        offsetx = transform.position.x - player.transform.position.x;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x + offsetx, transform.position.y, transform.position.z);
    }
}
