using UnityEngine;
using UnityEngine.Serialization;

public class HoleScript : BasicTickable
{
    public Sprite filledHoleSprite;
    public bool isFilled;

    private void Fill()
    {
        if (isFilled)
        {
            return;
        }

        isFilled = true;
        SpriteRenderer.sprite = filledHoleSprite;
    }

    public override void OnEndTick()
    {
        base.OnEndTick();
        
        Collider2D[] entities = Physics2D.OverlapPointAll(transform.position, LayerMask.GetMask("Collision"));
        foreach(Collider2D entity in entities)
        {
            if (entity && !isFilled)
            {
                if (entity.CompareTag("Boulder"))
                {
                    GameLogic.PlayAudioClipRandom(triggerSounds);
                    Fill();
                    Destroy(entity.gameObject);
                }
                else if (entity.TryGetComponent<BasicLivingEntity>(out var livingEntity))
                {
                    if (entity.CompareTag("Player"))
                    {
                        GameLogic.PlayAudioClipRandom(PlayerScript.Instance.playerFallSounds);
                        GameLogic.Instance.GameOver("Player jatuh ke jurang");
                        livingEntity.IsNextTickDestroyScheduled = true;
                    }
                    else if (entity.CompareTag("Mummy"))
                    {
                        GameLogic.PlayAudioClipRandom(PlayerScript.Instance.playerFallSounds);
                        livingEntity.Deactivate();
                        Destroy(entity.gameObject);
                    }
                }
            }
        }
    }
}
