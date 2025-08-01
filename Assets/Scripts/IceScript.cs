using System.Collections.Generic;
using UnityEngine;

public class IceScript : BasicTickable
{
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
                        if (moveable.StartTickPosition == entity.transform.position) continue;
                        // IsNextTickScheduled = true;
                        if (moveable.IsStationary())
                        {
                            if (moveable.CanMove(moveable.LastMoveDir))
                            {
                                moveable.ScheduleMove(moveable.LastMoveDir);
                                // GameLogic.Instance.StartTick(moveable.LastMoveDir);
                            }
                        }
                    }
                }
            }
        }
    }
}
