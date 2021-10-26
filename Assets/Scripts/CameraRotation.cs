using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float speed;
    public LevelManager LevelManager;
    public Vector3 distFromPlayer;
    
    private Transform Player;
    private Vector3 _initialClickPosition;

    Vector3 forward, right;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        distFromPlayer = new Vector3(-2.62f, -5.03f, 5.24f);
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
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
            Vector3 rightMovement = right * speed * Time.deltaTime * -distanceMoved.x;
            Vector3 upMovement = forward * speed * Time.deltaTime * -distanceMoved.y;

            transform.position += rightMovement;
            transform.position += upMovement;
            _initialClickPosition = Input.mousePosition;
            return;
        }

        if (!LevelManager.IsPanning())
        {
            Camera.main.transform.position = Player.position - distFromPlayer;
        }
    }
}
