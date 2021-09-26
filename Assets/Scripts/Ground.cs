using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public GameObject player;
    public float speed;

    public bool _isPainted;
    private LevelManager _levelManager;
    private Material _material;
    private Color _originalColour;
    private UpdateUI _updateUI;

    private bool _isMouseClicked;
    private bool _isMouseOver;

    private bool _isDroppingBlock;
    private Vector3 _destination;

    // Start is called before the first frame update
    void Start()
    {
        _material = GetComponentInChildren<Renderer>().material;
        _originalColour = _material.color;
        _levelManager = FindObjectOfType<LevelManager>();

        _isMouseOver = false;

        _isDroppingBlock = false;
        _destination = transform.position + new Vector3(0, -1, 0);
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
                PaintSurface();
            }
        }

        if (_isDroppingBlock)
        {
            transform.position = Vector3.MoveTowards(transform.position, _destination, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _destination) <= 0.01f)
            {
                _isDroppingBlock = false;
                transform.position = _destination;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!_isPainted && other.gameObject.CompareTag("Player"))
        {
            PaintSurface();
        }
    }

    public bool PaintSurface()
    {
        if (_levelManager.HasEnoughPaint() && !_isPainted)
        {
            _levelManager.DecreaseCurrentSelectedPaint(1);
            _isPainted = true;
            string currentlySelectedPaint = _levelManager.GetCurrentlySelectedPaint();
            switch (currentlySelectedPaint)
            {
                case "Red":
                    _material.color = Color.red;
                    _originalColour = _material.color;

                    break;
                case "Green":
                    _material.color = Color.green;
                    _originalColour = _material.color;
                    GreenExtend();
                    break;
                case "Black":
                    _material.color = Color.black;
                    _originalColour = _material.color;
                    BlackFall();
                    break;
                case "Orange":
                    _material.color = Color.yellow;
                    _originalColour = _material.color;
                    break;
                case "Special":
                    _material.color = Color.cyan;
                    _originalColour = _material.color;
                    break;
            }

            return true;
        }

        return false;
    }

    private void GreenExtend()
    {
        //extends platforms in wasd directions by 1 block
        Vector3 position = transform.position;
        var growth_block = Resources.Load(path: "NonColouredBlock");
        if (CanDown(position))
        {
            InstantiateNewBlock(growth_block, position, Vector3.back);
        }

        if (CanUp(position))
        {
            InstantiateNewBlock(growth_block, position, Vector3.forward);
        }

        if (CanLeft(position))
        {
            InstantiateNewBlock(growth_block, position, Vector3.left);
        }

        if (CanRight(position))
        {
            InstantiateNewBlock(growth_block, position, Vector3.right);
        }
    }

    private void InstantiateNewBlock(Object growth_block, Vector3 position, Vector3 direction)
    {
        GameObject newBlock = (GameObject) Instantiate(growth_block, position + direction,
            Quaternion.identity);
        Ground newGround = newBlock.GetComponent<Ground>();
        newGround.player = player;
        newGround.speed = speed;
        newGround._isPainted = false;
    }

    private bool CanDown(Vector3 currentTransformPosition)
    {
        return !Physics.Raycast(currentTransformPosition + new Vector3(0, 2, -1), Vector3.down, 3);
    }

    private bool CanUp(Vector3 currentTransformPosition)
    {
        return !Physics.Raycast(currentTransformPosition + new Vector3(0, 2, 1), Vector3.down, 3);
    }

    private bool CanLeft(Vector3 currentTransformPosition)
    {
        return !Physics.Raycast(currentTransformPosition + new Vector3(-1, 2, 0), Vector3.down, 3);
    }

    private bool CanRight(Vector3 currentTransformPosition)
    {
        return !Physics.Raycast(currentTransformPosition + new Vector3(1, 2, 0), Vector3.down, 3);
    }

    private void BlackFall()
    {
        _isDroppingBlock = true;
    }

    private void OnMouseOver()
    {
        if (!_isPainted)
        {
            _material.color = new Color(0.98f, 1f, 0.45f);
            _isMouseOver = true;
        }
    }

    private void OnMouseExit()
    {
        _material.color = _originalColour;
        _isMouseOver = false;
    }
}