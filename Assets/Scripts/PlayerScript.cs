using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerScript : BasicLivingEntity
{
    public static PlayerScript Instance;
    public bool scheduledMoveByUser;
    
    [SerializeField]
    private AudioClip[] playerWaterSounds;
    [SerializeField]
    private AudioClip[] playerLavaSounds;
    public AudioClip[] playerFallSounds;
    [SerializeField]
    private AudioClip[] playerMummySounds;
    
    private bool _postMoveAndTickEnd;
    private float _moveIntervalTimer;

    public override bool IsLivingEntityPushing(out BasicLivingEntity livingEntity)
    {
        // Return false if you don't want to have infinite loop 👀. Well actually since player is
        // an instance of IMoveable, this method bawaan is not true.
        livingEntity = null;
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
            var colliders = Physics2D.OverlapPointAll(MovePoint.transform.position + moveDir, LayerStopsMovement);
            foreach (var overlappingCollider in colliders)
            {
                if (overlappingCollider && !overlappingCollider.TryGetComponent<IMoveable>(out _) && !overlappingCollider.TryGetComponent<TreasureScript>(out _) && !overlappingCollider.CompareTag("MovePoint"))
                {
                    return false;
                }
            }
            return true;
        }
        IsNextTickMoveScheduled = false;
        
        return false;
    }
    
    public override void OnStartTick()
    {
        // print("Player start tick");
        if (IsNextTickDestroyScheduled)
        {
            GameLogic.PlayAudioClipRandom(destroySounds);
            gameObject.SetActive(false);
        }

        var scheduledMoveDir = ScheduledMoveDir;
        if (CanMoveOrRedirect(ref scheduledMoveDir))
        {
            ScheduledMoveDir = scheduledMoveDir;
            DoScheduledMove();
        }
        StartTickPosition = transform.position;
    }

    public override void OnEndTick()
    {
        base.OnEndTick();
        if (scheduledMoveByUser) scheduledMoveByUser = false;
        
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

    public override void OnPostEndTick()
    {
        base.OnPostEndTick();
        SpriteRenderer.sortingOrder += 2;
        
        // Put here because mummy can be killed in OnEndTick() so no racing condition
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (var monster in colliders)
        {
            if (monster.CompareTag("Mummy") && monster.TryGetComponent<MummyScript>(out var mummy) && mummy.IsAlive)
            {
                GameLogic.PlayAudioClipRandom(playerMummySounds);
                GameLogic.Instance.GameOver("Player terciduk oleh mummy");
            }
        }
    }

    public override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public override void Update()
    {
        base.Update();

        if (_moveIntervalTimer > 0)
        {
            _moveIntervalTimer -= Time.deltaTime;
        }
        
        if (IsAlive && IsStationary() && !GameLogic.Instance.waitingForAllStartTickToFinish && !GameLogic.Instance.waitingForAllEndTickToFinish)
        {
            if (_postMoveAndTickEnd)
            {
                // Start end tick after moving.
                // GameLogic.Instance.EndTick();
                _postMoveAndTickEnd = false;
                _moveIntervalTimer = 0.00f;
            }
            
            var moveDirectionInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (!_postMoveAndTickEnd && ((Math.Abs(moveDirectionInput.x) == 1 && moveDirectionInput.y == 0) ||
                               (Math.Abs(moveDirectionInput.y) == 1 && moveDirectionInput.x == 0)
                               ) && !GameLogic.Instance.waitingForAllEndTickToFinish && _moveIntervalTimer <= 0)
            {
                // print("CAN MOVE");
                ScheduleMove(moveDirectionInput);
                scheduledMoveByUser = true;
                GameLogic.Instance.StartTick();
                _postMoveAndTickEnd = true;
            }
        }
    }
}
