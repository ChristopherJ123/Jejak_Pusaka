using UnityEngine;
using UnityEngine.Tilemaps;

public class BasicTickable : MonoBehaviour, ITickable
{
    protected SpriteRenderer SpriteRenderer;
    public bool IsTriggered { get; set; }
    public bool NextRandom { get; set; }
    public bool IsNextTickScheduled { get; set; }
    public bool DoExtraTickLoop { get; set; }
    public bool IsStartTicking { get; set; }
    public bool IsEndTicking { get; set; }
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

    public virtual void Start()
    {
        if (gameObject.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = -(int)(transform.position.y * 100) + 10;
        }
        else if (gameObject.TryGetComponent<TilemapRenderer>(out var tilemapRenderer))
        {
            tilemapRenderer.sortingOrder = -10000;
        }
    }
}