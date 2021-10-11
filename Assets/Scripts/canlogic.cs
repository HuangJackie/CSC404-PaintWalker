using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanLogic : MonoBehaviour
{
    public string color;
    public int paintReplenished;
    public LevelManager manager;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            manager.IncreasePaint(color, paintReplenished);
            Destroy(gameObject);
        }
    }
}