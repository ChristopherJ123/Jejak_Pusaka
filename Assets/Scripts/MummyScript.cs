using System;
using System.Linq;
using UnityEngine;

public class MummyScript : BasicAI
{
    [SerializeField]
    private AudioClip[] mummyWaterSounds;
    [SerializeField]
    private AudioClip[] mummyLavaSounds;
    public AudioClip[] mummyFallSounds;
    
    public override bool IsLivingEntityPushing(out BasicLivingEntity livingEntity)
    {
        // Return false if you don't want to have infinite loop 👀. Well actually since player is
        // an instance of IMoveable, this method bawaan is not true.
        livingEntity = null;
        return false;
    }

    /// <summary>
    /// Start a PreStartTickConditions player movement check.
    /// </summary>
    /// <param name="moveDir">Player move dir</param>
    /// <returns>Check succeed</returns>
    public override bool CanMoveOrRedirect(ref Vector3 moveDir)
    {
        IsNextTickMoveScheduled = true; // Setting this to true so that the CanMove()'s PreStartTick()'s IsPlayerPushing() method works.
        
        // First check if player hits a pinball and needs a moveDir redirect
        moveDir = PinballGlobalScript.RedirectMoveFromPinballIfAny(gameObject, moveDir);

        // Thirdly check if player can move entities, more detailed see method docs
        var result = LivingEntityMoveCondition(moveDir);
        
        // Custom check, just like GameLogic's IsSpaceAvailable but modified.
        if (result && Physics2D.OverlapPoint(MovePoint.transform.position + moveDir, LayerAllowMovement))
        {
            // Lastly Check if mummy is not colliding with an IMoveable
            var colide = Physics2D.OverlapPoint(MovePoint.transform.position + moveDir, LayerStopsMovement);
            if (colide && !colide.TryGetComponent<IMoveable>(out _))
            {
                return false;
            }
            return true;
        }
        IsNextTickMoveScheduled = false;
        
        return false;
    }
    
    public override void OnStartTick()
    {
        // print("Player start tick");
        if (IsNextTickDestroyScheduled)
        {
            GameLogic.PlayAudioClipRandom(destroySounds);
            gameObject.SetActive(false);
        }
        
        DoScheduledMove();
        StartTickPosition = transform.position;
    }

    public override void OnEndTick()
    {
        base.OnEndTick();
        
        // Check if boulder is on water/lava
        bool onWater = false;
        bool onLava = false;
        
        Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position);
        foreach (var col in colliders)
        {
            if (col.CompareTag("Water"))
            {
                onWater = true;
            }
            else if (col.CompareTag("Lava"))
            {
                onLava = true;
            }
            else if (col.CompareTag("Floating Crate") || col.CompareTag("Floating Boulder") || col.CompareTag("Ice"))
            {
                onWater = false;
                onLava = false;
                break;
            }
        }

        if (onWater)
        {
            destroySounds = mummyWaterSounds;
            IsNextTickDestroyScheduled = true;
        }
        else if (onLava)
        {
            destroySounds = mummyLavaSounds;
            IsNextTickDestroyScheduled = true;
        }
    }

    public override void OnPostEndTick()
    {
        base.OnPostEndTick();
        SpriteRenderer.sortingOrder += 2;
    }
}
