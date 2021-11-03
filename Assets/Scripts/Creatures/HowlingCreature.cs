using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class HowlingCreature : SpecialCreature
{
    public bool isTriggered = false;
    public MoveWall wall;
    public LevelManager manager;

    // For painting
    public GameObject player;
    private ControllerUtil _controllerUtil;
    public float radius = 1.5f;

    new void Start()
    {
        base.Start();
        player = GameObject.FindWithTag("Player");
        _controllerUtil = FindObjectOfType<ControllerUtil>();
    }

    void Update()
    {
        if (isTriggered)
        {
            wall.operate = true;
            isTriggered = false;
        }
    }
    
    public override bool Paint(bool paintWithBrush)
    {
        if (SpecialCreatureUtil.ActivateSpecialCreature(
            isPainted,
            IsMouseOver || _controllerUtil.GetPaintButtonDown(),
            player.transform.position,
            transform.position,
            manager,
            paintColour1,
            paintColour2,
            paintQuantity1,
            paintQuantity2,
            Material,
            GameConstants.green))
        {
            originalColour = Material.color;
            paintedColour = Material.color;
            isPainted = true;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider hitCollider in hitColliders)
            {
                hitCollider.SendMessage("TriggerButtton",
                    SendMessageOptions.DontRequireReceiver);
            }

            return true;
        }

        return false;
    }

    /**
     * Do not delete, it is called by the SendMessage function above.
     */
    void TriggerButtton()
    {
        isTriggered = true;
        gameObject.GetComponentInChildren<Renderer>().material.color = GameConstants.red;
    }
}