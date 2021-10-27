using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class HowlingCreature : SpecialCreature
{
    public bool isTriggered = false;
    public MoveWall wall;
    public LevelManager manager;

    public GameObject player;
    public float radius = 1.5f;

    new void Start()
    {
        base.Start();
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (SpecialCreatureUtil.ActivateSpecialCreature(
            isPainted,
            IsMouseOver,
            Input.GetButtonDown("Fire1"),
            player.transform.position,
            transform.position,
            manager,
            paintColour1,
            paintColour2,
            paintQuantity1,
            paintQuantity2,
            Material,
            Paints.green))
        {
            OriginalColour = Material.color;
            isPainted = true;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider hitCollider in hitColliders)
            {
                hitCollider.SendMessage("TriggerButtton",
                    SendMessageOptions.DontRequireReceiver);
            }
        }

        if (isTriggered)
        {
            wall.operate = true;
            isTriggered = false;
        }
    }

    void TriggerButtton()
    {
        isTriggered = true;
        gameObject.GetComponentInChildren<Renderer>().material.color = Paints.red;
    }
}