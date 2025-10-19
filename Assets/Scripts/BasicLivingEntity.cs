using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class BasicLivingEntity : BasicMoveable, ILivingEntity
{
    public bool IsAlive { get; set; }
    
    public virtual bool LivingEntityMoveCondition(Vector3 moveDir)
    {
        if (!IsAlive)
        {
            print("Returning false for dead entity");
            return false;
        }
        
        var allMoveables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IMoveable>();

        foreach (var moveable in allMoveables)
        {
            if (moveable.IsLivingEntityPushing(out var livingEntity))
            {
                print("SOMEHOME THIS IS TRUE");
                if (moveable.CanMoveOrRedirect(ref moveDir))
                {
                    print($"{livingEntity.name} can move to {moveDir}");
                    return true;
                }
                return false;
            }
        }

        print("Returning true for normal movement");
        return true;
    }

    public void OnLivingEntityStartTick()
    {
        if (PlayerScript.Instance.IsNextTickMoveScheduled && PlayerScript.Instance.scheduledMoveByUser && IsAlive)
        {
            ScheduleAutoMove();
        }
        PlayMoveSound();
    }

    public virtual void ScheduleAutoMove()
    {
    }

    public virtual void Awake()
    {
        IsAlive = true;
    }
}