using UnityEngine;

public interface ILivingEntity
{
    bool IsAlive { get; set; }
    
    /// <summary>
    /// Determine whether LivingEntity can move in a certain direction.
    /// </summary>
    /// <param name="moveDir">LivingEntity move direction</param>
    /// <returns></returns>
    bool LivingEntityMoveCondition(Vector3 moveDir);
    void OnLivingEntityStartTick();
    void ScheduleAutoMove();
}