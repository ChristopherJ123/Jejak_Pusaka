using UnityEngine;

public class HoleScript : BasicTickable
{
    public Sprite filledHoleSprite;
    private bool _isFilled;
    
    public void Fill()
    {
        if (_isFilled)
        {
            return;
        }

        _isFilled = true;
        SpriteRenderer.sprite = filledHoleSprite;
    }

    public override void OnEndTick()
    {
        base.OnEndTick();
        
        Collider2D entity = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("Collision"));
        if (entity && entity.CompareTag("Boulder"))
        {
            Fill();
            Destroy(entity.gameObject);
        }
    }
}
