using System;
using UnityEngine;

public class PlayerMovementScript : BasicMoveable
{
    public static PlayerMovementScript Instance;
    public float moveSpeed = 5f;
    
    private bool _postMoveAndTickEnd;
    private float _moveIntervalTimer;

    /// <summary>
    /// Start a PreStartTickConditions player movement check.
    /// </summary>
    /// <param name="moveDirection">Player move dir</param>
    /// <returns>Check succeed</returns>
    public override bool CanMove(Vector3 moveDirection)
    {
        IsNextTickScheduled = true; // Setting this to true so that the CanMove()'s PreStartTick()'s IsPlayerPushing() method works.
        var result = GameLogic.Instance.PlayerMoveCondition(moveDirection);
        if (result)
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
