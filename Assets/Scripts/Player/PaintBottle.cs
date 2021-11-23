using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBottle : MonoBehaviour
{
    public Material bottlePaint;

    private ParticleSystem.MainModule particleSettings;
    private Light lightSettings;

    private void Start()
    {
        particleSettings = GetComponentInChildren<ParticleSystem>().main;
        lightSettings = GetComponentInChildren<Light>();
        SetColor(GameConstants.yellow);
    }

    public void SetColor(Color32 color)
    {
        bottlePaint.SetColor("_Color", color);
        particleSettings.startColor = (Color) color;
        lightSettings.color = color;
    }
}
