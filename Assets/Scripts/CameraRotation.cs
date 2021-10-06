using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float speed;
    public LevelManager LevelManager;
    
    private Transform Player;
    private Vector3 distFromPlayer;
    private Vector3 _initialClickPosition;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        distFromPlayer = new Vector3(-0.75f, -4f, 3f);
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _initialClickPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            LevelManager.SetIsPanning(true);
            Vector3 distanceMoved = Input.mousePosition - _initialClickPosition;
            transform.position += new Vector3(-distanceMoved.x * speed, 0, -distanceMoved.y * speed);
            _initialClickPosition = Input.mousePosition;
            return;
        }

        if (!LevelManager.IsPanning())
        {
            transform.position = Player.position - distFromPlayer;
        }
    }
}
