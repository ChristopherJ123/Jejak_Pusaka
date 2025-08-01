using UnityEngine;

public class BasicTickable : MonoBehaviour, ITickable
{
    public virtual bool IsNextTickScheduled { get; set; }
    public virtual bool IsStartTicking { get; set; }
    public virtual bool IsEndTicking { get; set; }
    public virtual void OnStartTick(Vector3 playerMoveDir)
    {
        
    }

    public virtual void PostStartTick(Vector3 playerMoveDir)
    {
        
    }

    public virtual void OnEndTick()
    {
        
    }

    public virtual void PostEndTick()
    {
        
    }
}