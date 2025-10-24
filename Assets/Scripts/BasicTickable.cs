using UnityEngine;
using UnityEngine.Tilemaps;

public class BasicTickable : MonoBehaviour, ITickable
{
    protected SpriteRenderer SpriteRenderer;
    [SerializeField]
    public AudioClip[] triggerSounds;
    [SerializeField]
    protected AudioClip[] destroySounds;
    public bool IsTriggered { get; set; }
    public bool NextRandom { get; set; }
    public bool IsNextTickDestroyScheduled { get; set; }
    public bool IsNextTickMoveScheduled { get; set; }
    public bool DoExtraTickLoop { get; set; }
    public bool IsStartTicking { get; set; }
    public bool IsEndTicking { get; set; }
    public virtual void OnStartTick()
    {
        if (IsNextTickDestroyScheduled)
        {
            GameLogic.PlayAudioClipRandom(destroySounds);
            Deactivate();
            Destroy(gameObject);
        }
    }

    public virtual void OnPostStartTick()
    {
        
    }

    public virtual void OnEndTick()
    {
        
    }

    public virtual void OnPostEndTick()
    {
        
    }

    public virtual void OnReset()
    {
        
    }

    public virtual void Deactivate()
    {
        gameObject.layer = LayerMask.NameToLayer("Tile");
    }

    public virtual void Start()
    {
        if (gameObject.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = -(int)(transform.position.y * 10);
        }
        // else if (gameObject.TryGetComponent<TilemapRenderer>(out var tilemapRenderer))
        // {
        //     tilemapRenderer.sortingOrder = -10000;
        // }
    }
}