using UnityEngine;

public class BasicMoveable : BasicTickable, IMoveable
{
    protected LayerMask LayerAllowMovement;
    protected LayerMask LayerStopsMovement;
    protected Transform MovePoint;
    
    public virtual Vector3 StartTickPosition { get; set; }
    public virtual Vector3 EndTickPosition { get; set; }
    public virtual Vector3 LastMoveDir { get; set; }
    public virtual Vector3 ScheduledMoveDir { get; set; }
    
    public bool IsIceMoveable { get; set; }
    public bool IsPinballMoveable { get; set; }
    public bool IsSlopeMoveable { get; set; }
    
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
        return PlayerScript.Instance.IsNextTickScheduled &&
               PlayerScript.Instance.transform.position + moveDirection == transform.position;
    }

    public virtual bool CanMoveOrRedirect(ref Vector3 moveDir)
    {
        // First check if there is a pinball move redirect
        moveDir = PinballGlobalScript.MoveRedirectFromPinballIfAny(gameObject, moveDir);
        
        // Second check if there is a slope move redirect
        moveDir = SlopeGlobalScript.MoveRedirectFromSlopeIfAny(gameObject, moveDir);
        
        // Thirdly check if it is out of bounds (not touching any Tile Layer)
        if (Physics2D.OverlapPoint(MovePoint.transform.position + moveDir, LayerAllowMovement))
        {
            // Lastly it checks if it is not colliding with another Collision Layer
            if (!Physics2D.OverlapPoint(MovePoint.transform.position + moveDir, LayerStopsMovement))
            {
                return true;
            }        
        }
        return false;
    }

    public void ScheduleMove(Vector3 moveDir, bool doExtraTickLoop = false)
    {
        IsTriggered = true;
        IsNextTickScheduled = true;
        ScheduledMoveDir = moveDir;
        if (doExtraTickLoop) DoExtraTickLoop = true;
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
        // DateTime now = DateTime.Now;
        // print($"Moving {transform.name} Current Time: {now:HH:mm:ss.fff}");
        MovePoint.transform.position += moveDir;
        LastMoveDir = moveDir;
        IsStartTicking = true;
    }
    public override void OnStartTick(Vector3 playerMoveDir)
    {
        base.OnStartTick(playerMoveDir);
        
        DoScheduledMove();
        
        if (!IsNextTickScheduled && IsPlayerPushing(playerMoveDir))
        {
            if (CanMoveOrRedirect(ref playerMoveDir))
            {
                Move(playerMoveDir);
            }
        }
    }

    public override void PostStartTick(Vector3 playerMoveDir)
    {
        base.PostStartTick(playerMoveDir);
        StartTickPosition = transform.position;

        // Turn off DoExtraTickLoop after IsNextTickScheduled is done (On the next tick)
        if (!IsNextTickScheduled)
        {
            if (DoExtraTickLoop) DoExtraTickLoop = false;
        }
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
        SpriteRenderer.sortingOrder = -(int)(transform.position.y * 100) + 10;

        // Ditaruh sini soalnya agar EndTickPosition ini kalau di panggil di OnEndTick() dia akan pasti reference
        // ke EndTickPosition EndTick sebelumnya, daripada setengah2 bisa random kalau ditaruh di OnEndTick()
        EndTickPosition = transform.position;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        LayerAllowMovement = LayerMask.GetMask("Tile");
        LayerStopsMovement = LayerMask.GetMask("Collision");
        
        IsIceMoveable = true;
        IsPinballMoveable = true;
        IsSlopeMoveable = false;
        
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