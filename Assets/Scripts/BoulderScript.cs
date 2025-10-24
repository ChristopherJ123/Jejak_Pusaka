using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoulderScript : BasicMoveable
{
    [SerializeField]
    private GameObject boulderFloatingPrefab;
    [SerializeField]
    private AudioClip[] boulderWaterSounds;
    [SerializeField]
    private AudioClip[] boulderLavaSounds;
    [SerializeField]
    private AudioClip[] boulderSplatSounds;
    
    public bool isTriggeredNear;
    private readonly Vector3[] _triggerFar =
    {
        Vector3.up + Vector3.left,
        Vector3.right + Vector3.up,
        Vector3.down + Vector3.right,
        Vector3.left + Vector3.down,
    };
    private readonly Vector3[] _triggerNear =
    {
        Vector3.up,
        Vector3.down,
        Vector3.left,
        Vector3.right,
    };
    private Dictionary<Vector3, GameObject> _entityInAreaBefore = new();
    private Dictionary<Vector3, GameObject> _entityInArea = new();

    private bool IsBoulderPushing()
    {
        var entity = Physics2D.OverlapPoint(transform.position + Vector3.up, LayerStopsMovement);
        if (entity)
        {
            // print(transform.name + " is detecting a boulder above");

            // boulder di atase
            if (entity.CompareTag("Boulder") && entity.transform.position + Vector3.down == transform.position)
            {
                var boulder = entity.GetComponent<BoulderScript>();
                // dengan condition boulder dischedule move, artinya yang bisa mendorong cuma boulder yang
                // sudah triggered dan sedang moving (aka. di schedule move).
                // UPDATE: Pakai IsTriggered yang artinya sudah pernah di schedule agar konsisten
                
                if (boulder.IsTriggered)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    // Called at movement start tick
    public override bool CanMoveOrRedirect(ref Vector3 moveDir)
    {
        // First
        var canMove = base.CanMoveOrRedirect(ref moveDir);
        
        // Last. If there's any UP moving then return false. Boulder cannot go up.
        return !Mathf.Approximately(moveDir.y, 1) && canMove;
    }

    private void ScheduleFall()
    {
        var fallDirection = Vector3.down;
        if (CanMoveOrRedirect(ref fallDirection))
        {
            isTriggeredNear = true;
            ScheduleMove(fallDirection, true);
        }
    }

    protected override void OnHit(GameObject hitObject)
    {
        // print($"BOULDER HIT EVENT");
        if (hitObject && hitObject.TryGetComponent<BasicLivingEntity>(out var entity))
        {
            // print($"BOULDER HIT {hitObject.name}");
            if (entity.CompareTag("Player"))
            {
                // If diagonal movement then check all collisions (2 collisions total before splatting player)
                if (Mathf.Approximately(Mathf.Abs(LastMoveDir.x), 1f) && Mathf.Approximately(Mathf.Abs(LastMoveDir.y), 1f))
                {
                    if (!GameLogic.IsSpaceAvailable(transform.position + new Vector3(LastMoveDir.x, 0, 0)))
                    {
                        base.OnHit(hitObject);
                        return;
                    }
                }
                hitObject.transform.localScale = new Vector3(hitObject.transform.localScale.x, hitObject.transform.localScale.y / 4, hitObject.transform.localScale.z);
                GameLogic.PlayAudioClipRandom(boulderSplatSounds);
                GameLogic.Instance.GameOver("Player terlindas oleh boulder");
                Move(LastMoveDir);
            } else if (entity.CompareTag("Mummy"))
            {
                // If diagonal movement then check all collisions (2 collisions total before splatting player)
                if (Mathf.Approximately(Mathf.Abs(LastMoveDir.x), 1f) && Mathf.Approximately(Mathf.Abs(LastMoveDir.y), 1f))
                {
                    if (!GameLogic.IsSpaceAvailable(transform.position + new Vector3(LastMoveDir.x, 0, 0)))
                    {
                        base.OnHit(hitObject);
                        return;
                    }
                }
                hitObject.transform.localScale = new Vector3(hitObject.transform.localScale.x, hitObject.transform.localScale.y / 4, hitObject.transform.localScale.z);
                entity.Deactivate(); // Makes the entity a tile layer and destroys movePoint (movePoint is a collision)
                GameLogic.PlayAudioClipRandom(boulderSplatSounds);
                ScheduleFall();
                // Move(LastMoveDir);
            }
            base.OnHit(hitObject);
        }
        else
        {
            base.OnHit(hitObject);
        }
    }

    public override void OnStartTick()
    {
        // print(transform.name + " start tick");
        base.OnStartTick();

        if (IsBoulderPushing() && GameLogic.IsSpaceAvailable(transform.position + Vector3.down))
        {
            var down = Vector3.down;
            if (CanMoveOrRedirect(ref down))
            {
                // print($"{transform.name} is boulder pushing");
                PlayMoveSound();
                Move(down);
            }
        }
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
            else if (col.CompareTag("Floating Crate") || col.CompareTag("Floating Boulder"))
            {
                onWater = false;
                onLava = false;
                break;
            }
        }

        if (onWater)
        {
            destroySounds = boulderWaterSounds;
            IsNextTickDestroyScheduled = true;
            return;
        }
        if (onLava)
        {
            destroySounds = boulderLavaSounds;
            Instantiate(boulderFloatingPrefab, transform.position, Quaternion.identity);
            IsNextTickDestroyScheduled = true;
            return;
        }
        
        // Logic part
        if (LastMoveDir != Vector3.zero)
        {
            // print("am triggered");
            // Was moved and that means is triggered
            isTriggeredNear = true;
            ScheduleFall();
            return;
        }
        _entityInArea.Clear();
        Vector3[] allTrigger = _triggerFar.Concat(_triggerNear).ToArray();
        foreach (Vector3 direction in allTrigger)
        {
            Collider2D entity = Physics2D.OverlapPoint(transform.position + direction, LayerStopsMovement);
            if (entity)
            {
                _entityInArea.Add(direction, entity.gameObject);
            }
        }
        
        if (GameLogic.AreDictionariesDifferent(_entityInArea, _entityInAreaBefore))
        {
            // Left the trigger
            if (_entityInArea.Count < _entityInAreaBefore.Count)
            {
                // print("Calling left the trigger");
                // fall
                isTriggeredNear = true;
                ScheduleFall();
                return;
            }
            
            // Step onto farside trigger
            int farSideTriggeredCount = 0;
            foreach (Vector3 direction in _triggerFar)
            {
                Collider2D entity = Physics2D.OverlapPoint(transform.position + direction, LayerStopsMovement);
                if (
                    // Sebelumnya gaada, setelahnya ada entity
                    (entity && !_entityInAreaBefore.ContainsKey(direction)) 
                    ||
                    // Make sure we choose the different entity (gabisa null/gaada makanya ada kondisi atas)
                    (entity &&
                     _entityInAreaBefore.TryGetValue(direction, out var beforeEntity) &&
                     _entityInArea.TryGetValue(direction, out var currentEntity) &&
                     beforeEntity != currentEntity)
                    ||
                    // Sebelumnya ada, terus gaada (kasus buat dorong crate bisa)
                    (!entity && _entityInAreaBefore.ContainsKey(direction))
                    )
                {
                    if (isTriggeredNear)
                    {
					    // print(transform.name + "Calling farside trigger");
                        // fall
                        ScheduleFall();
                        return;
                    }
                    farSideTriggeredCount++;
                    if (farSideTriggeredCount == 2)
                    {
                        // print(transform.name + " Calling farside trigger twice2");
                        // fall
                        isTriggeredNear = true;
                        ScheduleFall();
                        return;
                    }
                }
            }

            // Step onto nearside trigger
            foreach (Vector3 direction in _triggerNear)
            {
                Collider2D entity = Physics2D.OverlapPoint(transform.position + direction, LayerStopsMovement);
                if (
                    (entity && !_entityInAreaBefore.ContainsKey(direction)) 
                    ||
                    (entity &&
                     _entityInAreaBefore.TryGetValue(direction, out var beforeEntity) &&
                     _entityInArea.TryGetValue(direction, out var currentEntity) &&
                     beforeEntity != currentEntity)
                    )
                {
                    print($"{transform.name} Calling nearside trigger");
                    isTriggeredNear = true;
                    break;
                }
            }
        }
        else
        {
            // Hardcoded way right now. If player is in the boulder, triggered near
            // won't be reset, only reset if it's from a crate or other non-living
            // entities.
            if (_entityInArea.ContainsValue(PlayerScript.Instance.gameObject))
            {
                return;
            }
            isTriggeredNear = false;
        }
        _entityInAreaBefore = new Dictionary<Vector3, GameObject>(_entityInArea);
    }
    
    public override void Start()
    {
        base.Start();
        IsIceMoveable = false;
        IsPinballMoveable = false;
        IsSlopeMoveable = true;
        IsPinballSlopeMoveable = true;
        IsBoulderSlopeMoveable = true;
        
        Vector3[] allTrigger = _triggerFar.Concat(_triggerNear).ToArray();
        foreach (Vector3 direction in allTrigger)
        {
            Collider2D entity = Physics2D.OverlapPoint(transform.position + direction, LayerStopsMovement);
            if (entity)
            {
                _entityInArea.Add(direction, entity.gameObject);
                _entityInAreaBefore.Add(direction, entity.gameObject);
            }
        }
    }
}
