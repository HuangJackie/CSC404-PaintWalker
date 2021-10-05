using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RammingSpecialCreature : MonoBehaviour
{
    // Start is called before the first frame update
    private LevelManager _levelManager;
    private Material _material;
    private Color _originalColour;
    private bool is_moving = false;
    public int speed = 3;
    public Transform target;

    void Start()
    {
        _levelManager = FindObjectOfType<LevelManager>();
        _material = GetComponentInChildren<Renderer>().material;
        _originalColour = _material.color;
    }

    private void FixedUpdate()
    {
        if (is_moving)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
            if (Vector3.Distance(transform.position, target.position) < 0.01f)
                {
                is_moving = false;
                }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<Collider>().CompareTag("Player"))
        {
            if (_levelManager.GetCurrentlySelectedPaint() == "Blue")
            {
                if (_levelManager.HasEnoughPaint())
                {
                    _material.color = Color.blue;
                    _originalColour = _material.color;
                    _levelManager.DecreaseCurrentSelectedPaint(3);
                    is_moving = true;
                }
            }
        }
        if (collision.gameObject.GetComponent<Ground>())
        {
            Debug.Log("ddddd");
            Destroy(collision.gameObject);
        }
    }
}