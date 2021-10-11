using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanLogic : MonoBehaviour
{
    public string color;
    public int paintReplenished;
    public LevelManager manager;

    private ParticleSystem.MainModule particleSettings;
    private Light lightSettings;

    private void Start()
    {
        particleSettings = GetComponentInChildren<ParticleSystem>().main;
        lightSettings = GetComponentInChildren<Light>();

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
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            manager.IncreasePaint(color, paintReplenished);
            Destroy(gameObject);
        }
    }
}