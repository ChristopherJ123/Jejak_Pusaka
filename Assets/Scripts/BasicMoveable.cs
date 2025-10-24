using JetBrains.Annotations;
using UnityEngine;

public class BasicMoveable : BasicTickable, IMoveable
{
    private Animator _animator;
    private static readonly int IsMoving = Animator.StringToHash("IsWalking");
    private static readonly int X = Animator.StringToHash("X");
    private static readonly int Y = Animator.StringToHash("Y");
    protected LoopingAudioPlayer LoopingAudioPlayer;
    
    protected LayerMask LayerAllowMovement;
    protected LayerMask LayerStopsMovement;
    protected Transform MovePoint;
    
    [SerializeField]
    public AudioClip[] moveSounds;
    [SerializeField]
    protected AudioClip movingSound;
    [SerializeField]
    protected AudioClip[] hitSounds;
    
    public Vector3 StartTickPosition { get; set; }
    public Vector3 EndTickPosition { get; set; }
    public Vector3 LastMoveDir { get; set; }
    public Vector3 ScheduledMoveDir { get; set; }
    public bool IsHitSoundScheduled { get; set; }
    public bool IsIceMoveable { get; set; }
    public bool IsPinballMoveable { get; set; }
    public bool IsSlopeMoveable { get; set; }
    public bool IsPinballSlopeMoveable { get; set; }
    public bool IsBoulderSlopeMoveable { get; set; }
    
    public override void Deactivate()
    {
        base.Deactivate();
        Destroy(MovePoint.gameObject);
    }

    /// <summary>
    /// Tickable object is idle.
    /// </summary>
    /// <returns>bool</returns>
    public virtual bool IsStationary()
    {
        return Vector3.Distance(transform.position, MovePoint.position) == 0;
    }

