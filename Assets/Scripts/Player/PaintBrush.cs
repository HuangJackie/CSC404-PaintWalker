using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBrush : MonoBehaviour
{
    public Material brushTip;

    private ParticleSystem.MainModule particleSettings;
    private Light lightSettings;

    private void Start()
    {
        particleSettings = GetComponentInChildren<ParticleSystem>().main;
        lightSettings = GetComponentInChildren<Light>();
        SetColor(Paints.yellow);
    }

    public void SetColor(Color32 color)
    {
        brushTip.SetColor("_Color", color);
        particleSettings.startColor = (Color) color;
        lightSettings.color = color;
    }
}
