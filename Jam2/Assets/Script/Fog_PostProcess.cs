using UnityEngine;

public class Fog_PostProcess : MonoBehaviour
{
    [SerializeField] GameObject PostProcessUnderwater;

    private void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent<Camera>(out Camera camera))
        {
            RenderSettings.fog = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {

        if (other.TryGetComponent<Camera>(out Camera camera))
        {
            RenderSettings.fog = false;
        }

    }
}