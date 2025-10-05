using System;
using System.Linq;
using UnityEngine;

public class PlayerScript : BasicLivingEntity
{
    public static PlayerScript Instance;
    
    [SerializeField]
    private AudioClip[] playerWaterSounds;
    [SerializeField]
    private AudioClip[] playerLavaSounds;
    public AudioClip[] playerFallSounds;
    
    public bool isAlive = true;
    private bool _postMoveAndTickEnd;
    private float _moveIntervalTimer;
    
    /// <summary>
    /// Determine whether Player can move in a certain direction.
    /// </summary>
    /// <param name="playerMoveDir">Player move direction</param>
    /// <returns></returns>
    private bool LivingEntityMoveCondition(Vector3 playerMoveDir)
    {
        if (!isAlive) return false;
        
        var allMoveables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IMoveable>();

        foreach (var moveable in allMoveables)
        {
            if (moveable.IsLivingEntityPushing(playerMoveDir))
            {
                if (moveable.CanMoveOrRedirect(ref playerMoveDir))
                {
                    return true;
                }
                return false;
            }
        }

        // print("Returning true for normal movement");
        return true;
    }

    public override bool IsLivingEntityPushing(Vector3 moveDirection)
    {
        // Return false if you don't want to have infinite loop 👀. Well actually since player is
        // an instance of IMoveable, this method bawaan is not true.
        return false;
    }

    /// <summary>
    /// Start a PreStartTickConditions player movement check.
    /// </summary>
    /// <param name="moveDir">Player move dir</param>
    /// <returns>Check succeed</returns>
    public override bool CanMoveOrRedirect(ref Vector3 moveDir)
    {
        IsNextTickMoveScheduled = true; // Setting this to true so that the CanMove()'s PreStartTick()'s IsPlayerPushing() method works.
        
        // First check if player hits a pinball and needs a moveDir redirect
        moveDir = PinballGlobalScript.RedirectMoveFromPinballIfAny(gameObject, moveDir);

        // Thirdly check if player can move entities, more detailed see method docs
        var result = LivingEntityMoveCondition(moveDir);
        
        // Custom check, just like GameLogic's IsSpaceAvailable but modified so that we can go to treasure (other living entities can't).
        if (result && Physics2D.OverlapPoint(MovePoint.transform.position + moveDir, LayerAllowMovement))
        {
            // Lastly Check if player is not colliding with an IMoveable or treasure
            var colide = Physics2D.OverlapPoint(MovePoint.transform.position + moveDir, LayerStopsMovement);
            if (colide && !colide.TryGetComponent<IMoveable>(out _) && !colide.TryGetComponent<TreasureScript>(out _))
            {
                return false;
            }
            return true;
        }
        IsNextTickMoveScheduled = false;
        
        return false;
    }
    
    public override void OnStartTick(Vector3 playerMoveDir)
    {
        // print("Player start tick");
        if (IsNextTickDestroyScheduled)
        {
            GameLogic.PlayAudioClipRandom(destroySounds);
            gameObject.SetActive(false);
        }
        
        DoScheduledMove();
        StartTickPosition = transform.position;
    }

    public override void OnEndTick()
    {
        base.OnEndTick();
        
        // Check if boulder is on water/lava
        bool onWater = false;
        bool onLava = false;
        
        Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position);
        foreach (var col in colliders)
        {
            if (col.CompareTag("Water"))
            {
                onWater = true;
            }
            else if (col.CompareTag("Lava"))
            {
                onLava = true;
            }
            else if (col.CompareTag("Floating Crate") || col.CompareTag("Floating Boulder") || col.CompareTag("Ice"))
            {
                onWater = false;
                onLava = false;
                break;
            }
        }

        if (onWater)
        {
            destroySounds = playerWaterSounds;
            IsNextTickDestroyScheduled = true;
            GameLogic.Instance.GameOver("Player lupa kalau dia ga bisa berenang");
        }
        else if (onLava)
        {
            destroySounds = playerLavaSounds;
            IsNextTickDestroyScheduled = true;
            GameLogic.Instance.GameOver("Player mencoba jalan di lava");
        }
    }

    public override void PostEndTick()
    {
        base.PostEndTick();
        SpriteRenderer.sortingOrder += 2;
    }

    private void Awake()
    {
        Instance = this;
    }

    public override void Update()
    {
        base.Update();

        if (_moveIntervalTimer > 0)
        {
            _moveIntervalTimer -= Time.deltaTime;
        }
        
        if (IsStationary() && !GameLogic.Instance.waitingForAllStartTickToFinish && !GameLogic.Instance.waitingForAllEndTickToFinish)
        {
            if (_postMoveAndTickEnd)
            {
                // Start end tick after moving.
                // GameLogic.Instance.EndTick();
                _postMoveAndTickEnd = false;
                _moveIntervalTimer = 0.05f;
            }

            var moveDirectionInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (!_postMoveAndTickEnd && ((Math.Abs(moveDirectionInput.x) == 1 && moveDirectionInput.y == 0) ||
                               (Math.Abs(moveDirectionInput.y) == 1 && moveDirectionInput.x == 0)
                               ) && !GameLogic.Instance.waitingForAllEndTickToFinish && _moveIntervalTimer <= 0)
            {
                if (CanMoveOrRedirect(ref moveDirectionInput))
                {
                    ScheduleMove(moveDirectionInput);
                    GameLogic.Instance.StartTick(moveDirectionInput);
                }                _postMoveAndTickEnd = true;
            }
        }
    }
}
