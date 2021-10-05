using System.Collections;
using System.Collections.Generic;
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
        if (!_isPainted)
        {
            _isMouseClicked = Input.GetButtonDown("Fire1");
            if (_isMouseOver && _isMouseClicked &&
                Vector3.Distance(player.transform.position, gameObject.transform.position) < 3)
            {
                if (!_isPainted &&
                    manager.GetPaintQuantity(paintColour1) >= paintQuantity1 &&
                    manager.GetPaintQuantity(paintColour2) >= paintQuantity2)
                {
                    manager.DecreasePaint(paintColour1, paintQuantity1);
                    manager.DecreasePaint(paintColour2, paintQuantity2);
                    _isPainted = true;
                    _material.color = Color.magenta;
                    _originalColour = _material.color;

                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
                    foreach (Collider hitCollider in hitColliders)
                    {
                        hitCollider.SendMessage("TriggerButtton",
                            SendMessageOptions.DontRequireReceiver);
                    }
                }
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
            _updateUI.SetPaintNeededText("Needs: " + paintQuantity1 + " " + paintColour1 + " " + paintQuantity2 + " " + paintColour2);
            _material.color = new Color(0.98f, 1f, 0.45f);
            _isMouseOver = true;
        }
    }

    private void OnMouseExit()
    {
        _updateUI.SetPaintNeededText("");

        _material.color = _originalColour;
        _isMouseOver = false;
    }
}