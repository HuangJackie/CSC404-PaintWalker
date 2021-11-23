using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controls the lighting system for the Player Paint brush.
 * In order to ensure correct behavior, the first light component
 * in its attached GameObject's children hierarchy must be of the
 * Paint Brush tip and not the Glowing blue orb.
 */
public class PaintBrush : MonoBehaviour
{
    public Material brushTip;
    private Color32 currentColor;

    private ParticleSystem particleSystemCore;
    private ParticleSystem.MainModule particleSettings;
    private Light lightSettings;

    private void Start()
    {
        lightSettings = GetComponentsInChildren<Light>()[0];
        particleSystemCore = GetComponentInChildren<ParticleSystem>();
        particleSettings = particleSystemCore.main;
    }

    private void SwitchColor(Color32 color)
    {
        currentColor = color;
        brushTip.SetColor("_Color", color);
        
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
