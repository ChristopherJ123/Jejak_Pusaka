using System.Collections.Generic;
using UnityEngine;

public class IceScript : MonoBehaviour, ITickable
{
    public bool IsNextTickScheduled { get; set; }
    public bool IsStartTicking { get; set; }
    public bool IsEndTicking { get; set; }

    public void OnStartTick(Vector3 playerMoveDir)
    {
        
    }

    public void PostStartTick(Vector3 playerMoveDir)
    {
        
    }

    public void OnEndTick()
    {
        // print("Ice end tick");

        List<Collider2D> entities = new List<Collider2D>();
        Physics2D.OverlapCollider(GetComponent<Collider2D>(), entities);
        if (entities.Count == 0)
        {
            IsNextTickScheduled = false;
            return;
        }
        foreach (Collider2D entity in entities)
        {
            if (entity)
            {
                if (entity.TryGetComponent<IMoveable>(out var moveable))
                {
                    if (entity.CompareTag("Player") || entity.CompareTag("Crate"))
                    {
                        IsNextTickScheduled = true;
                        if (moveable.IsStationary())
                        {
                            if (entity.CompareTag("Player")) PlayerMovementScript.Instance.IsNextTickScheduled = true; // Setting this to true so that the Player's CanMove()'s PreStartTick()'s IsPlayerPushing() method works.
                            if (moveable.CanMove(moveable.LastMoveDir))
                            {
                                moveable.ScheduleMove(moveable.LastMoveDir);
                                // GameLogic.Instance.StartTick(moveable.LastMoveDir);
                            }
                            else
                            {
                                if (entity.CompareTag("Player")) PlayerMovementScript.Instance.IsNextTickScheduled = false; // Set False if player cannot move instead
                                IsNextTickScheduled = false;
                            }
                        }
                    }
                }
            }
            else
            {
                IsNextTickScheduled = false;
            }
        }
    }

    public void PostEndTick()
    {
        
    }
}
