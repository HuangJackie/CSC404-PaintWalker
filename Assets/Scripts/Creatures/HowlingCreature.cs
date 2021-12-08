using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using static GameConstants;

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

    // For automatic panning
    private ChangePerspective changePerspective;

    new void Start()
    {
        base.Start();
        m_MyAudioSource = GetComponent<AudioSource>();
        player = GameObject.FindWithTag("Player");
        _controllerUtil = FindObjectOfType<ControllerUtil>();
        changePerspective = FindObjectOfType<ChangePerspective>();
    }

    void Update()
    {
        if (isTriggered)
        {
            if (wall)
            {
                StartCoroutine(changePerspective.MoveToPositionAndBack(wall.transform.position,
                    changePerspective.transform.position));
                wall.operate = true;
            }

            if (wall2)
            {
                StartCoroutine(changePerspective.MoveToPositionAndBack(wall2.transform.position,
                    changePerspective.transform.position));
                wall2.operate = true;
            }

            if (wall3)
            {
                StartCoroutine(changePerspective.MoveToPositionAndBack(wall3.transform.position,
                    changePerspective.transform.position));
                wall3.operate = true;
            }

            isTriggered = false;
        }
    }

    public override bool Paint(bool paintWithBrush)
    {
        if (SpecialCreatureUtil.ActivateSpecialCreature(
            isPainted,
            true,
            player.transform.position,
            transform.position,
            manager,
            paintType1,
            paintType2,
            paintQuantity1,
            paintQuantity2,
            Material,
            GameConstants.Green))
        {
            player.GetComponent<Player>().animation_update("paint", true);
            m_MyAudioSource.Play();
            originalColour = Material.color;
            paintedColour = Material.color;
            frozen_model.SetActive(false);
            coloured_model.SetActive(true);
            isPainted = true;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            player.GetComponent<Player>().animation_update("paint", false);
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
        gameObject.GetComponentInChildren<Renderer>().material.color = GameConstants.Red;
    }
}