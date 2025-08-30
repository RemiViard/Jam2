using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Fog_PostProcess : MonoBehaviour
{
    [SerializeField] Player _player;
    [SerializeField] Terrain terrain;
    [SerializeField] List<Material> sandMat = new List<Material>();
    float watersurface = -2.66f;
    float endSurface = -5f;
    Vector2 fogDensityMinMax = new Vector2(0f, 0.021f);
    private void Start()
    {
        terrain.materialTemplate = sandMat[1];
        //sandMaterial.SetPropertyLock("CausticsIsActive", true);
    }
    private void Update()
    {
        if (_player.transform.position.y < watersurface)
        {
            if(terrain.materialTemplate != sandMat[1])
                terrain.materialTemplate = sandMat[1];
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
            if (terrain.materialTemplate != sandMat[0])
                terrain.materialTemplate = sandMat[0];
        }

    }
}