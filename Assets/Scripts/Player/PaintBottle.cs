using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBottle : MonoBehaviour
{
    public Material bottlePaint;
    private Color32 currentColor;

    private ParticleSystem particleSystemCore;
    private ParticleSystem.MainModule particleSettings;
    private Light lightSettings;

    private void Start()
    {
        lightSettings = GetComponentInChildren<Light>();
        particleSystemCore = GetComponentInChildren<ParticleSystem>();
        particleSettings = particleSystemCore.main;
    }

    private void SwitchColor(Color32 color)
    {
        currentColor = color;
        bottlePaint.SetColor("_Color", color);
        particleSettings.startColor = (Color) color;
        lightSettings.color = color;
    }

    public void SetColor(Color32 color)
    {
        if (!PaintSwitchManager.IsSameColor(currentColor, color))
        {
            SwitchColor(color);
        }
        particleSystemCore.Play();
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
            particleSystemCore.Stop();
            lightSettings.enabled = false;
        }
        else
        {
            particleSystemCore.Play();
            lightSettings.enabled = true;
        }
    }
}
