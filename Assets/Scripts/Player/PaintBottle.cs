using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBottle : MonoBehaviour
{
    public Material bottlePaint;
    private Color32 currentColor;

    private ParticleSystem coreParticleSystem;
    private ParticleSystem switchParticleSystem;
    private ParticleSystem.MainModule coreParticleSettings;
    private ParticleSystem.MainModule switchParticleSettings;
    private Light lightSettings;

    private void Start()
    {
        lightSettings = GetComponentInChildren<Light>();
        coreParticleSystem = GetComponentsInChildren<ParticleSystem>()[0];
        coreParticleSettings = coreParticleSystem.main;
        switchParticleSystem = GetComponentsInChildren<ParticleSystem>()[1];
        switchParticleSettings = switchParticleSystem.main;
    }

    private void SwitchColor(Color32 color)
    {
        currentColor = color;
        bottlePaint.SetColor("_Color", color);

        coreParticleSettings.startColor = (Color) color;
        switchParticleSettings.startColor = (Color) color;
        lightSettings.color = color;
        PlayColorSwitchEffect();
    }

    public void PlayColorSwitchEffect()
    {
        switchParticleSystem.Play();
    }

    public void SetColor(Color32 color)
    {
        if (!PaintSwitchManager.IsSameColor(currentColor, color))
        {
            SwitchColor(color);
        }
        coreParticleSystem.Play();
        lightSettings.enabled = true;
    }

    // Change lighting to the provided `color` and toggle
    // lighting visibility based on paintQuantity == 0.
    public void SetColor(Color32 color, int paintQuantity)
    {
        if (!PaintSwitchManager.IsSameColor(currentColor, color))
        {
            SwitchColor(color);
        }
        
        if (paintQuantity == 0)
        {
            coreParticleSystem.Stop();
            lightSettings.enabled = false;
        }
        else
        {
            coreParticleSystem.Play();
            lightSettings.enabled = true;
        }
    }
}
