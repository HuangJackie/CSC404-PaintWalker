using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class RammingSpecialCreature : SpecialCreature
{
    // Start is called before the first frame update
    private LevelManager _levelManager;
    private bool is_moving = false;
    public int speed = 3;
    public Transform target;
    
    // For clicking
    public GameObject player;

    new void Start()
    {
        base.Start();
        _levelManager = FindObjectOfType<LevelManager>();
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (IsMouseOver &&
            SpecialCreatureUtil.ActivateSpecialCreature(
                isPainted,
                IsMouseOver,
                Input.GetButtonDown("Fire1"),
                player.transform.position,
                transform.position,
                _levelManager,
                paintColour1,
                paintColour2,
                paintQuantity1,
                paintQuantity2,
                Material,
                Color.magenta))
        {
            OriginalColour = Material.color;
            isPainted = true;
            is_moving = true;
        }
    }

    private void FixedUpdate()
    {
        if (is_moving)
        {
            _levelManager.freeze_player = true;
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
            if (Vector3.Distance(transform.position, target.position) < 0.01f)
            {
                is_moving = false;
                _levelManager.freeze_player = false;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<Ground>())
        {
            Destroy(collision.gameObject);
        }
    }
}