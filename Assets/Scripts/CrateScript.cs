using UnityEngine;
using UnityEngine.Serialization;

public class CrateScript : BasicMoveable
{
    [SerializeField]
    private GameObject crateFloatingPrefab;
    [SerializeField]
    private AudioClip[] crateWaterSounds;
    [SerializeField]
    private AudioClip[] crateLavaSounds;

    public override void OnEndTick()
    {
        base.OnEndTick();

        // Check if crate is on water/lava
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
            else if (col.CompareTag("Floating Crate") || col.CompareTag("Floating Boulder"))
            {
                onWater = false;
                onLava = false;
                break;
            }
        }

        if (onWater)
        {
            destroySounds = crateWaterSounds;
            Instantiate(crateFloatingPrefab, transform.position, Quaternion.identity);
            IsNextTickDestroyScheduled = true;
        } else if (onLava)
        {
            destroySounds = crateLavaSounds;
            IsNextTickDestroyScheduled = true;
        }
    }
}