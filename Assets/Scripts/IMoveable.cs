using UnityEngine;

public interface IMoveable
{
    Vector3 StartTickPosition { get; set; }
    Vector3 EndTickPosition { get; set; }
    Vector3 LastMoveDir { get; set; }
    Vector3 ScheduledMoveDir { get; set; }
    bool IsIceMoveable { get; set; }
    bool IsPinballMoveable { get; set; }
    bool IsStationary();
    bool IsPlayerPushing(Vector3 moveDirection);
    bool CanMoveOrPinballRedirect(ref Vector3 moveDir);
    void ScheduleMove(Vector3 moveDir);
    void DoScheduledMove();
    void Move(Vector3 moveDir);
}