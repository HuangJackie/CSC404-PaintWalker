using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
using static GameConstants;
using Object = UnityEngine.Object;

public class Ground : Interactable, Paintable
{
    public GameObject player;
    public float speed;
    public string test; // For debugging by giving a block a specific name

    public bool isPaintedByFeet;
    public bool isPaintedByBrush;

    // TODO: Refactor _paintedColour with paintedColour
    // --> Requires re-entering values in Unity UI
    public Color _paintedColour;
    public bool isPaintable = true;

    public bool player_enter_from_north;
    public bool isWalkedOverVertially = false;
    public bool isWalkedOverHorizontally = false;

    private MoveRedo latestState;
    private LevelManager _levelManager;
    private UpdateUI _updateUI;
    private Player _player;
    private float _playerYPosition;
    private float _moveableObjYPosition;
    private bool _outOfPaintAudioAlreadyPlayed;
    private Animator animator;

    private bool _isMouseClicked;
    private bool _isMouseOver;

    private bool _isIceBlockEffectEnabled;
    public bool _isSliding;
    private Vector3 _directionToSlideTo;
    public Vector3 _destinationDrop;
    public Vector3 destinationNeutral;
    public Vector3 _destinationRaise;
    private Vector3 _destinationMove;
    private bool _isBlockMoving;

    public GameObject YellowSounds;
    public GameObject RedSounds;
    public GameObject BlueSounds;
    public GameObject GreenSounds;
    public GameObject NoPaintSound;

    public GameObject blue_model;
    public GameObject yellow_model;
    public GameObject green_model;
    public GameObject red_model;
    public GameObject base_model;
    public GameObject _cur_model;

    private SoundManager _yellowSoundManager = new SoundManager();
    private SoundManager _redSoundManager = new SoundManager();
    private SoundManager _blueSoundManager = new SoundManager();
    private SoundManager _greenSoundManager = new SoundManager();
    private SoundManager _pushIceBlockSoundManager = new SoundManager();
    private SoundManager _outOfPaintSoundManager = new SoundManager();

    private ObjectStorage objectStorage;
    private const float BlockMovementDelay = 0.1f;

    private new void Start()
    {
        _levelManager = FindObjectOfType<LevelManager>();
        objectStorage = FindObjectOfType<ObjectStorage>();
        objectStorage.AddBlock(this.gameObject);
        Material = GetComponentInChildren<Renderer>().material;
        originalColour = Material.color;
        _paintedColour = originalColour;
        paintedColour = Material.color;
        player_enter_from_north = false;
        player = GameObject.FindWithTag("Player");
        _player = player.GetComponent<Player>();
        animator = player.GetComponentInChildren<Animator>();
        _playerYPosition = player.transform.position.y;
        _cur_model = base_model;

        _isMouseOver = false;

        _isIceBlockEffectEnabled = false;
        _isSliding = false;
        _outOfPaintAudioAlreadyPlayed = false;
        _destinationDrop = transform.position + new Vector3(0, -1, 0);
        destinationNeutral = transform.position;
        _destinationRaise = transform.position - new Vector3(0, -1, 0);
        _destinationMove = transform.position;
        _directionToSlideTo = Vector3.zero;

        // Sounds

        YellowSounds = GameObject.Find("PaintingYellow");
        RedSounds = GameObject.Find("PaintingRed");
        BlueSounds = GameObject.Find("PaintingBlue");
        GreenSounds = GameObject.Find("PaintingGreen");
        NoPaintSound = GameObject.Find("OutOfPaint");

        _yellowSoundManager.SetAudioSources(YellowSounds.GetComponents<AudioSource>());
        _redSoundManager.SetAudioSources(RedSounds.GetComponents<AudioSource>());
        _blueSoundManager.SetAudioSources(BlueSounds.GetComponents<AudioSource>());
        _greenSoundManager.SetAudioSources(GreenSounds.GetComponents<AudioSource>());
        _pushIceBlockSoundManager.SetAudioSources(GetComponents<AudioSource>());
        _outOfPaintSoundManager.SetAudioSource(NoPaintSound.GetComponent<AudioSource>());

        base.Start();
    }

    void FixedUpdate()
    {
        if (!_player.isPlayerMoving)
        {
            _outOfPaintAudioAlreadyPlayed = false;
        }
    }

