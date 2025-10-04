using UnityEngine;
using UnityEngine.Serialization;

public class TreasureScript : BasicTickable
{
    [SerializeField]
    private Sprite[] sprites;
    
    public virtual bool IsPlayerPushing(Vector3 moveDirection)
    {
        return PlayerScript.Instance.IsNextTickMoveScheduled &&
               PlayerScript.Instance.transform.position + moveDirection == transform.position;
    }
    public override void OnStartTick(Vector3 playerMoveDir)
    {
        base.OnStartTick(playerMoveDir);

        if (IsPlayerPushing(playerMoveDir))
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