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
        
        Collider2D entity = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("Collision"));
        if (entity && !isFilled)
        {
            if (entity.CompareTag("Boulder"))
            {
                GameLogic.PlayAudioClipRandom(triggerSounds);
                Fill();
                Destroy(entity.gameObject);
            } else if (entity.CompareTag("Player"))
            {
                GameLogic.PlayAudioClipRandom(PlayerScript.Instance.playerFallSounds);
                GameLogic.Instance.GameOver("Player jatuh ke jurang");
                entity.gameObject.GetComponent<PlayerScript>().IsNextTickDestroyScheduled = true;
            }
            else if (entity.CompareTag("Mummy"))
            {
                GameLogic.PlayAudioClipRandom(PlayerScript.Instance.playerFallSounds);
                Destroy(entity.gameObject);
            }
        }
    }
}
