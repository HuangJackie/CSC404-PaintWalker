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
    public string test; // for debugging by giving a block a specific name

    public bool isPaintedByFeet;
    public bool isPaintedByBrush;
    private LevelManager _levelManager;
    private Material _material;
    private Color _originalColour;
    private Color _paintedColour;
    private UpdateUI _updateUI;
    private Player _player;

    private bool _isMouseClicked;
    private bool _isMouseOver;

    private bool _isDroppingBlock;
    private bool _isRevertingBlock;
    private bool _isRaisingBlock;
    private bool _isIceBlockEffectEnabled;
    private bool _isOnSameLevelAsPlayer;
    private Vector3 _directionToSlideTo;
    private Vector3 _destinationDrop;
    private Vector3 _destinationNeutral;
    private Vector3 _destinationRaise;
    private Vector3 _destinationMove;

    void Start()
    {
        _material = GetComponentInChildren<Renderer>().material;
        _originalColour = _material.color;
        _paintedColour = _originalColour;
        _levelManager = FindObjectOfType<LevelManager>();
        player = GameObject.FindWithTag("Player");
        _player = player.GetComponent<Player>();

        _isMouseOver = false;

        _isOnSameLevelAsPlayer = false;
        _isDroppingBlock = false;
        _isRevertingBlock = false;
        _isRaisingBlock = false;
        _isIceBlockEffectEnabled = false;
        _destinationDrop = transform.position + new Vector3(0, -1, 0);
        _destinationNeutral = transform.position;
        _destinationRaise = transform.position - new Vector3(0, -1, 0);
        _destinationMove = transform.position;
        _directionToSlideTo = Vector3.zero;
    }

    void Update()
    {
        float playerBlockVertialDistance = Mathf.Abs(
            _player.gameObject.transform.position.y - gameObject.transform.position.y
        );

        if (playerBlockVertialDistance < 1.5f)
        {
            _isOnSameLevelAsPlayer = true;
        }
        else
        {
            _isOnSameLevelAsPlayer = false;
        }

        if (_levelManager.GetCurrentlySelectedPaintClass() != _paintedColour || !isPaintedByBrush)
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
            Debug.Log("dropping");
            transform.position = Vector3.MoveTowards(
                transform.position, _destinationDrop, speed * Time.deltaTime
            );
            if (Vector3.Distance(transform.position, _destinationDrop) <= 0.01f)
            {
                _isDroppingBlock = false;
                transform.position = _destinationDrop;
            }
        }

        if (_isRaisingBlock)
        {
            Debug.Log("raising");
            transform.position = Vector3.MoveTowards(
                transform.position, _destinationRaise, speed * Time.deltaTime
            );
            if (Vector3.Distance(transform.position, _destinationRaise) <= 0.01f)
            {
                _isRaisingBlock = false;
                transform.position = _destinationRaise;
            }
        }
    }

    private bool ReinitializeIceBlockMovement(bool isPushed)
    {
        bool canMove = false;
        // Check if there is a block below.
        _destinationMove = transform.position;
        Vector3 pos = _destinationMove + new Vector3(0, 0.5f, 0);
        if (!Physics.Raycast(pos, Vector3.down, 0.7f))
        {
            Debug.Log("Can move down");
            _destinationMove += Vector3.down;
            canMove = true; // _isMovingBlock = true;
        }
        // Check if there is a block in front.
        else if (isPushed && !Physics.Raycast(pos, _directionToSlideTo, 0.7f))
        {
            _destinationMove += _directionToSlideTo;
            canMove = true;
            // _isMovingBlock = true;
        }

        if (pos.y < -10 || pos.z > 30 || pos.z < -10 || pos.x > 20 || pos.x < -30)
        {
            // Block out of bounds so set inactive.
            canMove = false;
            gameObject.SetActive(false);
        }

        return canMove;
    }

    IEnumerator MoveIceBlockToDestination(bool isPushed)
    {
        bool stillMoving = true;
        while (stillMoving)
        {
            float distance = Vector3.Distance(transform.position, _destinationMove);
            while (distance > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, _destinationMove, speed * 2 * Time.deltaTime
                );
                yield return null;

                distance = Vector3.Distance(transform.position, _destinationMove);
            }

            stillMoving = ReinitializeIceBlockMovement(isPushed);
        }
    }

    IEnumerator MoveBlockToDestination(Vector3 destination)
    {
        while (Vector3.Distance(transform.position, destination) > 0.01f)
        {
            if (destination == _destinationNeutral)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, destination, speed * Time.deltaTime
                );
                if (Vector3.Distance(transform.position, destination) <= 0.01f)
                {
                    _isRevertingBlock = false;
                    yield break;
                }
            }

            if (!_isRevertingBlock)
            {
                transform.position = Vector3.Lerp(
                    transform.position, destination, speed * Time.deltaTime
                );
            }

            yield return null;
        }

        transform.position = destination;
    }

    private void OnCollisionEnter(Collision other)
    {
        if ((!isPaintedByBrush || !isPaintedByFeet) && other.gameObject.CompareTag("Player"))
        {
            PaintSurface(false);
        }

        IceBlockMovementWhenPushed(other);
        if (other.gameObject.CompareTag("SpecialCreature"))
        {
            Destroy(gameObject);
        }
    }

    public bool CanIceBlockSlide(GameObject player)
    {
        Vector3 dir = ReturnDirection(player, gameObject);
        Vector3 directionToPush = GetDirectionToMoveIceBlock(dir);
        Vector3 pos = transform.position + new Vector3(0, 0.5f, 0);
        return _isIceBlockEffectEnabled &&
               _isOnSameLevelAsPlayer &&
               dir != Vector3.negativeInfinity &&
               !Physics.Raycast(pos, directionToPush, 0.7f);
    }

    private void IceBlockMovementWhenPushed(Collision other)
    {
        Vector3 dir = ReturnDirection(other.gameObject, gameObject);
        bool shouldMoveIceBlock = _isIceBlockEffectEnabled &&
                                  _isOnSameLevelAsPlayer &&
                                  other.gameObject.CompareTag("Player") &&
                                  dir != Vector3.negativeInfinity;

        if (shouldMoveIceBlock)
        {
            Vector3 directionToPush = GetDirectionToMoveIceBlock(dir);
            Vector3 pos = transform.position + new Vector3(0, 0.5f, 0);
            if (!Physics.Raycast(pos, directionToPush, 0.7f))
            {
                _destinationMove = transform.position + directionToPush;
                _directionToSlideTo = directionToPush;

                _levelManager.EnqueueAction(() => { return MoveIceBlockToDestination(true); });
                // _isMovingBlock = true;
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

    public bool PaintSurface(bool paintWithBrush = false)
    {
        if (_levelManager.HasEnoughPaint())
        {
            if ((this.isPaintedByBrush || this.isPaintedByFeet) && !paintWithBrush)
            {
                return false;
            }

            _levelManager.DecreaseCurrentSelectedPaint(1);
            string currentlySelectedPaint = _levelManager.GetCurrentlySelectedPaint();

            if (_paintedColour != _originalColour && this.isPaintedByBrush)
            {
                RevertEffect(this._paintedColour, currentlySelectedPaint);
            }

            switch (currentlySelectedPaint)
            {
                case "Red":
                    _material.color = Paints.red;
                    _paintedColour = _material.color;
                    if (paintWithBrush)
                    {
                        Debug.Log("effect triggered");
                        _levelManager.EnqueueAction(() => { return MoveBlockToDestination(_destinationDrop); });
                    }

                    break;
                case "Green":
                    _material.color = Paints.green;
                    _paintedColour = _material.color;
                    if (paintWithBrush && !this.isPaintedByBrush)
                    {
                        Debug.Log("effect triggered");
                        GreenExtend();
                    }

                    break;
                case "Yellow":
                    _material.color = Paints.yellow;
                    _paintedColour = _material.color;
                    if (paintWithBrush)
                    {
                        Debug.Log("effect triggered");
                        _levelManager.EnqueueAction(() => { return MoveBlockToDestination(_destinationRaise); });
                    }

                    break;
                case "Blue":
                    _material.color = Paints.blue;
                    _paintedColour = _material.color;
                    if (paintWithBrush)
                    {
                        Debug.Log("effect triggered");
                        _isIceBlockEffectEnabled = true;
                        _destinationMove = transform.position;
                        _levelManager.EnqueueAction(() => { return MoveIceBlockToDestination(false); });
                    }

                    break;
            }

            if (!paintWithBrush)
            {
                isPaintedByFeet = true;
            }
            else
            {
                isPaintedByBrush = true;
            }

            return true;
        }

        return false;
    }

    private void RevertEffect(Color colorToRevert, String newColor)
    {
        if (colorToRevert == Paints.red && newColor != "Blue")
        {
            _isRevertingBlock = true;
            _levelManager.EnqueueAction(() => { return MoveBlockToDestination(_destinationNeutral); });
            Debug.Log("reverting red");
        }
        else if (colorToRevert == Paints.green)
        {
            // Does nothing for now. Staying here in case we implement erase in the future.
            Debug.Log("reverting green");
        }
        else if (colorToRevert == Paints.yellow && newColor != "Blue")
        {
            if (NoBlockBelow())
            {
                _isRevertingBlock = true;
                _levelManager.EnqueueAction(() => { return MoveBlockToDestination(_destinationNeutral); });
            }
            Debug.Log("reverting yellow");
        }
        else if (colorToRevert == Paints.blue)
        {
            //Make the block not pushable.
            Debug.Log("reverting blue");
            _isIceBlockEffectEnabled = false;
        }
        else
        {
            Debug.LogError("color to revert from is not recognized");
        }
    }

    private bool NoBlockBelow()
    {
        return !Physics.Raycast(transform.position, Vector3.down, 1);
    }

    IEnumerator ExtendGreenAfterRevert()
    {
        while (true)
        {
            if (!_isRevertingBlock)
            {
                GreenExtend();
                yield break;
            }

            yield return null;
        }
    }

    // move platform code (ice)
    private Vector3 ReturnDirection(GameObject Object, GameObject ObjectHit)
    {
        Vector3 hitDirection = Vector3.negativeInfinity;
        RaycastHit RayHit;
        Vector3 direction = (Object.transform.position - ObjectHit.transform.position).normalized;
        Ray MyRay = new Ray(ObjectHit.transform.position, direction);

        if (Physics.Raycast(MyRay, out RayHit) && RayHit.collider != null)
        {
            Vector3 MyNormal = RayHit.normal;
            hitDirection = MyNormal;
        }

        return hitDirection;
    }

    private void GreenExtend()
    {
        // Extends platforms in wasd directions by 1 block
        Vector3 position = transform.position;
        Vector3 positionToCheck = position - new Vector3(0, 0.5f, 0);
        var growth_block = Resources.Load(path: "NonColouredBlock");
        if (CanDown(positionToCheck))
        {
            InstantiateNewBlock(growth_block, position, Vector3.back);
        }

        if (CanUp(positionToCheck))
        {
            InstantiateNewBlock(growth_block, position, Vector3.forward);
        }

        if (CanLeft(positionToCheck))
        {
            InstantiateNewBlock(growth_block, position, Vector3.left);
        }

        if (CanRight(positionToCheck))
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
        newGround.isPaintedByBrush = false;
        newGround.isPaintedByFeet = false;
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

    private void OnMouseOver()
    {
        // Note: the tint should be the same color as the currently selected paint
        // Also, when block is in transit (e.g. moving), the highlight should disappear
        // as it is not selectable
        _material.color = new Color(0.98f, 1f, 0.45f);
        _isMouseOver = true;
    }

    private void OnMouseExit()
    {
        _material.color = _paintedColour;
        _isMouseOver = false;
    }

    public bool IsIceBlock()
    {
        return _isIceBlockEffectEnabled;
    }
}