using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canlogic : MonoBehaviour
{
    public string color;
    public int paintReplenished;
    public LevelManager manager;

    void OnTriggerEnter(Collider other)
    {
        manager.IncreasePaint(color, paintReplenished);
        Destroy(gameObject);
    }
}