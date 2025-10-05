using UnityEngine;

public class BasicLivingEntity : BasicMoveable, ILivingEntity
{
    public void OnLivingEntityStartTick(Vector3 scheduledMoveDir = new Vector3())
    {
        if (scheduledMoveDir != Vector3.zero)
        {
            // This probably means a player is trying to move
            if (CanMoveOrRedirect(ref scheduledMoveDir))
            {
                ScheduleMove(scheduledMoveDir);
            }
        }
        else
        {
            ScheduleMove();
        }
    }

    public virtual void ScheduleMove()
    {
    }
}