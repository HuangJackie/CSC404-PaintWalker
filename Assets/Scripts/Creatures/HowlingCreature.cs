using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class HowlingCreature : SpecialCreature
{
    public bool isTriggered = false;
    public MoveWall wall;
    public MoveWall wall2;
    public MoveWall wall3;
    public LevelManager manager;

    // For painting
    public GameObject player;
    private ControllerUtil _controllerUtil;
    public float radius = 1.5f;
    private AudioSource m_MyAudioSource;


    new void Start()
    {
        base.Start();
        m_MyAudioSource = GetComponent<AudioSource>();
        player = GameObject.FindWithTag("Player");
        _controllerUtil = FindObjectOfType<ControllerUtil>();
    }

    void Update()
    {
        if (isTriggered)
        {
            wall.operate = true;
            if (wall2)
            {
                wall2.operate = true;
            }
            if (wall3)
            {
                wall3.operate = true;
            }
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
            player.GetComponent<Player>().animation_update("paint", true);
            m_MyAudioSource.Play();
            originalColour = Material.color;
            paintedColour = Material.color;
            isPainted = true;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider hitCollider in hitColliders)
            {
                hitCollider.SendMessage("TriggerButtton",
                    SendMessageOptions.DontRequireReceiver);
            }
            ReinitializeMaterialColours();

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