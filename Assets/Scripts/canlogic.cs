using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canlogic : MonoBehaviour
{
    public string color;
    public LevelManager manager;

    void OnTriggerEnter(Collider other)
    {
        manager.IncreasePaint(color, 10);
        Destroy(gameObject);
    }
}