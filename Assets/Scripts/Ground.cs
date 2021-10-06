using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

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
    private bool _isRaisingBlock;
    private bool _isMovingBlock;
    private bool _isIceBlockEffectEnabled;
    private Vector3 _destinationDrop;
    private Vector3 _destinationRaise;
    private Vector3 _destinationMove;

    // Start is called before the first frame update
    void Start()
    {
        _material = GetComponentInChildren<Renderer>().material;
        _originalColour = _material.color;
        _levelManager = FindObjectOfType<LevelManager>();

        _isMouseOver = false;

        _isDroppingBlock = false;
        _isRaisingBlock = false;
        _isMovingBlock = false;
        _isIceBlockEffectEnabled = false;
        _destinationDrop = transform.position + new Vector3(0, -1, 0);
        _destinationRaise = transform.position - new Vector3(0, -1, 0);
        _destinationMove = transform.position;
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
                PaintSurface(true);
            }
        }

        if (_isDroppingBlock)
        {
            transform.position = Vector3.MoveTowards(transform.position, _destinationDrop, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _destinationDrop) <= 0.01f)
            {
                _isDroppingBlock = false;
                transform.position = _destinationDrop;
            }
        }

        if (_isRaisingBlock)
        {
            transform.position = Vector3.MoveTowards(transform.position, _destinationRaise, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _destinationRaise) <= 0.01f)
            {
                _isRaisingBlock = false;
                transform.position = _destinationRaise;
            }
        }

        if (_isMovingBlock)
        {
            transform.position = Vector3.MoveTowards(transform.position, _destinationMove, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _destinationMove) <= 0.01f)
            {
                _isMovingBlock = false;
                transform.position = _destinationMove;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!_isPainted && other.gameObject.CompareTag("Player"))
        {
            PaintSurface(false);
        }

        IceBlockMovement(other);
        if (other.gameObject.CompareTag("RammingCreature"))
        {
            Destroy(gameObject);
        }
    }

    private void IceBlockMovement(Collision other)
    {
        Vector3 dir = ReturnDirection(other.gameObject, gameObject);
        bool shouldMoveIceBlock = _isIceBlockEffectEnabled &&
                                  other.gameObject.CompareTag("Player") &&
                                  dir != Vector3.negativeInfinity;
        if (shouldMoveIceBlock)
        {
            Vector3 directionToPush = GetDirectionToMoveIceBlock(dir);
            Vector3 pos = transform.position + new Vector3(0, 0.5f, 0);
            if (!Physics.Raycast(pos, directionToPush, maxDistance: 1, 0))
            {
                _destinationMove += directionToPush;
                _isMovingBlock = true;
            }
        }
    }

    private Vector3 GetDirectionToMoveIceBlock(Vector3 dir)
    {
        string direction = "VERTICAL";
        if (Math.Abs(dir.x) > Math.Abs(dir.z))
        {
            direction = "HORIZONTAL";
        }

        switch (direction)
        {
            case "HORIZONTAL":
                if (dir.x < 0)
                {
                    return Vector3.left;
                }

                return Vector3.right;
            case "VERTICAL":
                if (dir.z > 0)
                {
                    return Vector3.forward;
                }

                return Vector3.back;
            default:
                return new Vector3(0, 0, 0);
        }
    }

    public bool PaintSurface(bool applyPaintEffect = false)
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
                    if (applyPaintEffect)
                    {
                        RedFall();
                    }

                    break;
                case "Green":
                    _material.color = Color.green;
                    _originalColour = _material.color;
                    if (applyPaintEffect)
                    {
                        GreenExtend();
                    }

                    break;
                case "Yellow":
                    _material.color = Color.yellow;
                    _originalColour = _material.color;
                    if (applyPaintEffect)
                    {
                        YellowRise();
                    }

                    break;
                case "Blue":
                    _material.color = Color.cyan;
                    _originalColour = _material.color;
                    if (applyPaintEffect)
                    {
                        _isIceBlockEffectEnabled = true;
                    }

                    break;
            }

            return true;
        }

        return false;
    }

    // move platform code (ice)
    private Vector3 ReturnDirection(GameObject Object, GameObject ObjectHit)
    {
        Vector3 hitDirection = Vector3.negativeInfinity;
        RaycastHit RayHit;
        Vector3 direction = (Object.transform.position - ObjectHit.transform.position).normalized;
        Ray MyRay = new Ray(ObjectHit.transform.position, direction);

        if (Physics.Raycast(MyRay, out RayHit))
        {
            if (RayHit.collider != null)
            {
                Vector3 MyNormal = RayHit.normal;
                hitDirection = MyNormal;
            }
        }

        return hitDirection;
    }
    // move platform code (ice)

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

    private void RedFall()
    {
        _isDroppingBlock = true;
    }

    private void YellowRise()
    {
        _isRaisingBlock = true;
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