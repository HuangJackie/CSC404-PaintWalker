using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class OnButtonTrigger : MonoBehaviour
{
    public bool isTriggered = false;
    public MoveWall wall;
    public LevelManager manager;

    public GameObject player;
    public float radius = 1.5f;
    public bool _isPainted;

    public string paintColour1;
    public string paintColour2;
    public int paintQuantity1;
    public int paintQuantity2;

    private Material _material;
    private Color _originalColour;
    private UpdateUI _updateUI;
    private bool _isMouseClicked;

    private bool _isMouseOver;

    // Start is called before the first frame update
    void Start()
    {
        _material = GetComponentInChildren<Renderer>().material;
        _originalColour = _material.color;
        _isMouseOver = false;
        _updateUI = FindObjectOfType<UpdateUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SpecialCreatureUtil.ActivateSpecialCreature(
            _isPainted,
            _isMouseOver,
            Input.GetButtonDown("Fire1"),
            player.transform.position,
            transform.position,
            manager,
            paintColour1,
            paintColour2,
            paintQuantity1,
            paintQuantity2,
            _material,
            Color.green))
        {
            _originalColour = _material.color;
            _isPainted = true;
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
        gameObject.GetComponentInChildren<Renderer>().material.color = Color.red;
        manager.DecreaseCurrentSelectedPaint(2);
    }

    private void OnMouseOver()
    {
        if (!_isPainted)
        {
            _updateUI.SetInfoText("Needs: " + paintQuantity1 + " " + paintColour1 + " " + paintQuantity2 + " " +
                                         paintColour2);
            _material.color = new Color(0.98f, 1f, 0.45f);
            _isMouseOver = true;
        }
    }

    private void OnMouseExit()
    {
        _updateUI.SetInfoText("");

        _material.color = _originalColour;
        _isMouseOver = false;
    }
}