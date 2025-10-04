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
        
        var collide = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("Collision"));
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
        }
    }
}