    void Update()
    {
        

        if (_levelManager.freezePlayer)
        {
            return;
        }

        if (_levelManager.GetCurrentlySelectedPaintClass().ToString() != _paintedColour.ToString() || !isPaintedByBrush)
        {
            _isMouseClicked = Input.GetButtonDown("Fire1");
            bool clickedUI = EventSystem.current.IsPointerOverGameObject();

            Vector3 horizontalPlayerPosition = new Vector3(player.transform.position.x, 0, player.transform.position.z);
            Vector3 horizontalBlockPosition =
                new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
            if (_isMouseOver && _isMouseClicked && !clickedUI &&
                Vector3.Distance(horizontalPlayerPosition, horizontalBlockPosition) < 3)
            {
                Paint(true);
            }
        }

        RaycastHit PlayerhitInfo;
        LayerMask mask = LayerMask.GetMask("Player");
        Vector3 spherePosition = transform.position + new Vector3(0, 1,0);
        float radius = 0.05f;
        Vector3 castDirection = Vector3.up;
        if (Physics.SphereCast(spherePosition, radius, castDirection, out PlayerhitInfo, 0.5f, mask))
        {
            if ((player_enter_from_north && !isWalkedOverVertially) ||
                (!player_enter_from_north && !isWalkedOverHorizontally))
            {
                GameObject footstepFX = GameObject.CreatePrimitive(PrimitiveType.Plane);
                footstepFX.transform.parent = gameObject.transform;
                footstepFX.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1.03f, gameObject.transform.position.z);
                if (player_enter_from_north)
                {
                    footstepFX.transform.rotation = Quaternion.Euler(0, 0, 0);
                    isWalkedOverVertially = true;
                }
                else
                {
                    footstepFX.transform.rotation = Quaternion.Euler(0, 90, 0);
                    isWalkedOverHorizontally = true;
                }
                footstepFX.transform.localScale = footstepFX.transform.localScale * 0.1f;
                Destroy(footstepFX.GetComponent<MeshCollider>());
                Material footstep = Resources.Load("Materials/FootSteps", typeof(Material)) as Material;
                footstepFX.GetComponent<Renderer>().material = footstep;
                objectStorage.AddFootPrint(footstepFX);
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
        if (!isPushed)
        {
            yield return new WaitForSeconds(BlockMovementDelay * 2);
        }

        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Player");
        Color intialColor = Material.color;

        // Needs to match the highlight given in Interactable.cs
        float amountToTint = 0.2f;
        Color initialTintedColor = new Color(
            tintColour(intialColor.r, GameConstants.SELECTION_R, amountToTint),
            tintColour(intialColor.g, GameConstants.SELECTION_G, amountToTint),
            tintColour(intialColor.b, GameConstants.SELECTION_B, amountToTint));
        
        bool stillMoving = true;
        while (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector3.up, out hit, 1, mask))
        {
            yield return null;
            if (intialColor != Material.color && initialTintedColor != Material.color)
            {
                yield break;
            }
        }
        
        if (isPushed)
        {
            yield return new WaitForSeconds(0.3f);
        }

        while (stillMoving)
        {
            float distance = Vector3.Distance(transform.position, _destinationMove);
            while (distance > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, _destinationMove, speed * 2f * Time.deltaTime
                );
                yield return null;

                distance = Vector3.Distance(transform.position, _destinationMove);
            }

            if (distance < 0.01f)
            {
                transform.position = _destinationMove;
            }

            stillMoving = ReinitializeIceBlockMovement(isPushed);
            if (!stillMoving)
            {
                _isSliding = false;
            }
        }
    }

    public bool IsMoving()
    {
        return _isBlockMoving;
    }

    IEnumerator RaiseLowerRedYellowBlockToDestination(Vector3 destination, bool reverting = false)
    {
        yield return new WaitForSeconds(BlockMovementDelay);

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
                _isBlockMoving = true;
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
                    _isBlockMoving = false;
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
                if (blockPathIsBlocked && destination != destinationNeutral)
                {
                    //print("changing to neutral dest since path blocked");
                    destination = destinationNeutral;
                    blockPathIsBlocked = false;
                }

                _isBlockMoving = true;

                transform.position = Vector3.Lerp(
                    transform.position, destination, speed * 1.6f * Time.deltaTime
                );
                if (Vector3.Distance(transform.position, destination) <= 0.01f)
                {
                    _isBlockMoving = false;
                    transform.position = destination;
                    _destinationDrop = transform.position + new Vector3(0, -1, 0);
                    destinationNeutral = transform.position;
                    _destinationRaise = transform.position - new Vector3(0, -1, 0);
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
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.transform.position.z - this.transform.position.z != 0)
            {
                player_enter_from_north = true;
            }
            else if (other.transform.position.x - this.transform.position.x != 0)
            {
                player_enter_from_north = false;
            }
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
        if (Physics.Raycast(other.gameObject.transform.position, other.gameObject.transform.forward, out RayHit) &&
            RayHit.collider.gameObject == this.gameObject)
        {
            _isOnSameLevelAsPlayer = true;
        }

        bool shouldMoveIceBlock = _isIceBlockEffectEnabled &&
                                  _isOnSameLevelAsPlayer &&
                                  other.gameObject.CompareTag("Player") &&
                                  dir != Vector3.negativeInfinity;

        if (shouldMoveIceBlock)
        {
            _player.animation_update("push", true);
            _player.animation_update("walk", false);
            _player.animation_update("paint", false);
            _isSliding = true;
            _player._isPushing = true;
            _player._pushTimer = 0.35f;
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

                StartCoroutine(MoveIceBlockToDestination(true));
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
            if (!(isPaintedByBrush || isPaintedByFeet) && !_outOfPaintAudioAlreadyPlayed && _player.isPlayerMoving)
            {
                _outOfPaintSoundManager.PlayAudio();
                _outOfPaintAudioAlreadyPlayed = true;
            }

            return false;
        }

        if ((isPaintedByBrush || isPaintedByFeet) && !paintWithBrush)
        {
            return false;
        }

        Paints currentlySelectedPaint = _levelManager.GetCurrentlySelectedPaint();

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
            case Paints.Red:
                Material.color = GameConstants.Red;
                _paintedColour = Material.color;
                paintedColour = Material.color;
                _levelManager.DecreasePaint(Paints.Red, 1);

                if (paintWithBrush)
                {
                    _player.animation_update("paint", true);
                    base_model.SetActive(false);
                    red_model.SetActive(true);
                    _cur_model = red_model;

                    if (NoBlockBelow())
                    {
                        _redSoundManager.PlayRandom();
                        StartCoroutine(RaiseLowerRedYellowBlockToDestination(_destinationDrop));
                        isPaintedByBrush = true;
                    }
                }
                else
                {
                    AddSparkleFX();
                }
                break;

            case Paints.Green:
                Material.color = GameConstants.Green;
                _paintedColour = Material.color;
                paintedColour = Material.color;
                _levelManager.DecreasePaint(Paints.Green, 1);

                if (paintWithBrush)
                {
                    _player.animation_update("paint", true);
                    _greenSoundManager.PlayRandom();

                    MoveRedo NewState = ScriptableObject.CreateInstance("MoveRedo") as MoveRedo;
                    NewState.ObjectInit(gameObject);
                    _levelManager.redoCommandHandler.AddCommand(NewState);
                    _levelManager.redoCommandHandler.TransitionToNewGameState();

                    StartCoroutine(GreenExtend(NewState));
                    isPaintedByBrush = true;
                    base_model.SetActive(false);
                    green_model.SetActive(true);
                    _cur_model = green_model;
                }
                else
                {
                    AddSparkleFX();
                }
                break;

            case Paints.Yellow:
                Material.color = GameConstants.Yellow;
                _paintedColour = Material.color;
                paintedColour = Material.color;
                _levelManager.DecreasePaint(Paints.Yellow, 1);

                if (paintWithBrush)
                {
                    _player.animation_update("paint", true);
                    base_model.SetActive(false);
                    yellow_model.SetActive(true);
                    _cur_model = yellow_model;

                    if (NoUnmovableBlockAbove())
                    {
                        _yellowSoundManager.PlayRandom();
                        StartCoroutine(RaiseLowerRedYellowBlockToDestination(_destinationRaise));
                        isPaintedByBrush = true;
                    }
                }
                else
                {
                    AddSparkleFX();
                }
                break;

            case Paints.Blue:
                Material.color = GameConstants.Blue;
                _paintedColour = Material.color;
                paintedColour = Material.color;
                _levelManager.DecreasePaint(Paints.Blue, 1);

                if (paintWithBrush)
                {
                    _player.animation_update("paint", true);
                    gameObject.layer = LayerMask.NameToLayer("IceCube");
                    _blueSoundManager.PlayRandom();
                    _isIceBlockEffectEnabled = true;
                    _destinationMove = transform.position;

                    MoveRedo NewState = ScriptableObject.CreateInstance("MoveRedo") as MoveRedo;
                    NewState.ObjectInit(gameObject);
                    _levelManager.redoCommandHandler.AddCommand(NewState);
                    _levelManager.redoCommandHandler.TransitionToNewGameState();
                    StartCoroutine(MoveIceBlockToDestination(false));

                    isPaintedByBrush = true;
                    base_model.SetActive(false);
                    blue_model.SetActive(true);
                    _cur_model = blue_model;
                }
                else
                {
                    AddSparkleFX();
                }
                break;
        }

        if (!paintWithBrush)
        {
            isPaintedByFeet = true;
        }

        ReinitializeMaterialColours();

        return true;
    }

    public bool IsPaintable()
    {
        return isPaintable;
    }

    private void RevertEffect(Color colorToRevert, Paints newPaintType)
    {
        GameObject new_model;

        switch (newPaintType)
        {
            case (Paints.Blue):
                new_model = blue_model;
                break;
            case (Paints.Yellow):
                new_model = yellow_model;
                break;
            case (Paints.Green):
                new_model = green_model;
                break;
            case (Paints.Red):
                new_model = red_model;
                break;
            default:
                new_model = base_model;
                break;
        }

        _cur_model.SetActive(false);
        new_model.SetActive(true);
    }

    public void AddSparkleFX()
    {
        GameObject sparklesFX = GameObject.CreatePrimitive(PrimitiveType.Plane);
        sparklesFX.transform.parent = gameObject.transform;
        sparklesFX.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1.02f, gameObject.transform.position.z);
        sparklesFX.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 4) * 90, 0);
        sparklesFX.transform.localScale = sparklesFX.transform.localScale * 0.1f;
        Destroy(sparklesFX.GetComponent<MeshCollider>());
        Material sparkles = Resources.Load("Materials/Sparkles", typeof(Material)) as Material;
        sparklesFX.GetComponent<Renderer>().material = sparkles;
        objectStorage.AddSparkle(sparklesFX);
    }

    public void UpdateModel(GameObject new_model)
    {
        _cur_model.SetActive(false);
        new_model.SetActive(true);
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