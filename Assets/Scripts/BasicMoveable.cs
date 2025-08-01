using UnityEngine;

public class BasicMoveable : BasicTickable, IMoveable
{
    protected LayerMask LayerStopsMovement;
    protected Transform MovePoint;
    
    public virtual Vector3 StartTickPosition { get; set; }
    public virtual Vector3 EndTickPosition { get; set; }
    public virtual Vector3 LastMoveDir { get; set; }
    public virtual Vector3 ScheduledMoveDir { get; set; }
    
    /// <summary>
    /// Tickable object is idle.
    /// </summary>
    /// <returns>bool</returns>
    public virtual bool IsStationary()
    {
        return Vector3.Distance(transform.position, MovePoint.position) == 0;
    }

    public virtual bool IsPlayerPushing(Vector3 moveDirection)
    {
        return PlayerMovementScript.Instance.IsNextTickScheduled &&
               PlayerMovementScript.Instance.transform.position + moveDirection == transform.position;
    }

    public virtual bool CanMove(Vector3 moveDir)
    {
        if (!Physics2D.OverlapPoint(MovePoint.transform.position + moveDir, LayerStopsMovement))
        {
            return true;
        }
        return false;
    }

    public virtual void ScheduleMove(Vector3 moveDir)
    {
        IsNextTickScheduled = true;
        ScheduledMoveDir = moveDir;
    }

    public virtual void DoScheduledMove()
    {
        if (IsNextTickScheduled)
        {
            Move(ScheduledMoveDir);
            LastMoveDir = ScheduledMoveDir;
            ScheduledMoveDir = Vector3.zero;
        }
    }

    public virtual void Move(Vector3 moveDir)
    {
        MovePoint.transform.position += moveDir;
        LastMoveDir = moveDir;
        IsStartTicking = true;
    }
    public override void OnStartTick(Vector3 playerMoveDir)
    {
        base.OnStartTick(playerMoveDir);
        DoScheduledMove();
        
        if (IsPlayerPushing(playerMoveDir))
        {
            if (CanMove(playerMoveDir))
            {
                Move(playerMoveDir);
            }
        }
    }

    public override void PostStartTick(Vector3 playerMoveDir)
    {
        base.PostStartTick(playerMoveDir);
        StartTickPosition = transform.position;

        // Finished doing scheduled move from OnStartTick
        // Called here so that other Tickable that depends on this variable won't get unexpected result from
        // OnStartTick()'s random ordering.
        if (IsNextTickScheduled)
        {
            IsNextTickScheduled = false;
        }
    }

    public override void OnEndTick()
    {
        base.OnEndTick();
    }

    public override void PostEndTick()
    {
        base.PostEndTick();
        EndTickPosition = transform.position;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        LayerStopsMovement = LayerMask.GetMask("Collision");
        
        MovePoint = transform.GetChild(0);
        MovePoint.parent = null;
    }

    public virtual void Update()
    {
        // Animation moving
        if (IsStartTicking)
        {
            transform.position = Vector3.MoveTowards(transform.position, MovePoint.position, 5f * Time.deltaTime);
            if (IsStationary())
            {
                IsStartTicking = false;
            }     
        }
    }
}