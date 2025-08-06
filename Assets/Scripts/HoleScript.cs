using UnityEngine;

public class HoleScript : MonoBehaviour, ITickable
{
    public Sprite filledHoleSprite;
    private bool _isFilled;
    private SpriteRenderer _spriteRenderer;
    
    public void Fill()
    {
        if (_isFilled)
        {
            return;
        }

        _isFilled = true;
        _spriteRenderer.sprite = filledHoleSprite;
    }
    
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool IsNextTickScheduled { get; set; }
    public bool DoExtraTickLoop { get; set; }
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
        Collider2D entity = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("Collision"));
        if (entity && entity.CompareTag("Boulder"))
        {
            Fill();
            Destroy(entity.gameObject);
        }
    }

    public void PostEndTick()
    {
        
    }
}
