using System.Collections.Generic;
using UnityEngine;

public class IceScript : BasicTickable
{
    public bool firstTimeStepOnIce = true;
    
    public override void OnEndTick()
    {
        // print("Ice end tick");
        base.OnEndTick();

        List<Collider2D> entities = new List<Collider2D>();
        Physics2D.OverlapCollider(GetComponent<Collider2D>(), entities);
        if (entities.Count == 0)
        {
            return;
        }
        foreach (Collider2D entity in entities)
        {
            if (entity)
            {
                if (entity.TryGetComponent<IMoveable>(out var moveable))
                {
                    if (moveable.IsIceMoveable)
                    {
                        // This condition means that this moveable is stationary ontop the ice.
                        if (moveable.StartTickPosition == entity.transform.position) continue;
                        if (moveable.IsStationary())
                        {
                            var moveableLastMoveDir = moveable.LastMoveDir;
                            if (moveable.CanMoveOrRedirect(ref moveableLastMoveDir))
                            {
                                // print("Moving " + entity.name);
                                if (firstTimeStepOnIce)
                                {
                                    GameLogic.PlayAudioClipRandom(triggerSounds);
                                    firstTimeStepOnIce = false;
                                }
                                moveable.ScheduleMove(moveableLastMoveDir);
                            }
                        }
                    }
                }
            }
        }
    }

    public override void OnReset()
    {
        base.OnReset();
        firstTimeStepOnIce = true;
    }
}
