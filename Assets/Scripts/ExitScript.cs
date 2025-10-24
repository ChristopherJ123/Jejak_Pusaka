using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishScript : BasicTickable
{
    public AudioClip exitSound;
    public string nextLevel;
    public bool isGameEnd;

    public override void Start()
    {
        base.Start();
        GlobalGameManager.Instance.exitSound = exitSound;
    }

    public override void OnEndTick()
    {
        base.OnEndTick();
        
        var colliders = Physics2D.OverlapPointAll(transform.position, LayerMask.GetMask("Collision"));
        foreach(var collide in colliders)
        {
            if (collide && collide.CompareTag("Player"))
            {
                if (GameLogic.Instance.shouldSaveScore)
                {
                    GlobalGameManager.Instance.isGameEnd = isGameEnd;
                    GlobalGameManager.Instance.ChangeLevel(nextLevel, GameLogic.Instance.Score);
                }
                else
                {
                    GlobalGameManager.Instance.ChangeLevel(nextLevel);
                }
                break;
            }
        }
    }
}