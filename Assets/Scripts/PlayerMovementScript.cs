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
        return GameLogic.Instance.PlayerMoveCondition(moveDirection);
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
                IsNextTickScheduled = true; // Setting this to true so that the CanMove()'s PreStartTick()'s IsPlayerPushing() method works.
                if (CanMove(moveDirectionInput))
                {
                    ScheduleMove(moveDirectionInput);
                    GameLogic.Instance.StartTick(moveDirectionInput);
                }
                else
                {
                    IsNextTickScheduled = false; // False if player cannot move
                    // Future ways may include replacing the isScheduledMoveAndBeforeTickEnd variable and using a
                    // Exception list for the Start Tick's and End Tick's Schedule method, especially for the boulder.
                    // Boulder's tick schedule declines the Crate Tick's method.
                }
                _postMoveAndTickEnd = true;
            }
        }
    }
    
    public override void OnStartTick(Vector3 playerMoveDir)
    {
        // print("Player start tick");
        DoScheduledMove();
    }
}
