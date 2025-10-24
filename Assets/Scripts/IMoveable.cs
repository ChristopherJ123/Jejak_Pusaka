using UnityEngine;

public interface IMoveable
{
    Vector3 StartTickPosition { get; set; }
    Vector3 EndTickPosition { get; set; }
    Vector3 LastMoveDir { get; set; }
    Vector3 ScheduledMoveDir { get; set; }
    bool IsIceMoveable { get; set; }
    bool IsPinballMoveable { get; set; }
    bool IsSlopeMoveable { get; set; }
    /// <summary>
    /// Entity will slide of Pinball (e.g. Arrows hitting pinball, boulder hitting pinball)
    /// </summary>
    bool IsPinballSlopeMoveable { get; set; }
    /// <summary>
    /// Entity will slide of Boulder (e.g. Boulder hitting boulder, arrow hitting boulder)
    /// </summary>
    bool IsBoulderSlopeMoveable { get; set; }

    bool IsStationary();
    bool IsLivingEntityPushing(out BasicLivingEntity livingEntity);
    bool CanMoveOrRedirect(ref Vector3 moveDir);
    void ScheduleMove(Vector3 moveDir, bool doExtraTickLoop = false);
    void DoScheduledMove();
    void Move(Vector3 moveDir);
    void PlayMoveSound();
}