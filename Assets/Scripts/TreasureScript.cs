using UnityEngine;
using UnityEngine.Serialization;

public class TreasureScript : BasicTickable
{
    [SerializeField]
    private Sprite[] sprites;
    
    public virtual bool IsPlayerPushing()
    {
        return PlayerScript.Instance.IsNextTickMoveScheduled &&
               PlayerScript.Instance.transform.position + PlayerScript.Instance.ScheduledMoveDir == transform.position;
    }
    public override void OnStartTick()
    {
        base.OnStartTick();

        if (IsPlayerPushing())
        {
            GameLogic.Instance.AddScore();
            GameLogic.PlayAudioClipRandom(triggerSounds);
            Destroy(gameObject);
        }
    }

    public override void Start()
    {
        base.Start();
        var randomIndex = Random.Range(0, sprites.Length);
        SpriteRenderer.sprite = sprites[randomIndex];
    }
}