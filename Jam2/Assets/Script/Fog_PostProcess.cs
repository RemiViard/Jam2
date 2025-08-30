using UnityEngine;

public class Fog_PostProcess : MonoBehaviour
{
    [SerializeField] Player _player;
    float watersurface = -2.66f;
    float endSurface = -5f;
    Vector2 fogDensityMinMax = new Vector2(0f, 0.021f);
    private void Update()
    {
        if (_player.transform.position.y < watersurface)
        {
            if (!RenderSettings.fog)
                RenderSettings.fog = true;
            float t = Mathf.InverseLerp(watersurface, endSurface, _player.transform.position.y);
            float fogDensity = Mathf.Lerp(fogDensityMinMax.x, fogDensityMinMax.y, t);
            if(RenderSettings.fogDensity != fogDensity)
                RenderSettings.fogDensity = fogDensity;
        }
        else
        {
            if (RenderSettings.fog)
                RenderSettings.fog = false;
        }

    }
}