using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public class Ground : Interactable, Paintable
{
    public GameObject player;
    public float speed;
    public string test; // for debugging by giving a block a specific name

    public bool isPaintedByFeet;
    public bool isPaintedByBrush;

    public Color _paintedColour;
    // [FormerlySerializedAs("_paintedColour")] public Color originalColour;
    public bool isPaintable = true;

    private MoveRedo latestState;
    private LevelManager _levelManager;
    private UpdateUI _updateUI;
    private Player _player;
    private float _playerYPosition;
    private float _moveableObjYPosition;

    private bool _isMouseClicked;
    private bool _isMouseOver;

    private bool _isIceBlockEffectEnabled;
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

    private new void Start()
    {
        _levelManager = FindObjectOfType<LevelManager>();
        ObjectStorage objectStorage = FindObjectOfType<ObjectStorage>();
        objectStorage.AddBlock(this.gameObject);
        Material = GetComponentInChildren<Renderer>().material;
        originalColour = Material.color;
        _paintedColour = originalColour; // TODO: Replace _paintedColour with paintedColour
        paintedColour = Material.color;
        player = GameObject.FindWithTag("Player");
        _player = player.GetComponent<Player>();
        _playerYPosition = player.transform.position.y;

        _isMouseOver = false;

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

        if (_levelManager.GetCurrentlySelectedPaintClass().ToString() != _paintedColour.ToString() || !isPaintedByBrush)
        {
            _isMouseClicked = Input.GetButtonDown("Fire1");
            bool clickedUI = EventSystem.current.IsPointerOverGameObject();

            Vector3 horizontalPlayerPosition = new Vector3(player.transform.position.x, 0, player.transform.position.z);
            Vector3 horizontalBlockPosition = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
            if (_isMouseOver && _isMouseClicked && !clickedUI &&
                Vector3.Distance(horizontalPlayerPosition, horizontalBlockPosition) < 3)
            {
                Paint(true);
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

        if (pos.y < -5)
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
            if (distance < 0.01f)
            {
                transform.position = _destinationMove;
            }
            stillMoving = ReinitializeIceBlockMovement(isPushed);
        }
    }

    IEnumerator RaiseLowerRedYellowBlockToDestination(Vector3 destination, bool reverting = false)
    {
        GameObject movableObjectOnTop = IsMovableObjectOnTop();
        if (!movableObjectOnTop && !reverting)
        {
            print("no object on top");
            MoveRedo NewState = ScriptableObject.CreateInstance("MoveRedo") as MoveRedo;
            NewState.ObjectInit(gameObject);
            _levelManager.redoCommandHandler.AddCommand(NewState);
            _levelManager.redoCommandHandler.TransitionToNewGameState();
        }
        else if (movableObjectOnTop && !reverting)
        {
            print("object on top");
            bool up;
            if (destination.y > transform.position.y)
            {
                up = true;
            }
            else
            {
                up = false;
            }

            _player.CreateCopyOfCurrentState(up);
            _player.GameState.InjectBlockState(gameObject);
        }

        bool blockPathIsBlocked = (destination == _destinationDrop && !NoBlockBelow()) ||
                                  (destination == _destinationRaise && !NoUnmovableBlockAbove());
        while (this && Vector3.Distance(transform.position, destination) > 0.01f)
        {
            if (reverting)
            {
                //print("neutrual destination. Moving block to revert");
                transform.position = Vector3.MoveTowards(
                    transform.position, destination, speed * 1.6f * Time.deltaTime
                );
                if (movableObjectOnTop)
                {
                    MoveObjectWithBlock(transform.position, movableObjectOnTop);
                }

                if (Vector3.Distance(transform.position, destination) <= 0.01f)
                {
                    transform.position = destination;
                    //if (movableObjectOnTop && movableObjectOnTop.CompareTag("Player"))
                    //{
                    //_player.GameState.UpdatePlayerY(player.transform.position.y);
                    //}
                    //print("reached neutral dest");
                    yield break;
                }
            }
            else
            {
                //print("moving by effect");
                if (blockPathIsBlocked && destination != _destinationNeutral)
                {
                    //print("changing to neutral dest since path blocked");
                    destination = _destinationNeutral;
                    blockPathIsBlocked = false;
                }

                transform.position = Vector3.Lerp(
                    transform.position, destination, speed * 1.6f * Time.deltaTime
                );
                if (Vector3.Distance(transform.position, destination) <= 0.01f)
                {
                    transform.position = destination;
                    //print("reached effect dest");
                    if (movableObjectOnTop && movableObjectOnTop.CompareTag("Player"))
                    {
                        //_player.GameState.UpdatePlayerY(player.transform.position.y);
                    }

                    yield break;
                }

                if (movableObjectOnTop)
                {
                    MoveObjectWithBlock(transform.position, movableObjectOnTop);
                }
            }

            yield return null;
        }

        if (this)
        {
            transform.position = destination;
            if (movableObjectOnTop)
            {
                MoveObjectWithBlock(destination, movableObjectOnTop);
            }
        }
    }

    private void MoveObjectWithBlock(Vector3 curBlockPosition, GameObject otherObject)
    {
        //if (otherObject.GetComponent<Player>())
        //{
        //    _player.UpdateTargetLocation(curBlockPosition + new Vector3(0, _playerYPosition + 0.01f, 0));
        //}
        if (otherObject.gameObject.layer == LayerMask.NameToLayer("IceCube"))
        {
            otherObject.transform.position = curBlockPosition + new Vector3(0, 1, 0);
        }
    }

    private GameObject IsMovableObjectOnTop()
    {
        RaycastHit hit;
        Vector3 blockLocation = transform.position;
        GameObject otherObject = null;
        Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), Vector3.up, Color.blue, 120f);
        if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector3.up, out hit, 1))
        {
            otherObject = hit.transform.gameObject;
            _moveableObjYPosition = hit.transform.position.y;
        }

        return otherObject;
    }

    private void OnCollisionEnter(Collision other)
    {
        if ((!isPaintedByBrush || !isPaintedByFeet) && other.gameObject.CompareTag("Player"))
        {
            Paint(false);
        }

        IceBlockMovementWhenPushed(other);
        if (other.gameObject.CompareTag("SpecialCreature"))
        {
            this.gameObject.SetActive(false);
        }
    }

    public bool CanIceBlockSlide(GameObject player)
    {
        Vector3 dir = ReturnDirection(player, gameObject);
        Vector3 directionToPush = GetDirectionToMoveIceBlock(dir);
        Vector3 pos = transform.position + new Vector3(0, 0.5f, 0);
        return _isIceBlockEffectEnabled &&
               //_isOnSameLevelAsPlayer &&
               dir != Vector3.negativeInfinity &&
               !Physics.Raycast(pos, directionToPush, 0.7f);
    }

    private void IceBlockMovementWhenPushed(Collision other)
    {
        Vector3 dir = ReturnDirection(other.gameObject, gameObject);
        RaycastHit RayHit;
        bool _isOnSameLevelAsPlayer = false;
        if (Physics.Raycast(other.gameObject.transform.position, other.gameObject.transform.forward, out RayHit) && RayHit.collider.gameObject == this.gameObject)
        {
            _isOnSameLevelAsPlayer = true;
        }
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
                print("pushing");
                if (_player.GameState != null)
                {
                    _player.GameState.InjectBlockState(gameObject);
                }

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

    public bool Paint(bool paintWithBrush = false)
    {
        if (!IsPaintable())
        {
            return false;
        }
        
        // TODO: (Refractor) the method that calls this to remove this duplicate check since it's present here already.
        if (paintWithBrush && (_levelManager.GetCurrentlySelectedPaintClass() == _paintedColour && isPaintedByBrush))
        {
            return false;
        }
        
        if (!_levelManager.HasEnoughPaint())
        {
            return false;
        }

        if ((isPaintedByBrush || isPaintedByFeet) && !paintWithBrush)
        {
            return false;
        }

        string currentlySelectedPaint = _levelManager.GetCurrentlySelectedPaint();

        if (_paintedColour != originalColour && isPaintedByBrush)
        {
            RevertEffect(_paintedColour, currentlySelectedPaint);
        }

        if (!paintWithBrush && _player.GameState != null)
        {
            _player.GameState.InjectBlockState(gameObject);
        }

        switch (currentlySelectedPaint)
        {
            case "Red":
                Material.color = GameConstants.red;
                _paintedColour = Material.color;
                paintedColour = Material.color;
                _levelManager.DecreasePaint("Red", 1);
                if (paintWithBrush && NoBlockBelow())
                {
                    _redSoundManager.PlayRandom();
                    Debug.Log("red effect triggered");
                    _levelManager.EnqueueAction(() =>
                    {
                        return RaiseLowerRedYellowBlockToDestination(_destinationDrop);
                    });
                    isPaintedByBrush = true;
                }

                break;
            case "Green":
                Material.color = GameConstants.green;
                _paintedColour = Material.color;
                paintedColour = Material.color;
                _levelManager.DecreasePaint("Green", 1);
                if (paintWithBrush)
                {
                    _greenSoundManager.PlayRandom();
                    Debug.Log("green effect triggered");
                    MoveRedo NewState = ScriptableObject.CreateInstance("MoveRedo") as MoveRedo;
                    NewState.ObjectInit(gameObject);
                    _levelManager.redoCommandHandler.AddCommand(NewState);
                    _levelManager.redoCommandHandler.TransitionToNewGameState();
                    _levelManager.EnqueueAction(() => { return GreenExtend(NewState); });
                    isPaintedByBrush = true;
                }

                break;
            case "Yellow":
                Material.color = GameConstants.yellow;
                _paintedColour = Material.color;
                paintedColour = Material.color;
                _levelManager.DecreasePaint("Yellow", 1);
                if (paintWithBrush && NoUnmovableBlockAbove())
                {
                    _yellowSoundManager.PlayRandom();
                    Debug.Log("yellow effect triggered");
                    _levelManager.EnqueueAction(() =>
                    {
                        return RaiseLowerRedYellowBlockToDestination(_destinationRaise);
                    });
                    isPaintedByBrush = true;
                }

                break;
            case "Blue":
                Material.color = GameConstants.blue;
                _paintedColour = Material.color;
                paintedColour = Material.color;
                _levelManager.DecreasePaint("Blue", 1);
                if (paintWithBrush)
                {
                    gameObject.layer = LayerMask.NameToLayer("IceCube");
                    _blueSoundManager.PlayRandom();
                    Debug.Log("blue effect triggered");
                    _isIceBlockEffectEnabled = true;
                    _destinationMove = transform.position;
                    MoveRedo NewState = ScriptableObject.CreateInstance("MoveRedo") as MoveRedo;
                    NewState.ObjectInit(gameObject);
                    _levelManager.redoCommandHandler.AddCommand(NewState);
                    _levelManager.redoCommandHandler.TransitionToNewGameState();
                    _levelManager.EnqueueAction(() => { return MoveIceBlockToDestination(false); });
                    isPaintedByBrush = true;
                }

                break;
        }

        if (!paintWithBrush)
        {
            isPaintedByFeet = true;
        }

        return true;
    }

    public bool IsPaintable()
    {
        return isPaintable;
    }

    private void RevertEffect(Color colorToRevert, String newColor)
    {
        if (colorToRevert == GameConstants.red && newColor != "Blue")
        {
            if (NoUnmovableBlockAbove() && _destinationNeutral != transform.position)
            {
                //print("top empty");
                _levelManager.EnqueueAction(
                    () => { return RaiseLowerRedYellowBlockToDestination(_destinationNeutral, true); });
            }
            else if (_destinationNeutral != transform.position)
            {
                //print("top not empty, setting new destinations");
                _destinationNeutral = transform.position;
                _destinationDrop = _destinationNeutral + new Vector3(0, -1, 0);
                _destinationRaise = _destinationNeutral - new Vector3(0, -1, 0);
            }

            Debug.Log("reverting red");
        }
        else if (colorToRevert == GameConstants.green)
        {
            // Does nothing for now. Staying here in case we implement erase in the future.
            _destinationNeutral = transform.position;
            _destinationDrop = _destinationNeutral + new Vector3(0, -1, 0);
            _destinationRaise = _destinationNeutral - new Vector3(0, -1, 0);
            Debug.Log("reverting green");
        }
        else if (colorToRevert == GameConstants.yellow && newColor != "Blue")
        {
            if (NoBlockBelow() && _destinationNeutral != transform.position)
            {
                print("bottom empty");
                _levelManager.EnqueueAction(
                    () => { return RaiseLowerRedYellowBlockToDestination(_destinationNeutral, true); });
            }
            else if (_destinationNeutral != transform.position)
            {
                // print("bottom not empty, setting new destinations");
                _destinationNeutral = transform.position;
                _destinationDrop = _destinationNeutral + new Vector3(0, -1, 0);
                _destinationRaise = _destinationNeutral - new Vector3(0, -1, 0);
            }

            Debug.Log("reverting yellow");
        }
        else if (colorToRevert == GameConstants.blue)
        {
            //Make the block not pushable.
            Debug.Log("reverting blue");
            gameObject.layer = LayerMask.NameToLayer("Default");
            _destinationNeutral = transform.position;
            _destinationDrop = _destinationNeutral + new Vector3(0, -1, 0);
            _destinationRaise = _destinationNeutral - new Vector3(0, -1, 0);
            _isIceBlockEffectEnabled = false;
        }
    }

    private bool NoBlockBelow()
    {
        LayerMask mask = LayerMask.GetMask("Default");
        Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), Vector3.down, Color.blue, 120f);
        return !Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector3.down, 1, mask);
    }

    private bool NoUnmovableBlockAbove()
    {
        LayerMask mask = LayerMask.GetMask("Default");
        Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), Vector3.up, Color.red, 120f);
        bool noBlockAbove =
            !Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector3.up, out var hit, 1, mask);
        return noBlockAbove || (hit.collider.gameObject.TryGetComponent(out Ground ground) && ground.IsIceBlock());
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

    private IEnumerator GreenExtend(MoveRedo gameState)
    {
        // Extends platforms in wasd directions by 1 block
        Vector3 position = transform.position;
        Vector3 positionToCheck = position - new Vector3(0, 0.5f, 0);
        var growth_block = Resources.Load(path: "Blocks/NonColouredBlock");
        if (CanDown(positionToCheck))
        {
            GameObject newBlock = InstantiateNewBlock(growth_block, position, Vector3.back);
            gameState.AddGreenBlocksToRevert(newBlock);
        }

        if (CanUp(positionToCheck))
        {
            GameObject newBlock = InstantiateNewBlock(growth_block, position, Vector3.forward);
            gameState.AddGreenBlocksToRevert(newBlock);
        }

        if (CanLeft(positionToCheck))
        {
            GameObject newBlock = InstantiateNewBlock(growth_block, position, Vector3.left);
            gameState.AddGreenBlocksToRevert(newBlock);
        }

        if (CanRight(positionToCheck))
        {
            GameObject newBlock = InstantiateNewBlock(growth_block, position, Vector3.right);
            gameState.AddGreenBlocksToRevert(newBlock);
        }

        yield return null;
    }

    private GameObject InstantiateNewBlock(Object growth_block, Vector3 position, Vector3 direction)
    {
        GameObject newBlock = (GameObject) Instantiate(growth_block, position + direction,
            Quaternion.identity);
        Ground newGround = newBlock.GetComponent<Ground>();
        newGround.player = player;
        newGround.speed = speed;
        newGround.isPaintedByBrush = false;
        newGround.isPaintedByFeet = false;
        return newBlock;
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

    private new void OnMouseEnter()
    {
        if (!IsPaintable())
        {
            return;
        }
        // originalColour = Material.color;
        // Material.color = new Color(0.98f, 1f, 0.45f);
        HighlightForHoverover();
        _isMouseOver = true;
    }

    private new void OnMouseExit()
    {
        if (!IsPaintable())
        {
            return;
        }
        // Material.color = _paintedColour;
        UndoHighlight();
        _isMouseOver = false;
    }

    public bool IsIceBlock()
    {
        return _isIceBlockEffectEnabled;
    }
}