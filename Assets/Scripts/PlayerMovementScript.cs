using System;
using System.Linq;
using UnityEngine;

public class PlayerMovementScript : BasicMoveable
{
    public static PlayerMovementScript Instance;
    public float moveSpeed = 5f;
    
    private bool _postMoveAndTickEnd;
    private float _moveIntervalTimer;
    
    /// <summary>
    /// Determine whether Player can move in a certain direction.
    /// </summary>
    /// <param name="playerMoveDir">Player move direction</param>
    /// <returns></returns>
    private bool PlayerMoveCondition(Vector3 playerMoveDir)
    {
        var allMoveables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IMoveable>();

        foreach (var moveable in allMoveables)
        {
            if (moveable.IsPlayerPushing(playerMoveDir))
            {
                if (moveable.CanMove(playerMoveDir))
                {
                    return true;
                }
                return false;
            }
        }

        // print("Returning true for normal movement");
        return true;
    }

    public override bool IsPlayerPushing(Vector3 moveDirection)
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
    public override bool CanMove(Vector3 moveDir)
    {
        IsNextTickScheduled = true; // Setting this to true so that the CanMove()'s PreStartTick()'s IsPlayerPushing() method works.
        var result = PlayerMoveCondition(moveDir);
        if (result && Physics2D.OverlapPoint(MovePoint.transform.position + moveDir, LayerAllowMovement))
        {
            return true;
        }
        IsNextTickScheduled = false;
        return false;
    }
    
    public override void OnStartTick(Vector3 playerMoveDir)
    {
        // print("Player start tick");
        DoScheduledMove();
        StartTickPosition = transform.position;
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
                _moveIntervalTimer = 0.1f;
            }

            var moveDirectionInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (!_postMoveAndTickEnd && ((Math.Abs(moveDirectionInput.x) == 1 && moveDirectionInput.y == 0) ||
                               (Math.Abs(moveDirectionInput.y) == 1 && moveDirectionInput.x == 0)
                               ) && !GameLogic.Instance.waitingForAllEndTickToFinish && _moveIntervalTimer <= 0)
            {
                if (CanMove(moveDirectionInput))
                {
                    ScheduleMove(moveDirectionInput);
                    GameLogic.Instance.StartTick(moveDirectionInput);
                }
                _postMoveAndTickEnd = true;
            }
        }
    }
}
