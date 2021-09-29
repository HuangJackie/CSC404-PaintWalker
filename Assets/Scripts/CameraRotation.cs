using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    private Transform Player;
    private Vector3 distFromPlayer;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        distFromPlayer = new Vector3(-0.75f, -4f, 3f);
    }

    void LateUpdate()
    {
        transform.position = Player.position - distFromPlayer;
    }
}
