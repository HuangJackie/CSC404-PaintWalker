using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DefaultNamespace;
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
    private float _playerYPosition;
    private float _moveableObjYPosition;

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

    public GameObject YellowSounds;
    public GameObject RedSounds;
    public GameObject BlueSounds;
    public GameObject GreenSounds;
    private SoundManager _yellowSoundManager = new SoundManager();
    private SoundManager _redSoundManager = new SoundManager();
    private SoundManager _blueSoundManager = new SoundManager();
    private SoundManager _greenSoundManager = new SoundManager();
    private SoundManager _pushIceBlockSoundManager = new SoundManager();

    void Start()
    {
        _material = GetComponentInChildren<Renderer>().material;
        _originalColour = _material.color;
        _paintedColour = _originalColour;
        _levelManager = FindObjectOfType<LevelManager>();
        player = GameObject.FindWithTag("Player");
        _player = player.GetComponent<Player>();
        _playerYPosition = player.transform.position.y;

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
        
        // Sounds

        YellowSounds = GameObject.Find("PaintingYellow");
        RedSounds = GameObject.Find("PaintingRed");
        BlueSounds = GameObject.Find("PaintingBlue");
        GreenSounds = GameObject.Find("PaintingGreen");

        _yellowSoundManager.SetAudioSources(YellowSounds.GetComponents<AudioSource>());
        _redSoundManager.SetAudioSources(RedSounds.GetComponents<AudioSource>());
        _blueSoundManager.SetAudioSources(BlueSounds.GetComponents<AudioSource>());
        _greenSoundManager.SetAudioSources(GreenSounds.GetComponents<AudioSource>());
        _pushIceBlockSoundManager.SetAudioSources(GetComponents<AudioSource>());
    }

    void Update()
    {
        if (_levelManager.freeze_player)
        {
            return;
        }
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

    IEnumerator RaiseLowerRedYellowBlockToDestination(Vector3 destination)
    {
        GameObject movableObjectOnTop = IsMovableObjectOnTop();
        bool blockPathIsBlocked = false;
        if ((destination == _destinationDrop && !NoBlockBelow()) || (destination == _destinationRaise && !NoBlockAbove()))
        {
            blockPathIsBlocked = true;
        }
        while (Vector3.Distance(transform.position, destination) > 0.01f)
        {
            if (destination == _destinationNeutral)
            {
                //print("neutrual destination. Moving block");
                transform.position = Vector3.MoveTowards(
                    transform.position, destination, speed * Time.deltaTime
                );
                if (movableObjectOnTop)
                {
                    MoveObjectWithBlock(transform.position, movableObjectOnTop);
                }

                if (Vector3.Distance(transform.position, destination) <= 0.01f)
                {
                    _isRevertingBlock = false;
                    //print("reached neutral dest");
                    yield break;
                }
            }

            if (!_isRevertingBlock)
            {
                if (blockPathIsBlocked && destination != _destinationNeutral)
                {
                    //print("changing to neutral dest since path blocked");
                    destination = _destinationNeutral;
                    blockPathIsBlocked = false;
                }
                transform.position = Vector3.Lerp(
                    transform.position, destination, speed * Time.deltaTime
                );
                if (movableObjectOnTop)
                {
                    MoveObjectWithBlock(transform.position, movableObjectOnTop);
                }
            }

            yield return null;
        }

        transform.position = destination;
        if (movableObjectOnTop)
        {
            MoveObjectWithBlock(destination, movableObjectOnTop);
        }
    }

    private void MoveObjectWithBlock(Vector3 newBlockPosition, GameObject otherObject)
    {
        if (otherObject.GetComponent<Player>())
        {
            player.transform.position = newBlockPosition + new Vector3(0, _playerYPosition + 0.01f, 0);
            _player.UpdateTargetLocation(player.transform.position);
        } else
        {
            otherObject.transform.position = newBlockPosition + new Vector3(0, _moveableObjYPosition + 0.01f, 0);
        }
    }

    private GameObject IsMovableObjectOnTop()
    {
        RaycastHit hit;
        Vector3 blockLocation = transform.position;
        GameObject otherObject = null;
        Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), Vector3.up, Color.blue, 120f);
        if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector3.up, out hit, 1)) {
            otherObject = hit.transform.gameObject;
            this._moveableObjYPosition = hit.transform.position.y;
        }
        return otherObject;
        //Vector3 playerLocation = player.transform.position;
        //return Math.Abs(blockLocation.x - playerLocation.x) < 0.1f &&
        //       Math.Abs(blockLocation.z - playerLocation.z) < 0.1f &&
        //       Math.Abs(playerLocation.y - blockLocation.y) < 2f;
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
            _pushIceBlockSoundManager.PlayRandom();
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
                    if (paintWithBrush && NoBlockBelow())
                    {
                        _redSoundManager.PlayRandom();
                        Debug.Log("red effect triggered");
                        _levelManager.EnqueueAction(() =>
                        {
                            return RaiseLowerRedYellowBlockToDestination(_destinationDrop);
                        });
                    }

                    break;
                case "Green":
                    _material.color = Paints.green;
                    _paintedColour = _material.color;
                    if (paintWithBrush)
                    {
                        _greenSoundManager.PlayRandom();
                        Debug.Log("green effect triggered");
                        _levelManager.EnqueueAction(() => { return GreenExtend(); });

                    }

                    break;
                case "Yellow":
                    _material.color = Paints.yellow;
                    _paintedColour = _material.color;
                    if (paintWithBrush && NoBlockAbove())
                    {
                        _yellowSoundManager.PlayRandom();
                        Debug.Log("yellow effect triggered");
                        _levelManager.EnqueueAction(() =>
                        {
                            return RaiseLowerRedYellowBlockToDestination(_destinationRaise);
                        });
                    }

                    break;
                case "Blue":
                    _material.color = Paints.blue;
                    _paintedColour = _material.color;
                    if (paintWithBrush)
                    {
                        this.gameObject.layer = LayerMask.NameToLayer("IceCube");
                        _blueSoundManager.PlayRandom();
                        Debug.Log("blue effect triggered");
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
            if (NoBlockAbove() && _destinationNeutral != transform.position)
            {
                //print("top empty");
                _isRevertingBlock = true;
                _levelManager.EnqueueAction(
                    () => { return RaiseLowerRedYellowBlockToDestination(_destinationNeutral); });
            } else if (_destinationNeutral != transform.position)
            {
                //print("top not empty, setting new destinations");
                _destinationNeutral = transform.position;
                _destinationDrop = _destinationNeutral + new Vector3(0, -1, 0);
                _destinationRaise = _destinationNeutral - new Vector3(0, -1, 0);
            }
            Debug.Log("reverting red");
        }
        else if (colorToRevert == Paints.green)
        {
            // Does nothing for now. Staying here in case we implement erase in the future.
            _destinationNeutral = transform.position;
            _destinationDrop = _destinationNeutral + new Vector3(0, -1, 0);
            _destinationRaise = _destinationNeutral - new Vector3(0, -1, 0);
            Debug.Log("reverting green");
        }
        else if (colorToRevert == Paints.yellow && newColor != "Blue")
        {
            if (NoBlockBelow() && _destinationNeutral != transform.position)
            {
                //print("bottom empty");
                _isRevertingBlock = true;
                _levelManager.EnqueueAction(
                    () => { return RaiseLowerRedYellowBlockToDestination(_destinationNeutral); });
            } else if (_destinationNeutral != transform.position)
            {
               // print("bottom not empty, setting new destinations");
                _destinationNeutral = transform.position;
                _destinationDrop = _destinationNeutral + new Vector3(0, -1, 0);
                _destinationRaise = _destinationNeutral - new Vector3(0, -1, 0);
            }
            Debug.Log("reverting yellow");
        }
        else if (colorToRevert == Paints.blue)
        {
            //Make the block not pushable.
            Debug.Log("reverting blue");
            this.gameObject.layer = LayerMask.NameToLayer("IceCube");
            _destinationNeutral = transform.position;
            _destinationDrop = _destinationNeutral + new Vector3(0, -1, 0);
            _destinationRaise = _destinationNeutral - new Vector3(0, -1, 0);
            _isIceBlockEffectEnabled = false;
        }
        else
        {
            Debug.LogError("color to revert from is not recognized");
        }
    }

    private bool NoBlockBelow()
    {
        LayerMask mask = LayerMask.GetMask("Default");
        Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), Vector3.down, Color.blue, 120f);
        return !Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector3.down, 1, mask);
    }

    private bool NoBlockAbove()
    {
        LayerMask mask = LayerMask.GetMask("Default");
        Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), Vector3.up, Color.red, 120f);
        return !Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector3.up, 1, mask);
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

    private IEnumerator GreenExtend()
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
        yield return null;
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
        Debug.DrawRay(currentTransformPosition + new Vector3(0, 1f, 0), Vector3.back * 1, Color.green, 120f);
        return !Physics.Raycast(currentTransformPosition + new Vector3(0, 1f, 0), Vector3.back, 1);
    }

    private bool CanUp(Vector3 currentTransformPosition)
    {
        Debug.DrawRay(currentTransformPosition + new Vector3(0, 1f, 0), Vector3.forward * 1, Color.green, 120f);
        return !Physics.Raycast(currentTransformPosition + new Vector3(0, 1f, 0), Vector3.forward, 1);
    }

    private bool CanLeft(Vector3 currentTransformPosition)
    {
        Debug.DrawRay(currentTransformPosition + new Vector3(0, 1f, 0), Vector3.left * 1, Color.green, 120f);
        return !Physics.Raycast(currentTransformPosition + new Vector3(0, 1f, 0), Vector3.left, 1);
    }

    private bool CanRight(Vector3 currentTransformPosition)
    {
        Debug.DrawRay(currentTransformPosition + new Vector3(0, 1f, 0), Vector3.right * 1, Color.green, 120f);
        return !Physics.Raycast(currentTransformPosition + new Vector3(0, 1f, 0), Vector3.right, 1);
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