    public virtual bool IsLivingEntityPushing(out BasicLivingEntity livingEntity)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1, LayerStopsMovement);
        foreach (var col in colliders)
        {
            if (col.TryGetComponent<BasicLivingEntity>(out var entity))
            {
                livingEntity = entity;
                return entity.IsNextTickMoveScheduled &&
                       entity.transform.position + entity.ScheduledMoveDir == transform.position;
            }
        }
        livingEntity = null;
        return false;
    }

    public virtual bool CanMoveOrRedirect(ref Vector3 moveDir)
    {
        // 1. check if there is a pinball move redirect
        moveDir = PinballGlobalScript.RedirectMoveFromPinballIfAny(gameObject, moveDir);
        
        // 2. check if there is a slope move redirect
        moveDir = SlopeGlobalScript.RedirectMoveFromSlopeIfAny(gameObject, moveDir);
        
        // print($"{transform.name} moveDir then={moveDir} from={transform.position}");
        
        // 3. check if there is a boulder slope move redirect
        moveDir = SlopeGlobalScript.RedirectMoveFromBoulderIfAny(gameObject, moveDir);
        
        // print($"{transform.name} moveDir now={moveDir} going to={transform.position + moveDir}");
        
        // 4. check if there is a pinball slope move redirect
        moveDir = SlopeGlobalScript.RedirectMoveFromPinballIfAny(gameObject, moveDir);
        
        return GameLogic.IsSpaceAvailable(transform.position + moveDir);
    }
    
    public void ScheduleMove(Vector3 moveDir, bool doExtraTickLoop = false)
    {
        IsTriggered = true;
        IsNextTickMoveScheduled = true;
        ScheduledMoveDir = moveDir;
        if (doExtraTickLoop) DoExtraTickLoop = true;
    }
    
    public virtual void DoScheduledMove()
    {
        if (IsNextTickMoveScheduled)
        {
            if (!IsHitSoundScheduled) GameLogic.PlayAudioClipRandom(triggerSounds);
            // if (movingSound) print($"Playing {transform.name} moving sound {movingSound.name} with moveDir {ScheduledMoveDir}");
            LoopingAudioPlayer.PlayAudioClipLoop(movingSound);
            // var scheduledMoveDir = ScheduledMoveDir;
            // if (CanMoveOrRedirect(ref scheduledMoveDir))
            // {
            Move(ScheduledMoveDir);
            // }
            LastMoveDir = ScheduledMoveDir;
            IsHitSoundScheduled = true;
        } else if(IsHitSoundScheduled)
        {
            Collider2D hitCollider;
            hitCollider = Physics2D.OverlapPoint(transform.position + LastMoveDir, LayerStopsMovement);
            
            // // Hardcode for now
            // if (transform.CompareTag("Boulder"))
            // {
            //     if (Mathf.Approximately(Vector3.Distance(Vector3.zero, LastMoveDir), Vector3.Distance(Vector3.zero, new Vector3(1, 1, 0))))
            //     {
            //         hitCollider = Physics2D.OverlapPoint(transform.position + Vector3.down, LayerStopsMovement);
            //         if (hitCollider.CompareTag("Player"))
            //         {
            //             LastMoveDir = Vector3.down;
            //         }
            //         else
            //         {
            //             hitCollider = Physics2D.OverlapPoint(transform.position + LastMoveDir, LayerStopsMovement);
            //         }
            //     }
            // }
            
            if (hitCollider) OnHit(hitCollider.gameObject);
            else OnHit();
        }
    }

    public virtual void Move(Vector3 moveDir)
    {
        // DateTime now = DateTime.Now;
        // print($"Moving {transform.name} Current Time: {now:HH:mm:ss.fff}");
        if (_animator)
        {
            _animator.SetBool(IsMoving, true);
            _animator.SetFloat(X, moveDir.x);
            _animator.SetFloat(Y, moveDir.y);
        }
        MovePoint.transform.position += moveDir;
        LastMoveDir = moveDir;
        IsStartTicking = true;
    }

    public virtual void PlayMoveSound()
    {
        if (moveSounds.Length > 0) print($"Playing move sounds for {transform.name} with moveDir {LastMoveDir}");
        GameLogic.PlayAudioClipRandom(moveSounds);
    }

    /// <summary>
    /// Called on Start Tick when there is a hit.
    /// </summary>
    protected virtual void OnHit()
    {
        // print("playing hit sound");
        GameLogic.PlayAudioClipRandom(hitSounds);
        IsHitSoundScheduled = false;
        LoopingAudioPlayer.StopLooping();
    }
    
    protected virtual void OnHit(GameObject hitObject)
    {
        // print("playing hit sound");
        GameLogic.PlayAudioClipRandom(hitSounds);
        IsHitSoundScheduled = false;
        LoopingAudioPlayer.StopLooping();
    }
    
    public override void OnStartTick()
    {
        base.OnStartTick();
        
        DoScheduledMove();
        
        if (IsLivingEntityPushing(out var livingEntity) && !IsNextTickMoveScheduled)
        {
            var scheduledMoveDir = livingEntity.ScheduledMoveDir;
            if (CanMoveOrRedirect(ref scheduledMoveDir))
            {
                PlayMoveSound();
                Move(scheduledMoveDir);
            }
        }
    }

    public override void OnPostStartTick()
    {
        base.OnPostStartTick();
        StartTickPosition = transform.position;

        // Turn off DoExtraTickLoop after IsNextTickScheduled is done (On the next tick)
        if (!IsNextTickMoveScheduled)
        {
            if (DoExtraTickLoop) DoExtraTickLoop = false;
        }
        // Finished doing scheduled move from OnStartTick
        // Called here so that other Tickable that depends on this variable won't get unexpected result from
        // OnStartTick()'s random ordering.
        // Reset IsNextTickMoveScheduled.
        if (IsNextTickMoveScheduled)
        {
            IsNextTickMoveScheduled = false;
            ScheduledMoveDir = Vector3.zero;
        }
    }

    public override void OnEndTick()
    {
        base.OnEndTick();
    }

    public override void OnPostEndTick()
    {
        base.OnPostEndTick();
        SpriteRenderer.sortingOrder = -(int)(transform.position.y * 10);

        // Ditaruh sini soalnya agar EndTickPosition ini kalau di panggil di OnEndTick() dia akan pasti reference
        // ke EndTickPosition EndTick sebelumnya, daripada setengah2 bisa random kalau ditaruh di OnEndTick()
        EndTickPosition = transform.position;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        LoopingAudioPlayer = gameObject.AddComponent<LoopingAudioPlayer>();
        _animator = GetComponent<Animator>();
        LayerAllowMovement = GameLogic.LayerAllowsMovement;
        LayerStopsMovement = GameLogic.LayerBlocksMovement;
        
        IsIceMoveable = true;
        IsPinballMoveable = true;
        IsSlopeMoveable = false;
        IsPinballSlopeMoveable = false;
        IsBoulderSlopeMoveable = false;
        
        MovePoint = transform.GetChild(0);
        MovePoint.parent = null;
    }

    public virtual void Update()
    {
        // Animation moving
        if (IsStartTicking)
        {
            transform.position = Vector3.MoveTowards(transform.position, MovePoint.position, GameLogic.Instance.speed * Time.deltaTime);
            if (IsStationary())
            {
                if (_animator)
                {
                    _animator.SetBool(IsMoving, false);
                }
                IsStartTicking = false;
            }     
        }
    }
}