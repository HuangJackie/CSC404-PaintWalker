using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using static GameConstants;

public class RammingCreature : SpecialCreature
{
    // Start is called before the first frame update
    private LevelManager _levelManager;
    private bool is_moving = false;
    public int speed = 3;
    public Transform target;
    
    // For painting
    public GameObject player;
    private ControllerUtil _controllerUtil;
    public GameObject color_idle;
    
    new void Start()
    {
        base.Start();
        _levelManager = FindObjectOfType<LevelManager>();
        player = GameObject.FindWithTag("Player");
        _controllerUtil = FindObjectOfType<ControllerUtil>();
    }

    private void FixedUpdate()
    {
        if (is_moving)
        {
            _levelManager.freezePlayer = true;
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
            if (Vector3.Distance(transform.position, target.position) < 0.01f)
            {
                is_moving = false;
                _levelManager.freezePlayer = false;
                coloured_model.SetActive(false);
                color_idle.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        Ground collidedGround = collision.gameObject.GetComponent<Ground>();
        if (collidedGround && collidedGround.IsPaintable())
        {
            collision.gameObject.SetActive(false);
        }
    }

    private bool PlayerInTheWay()
    {
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Player");
        float dist = Mathf.Abs(target.position.x - transform.position.x);
        Debug.DrawRay(transform.position, (target.position - transform.position), Color.red, 120f);
        if (Physics.Raycast(transform.position, target.position - transform.position, out hit, dist, mask))
        {
            print("player in the way");
            return true;
        }
        return false;
    }
    
    public override bool Paint(bool paintWithBrush)
    {
        if (SpecialCreatureUtil.ActivateSpecialCreature(
                isPainted,
                true,
                player.transform.position,
                transform.position,
                _levelManager,
                paintType1,
                paintType2,
                paintQuantity1,
                paintQuantity2,
                Material,
                Color.magenta))
        {
            player.GetComponent<Player>().animation_update("paint", true);
            originalColour = Material.color;
            paintedColour = Material.color;
            frozen_model.SetActive(false);
            coloured_model.SetActive(true);
            isPainted = true;
            is_moving = true;
            return true;
        }
        return false;
    }
}