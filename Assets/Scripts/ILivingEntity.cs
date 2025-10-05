using UnityEngine;

public interface ILivingEntity
{
    void OnLivingEntityStartTick(Vector3 scheduledMoveDir = new Vector3());
    void ScheduleMove();
}