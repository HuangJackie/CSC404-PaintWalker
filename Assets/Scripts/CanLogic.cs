using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanLogic : MonoBehaviour
{
    public LevelManager manager;
    public string color;
    public int paintReplenished;

    private ParticleSystem.MainModule particleSettings;
    private Light lightSettings;
    private Renderer[] meshRenderers;
    private UpdateUI _updateUI;

    private void Start()
    {
        particleSettings = GetComponentInChildren<ParticleSystem>().main;
        lightSettings = GetComponentInChildren<Light>();
        meshRenderers = GetComponentsInChildren<Renderer>();
        _updateUI = FindObjectOfType<UpdateUI>();

        Color newColor;
        switch (color)
        {
            case "Yellow":
                newColor = Paints.yellow;
                break;
            case "Blue":
                newColor = Paints.blue;
                break;
            case "Green":
                newColor = Paints.green;
                break;
            case "Red":
                newColor = Paints.red;
                break;
            default:
                newColor = Paints.yellow;
                break;
        }

        particleSettings.startColor = newColor;
        lightSettings.color = newColor;
        foreach (Renderer mesh in meshRenderers)
        {
            mesh.material.color = newColor;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            manager.IncreasePaint(color, paintReplenished);
            Destroy(gameObject);
        }
    }

    private void OnMouseOver()
    {
        if (this)
        {
            _updateUI.SetInfoText("Replenishes: " + paintReplenished + " " + color);
        }
    }

    private void OnMouseExit()
    {
        if (this)
        {
            _updateUI.SetInfoText("");
        }
    }
}