using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArrowScript : BasicMoveable, IDirectional
{
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
    public Vector3 Direction { get; set; }

    public void SetDirection(float dir)
    {
        Direction = (Mathf.RoundToInt(dir / 90) % 4) switch
        {
            -3 => Vector3.up,
            -2 => Vector3.left,
            -1 => Vector3.down,
            0 => Vector3.right,
            1 => Vector3.up,
            2 => Vector3.left,
            3 => Vector3.down,
            _ => Vector3.right
        };
        // print(Mathf.RoundToInt(dir / 90) % 4);
        // print(Direction);
    }
    
    private bool IsArrowPushing()
    {
        foreach (var trigger in _triggerNear)
        {
            var entity = Physics2D.OverlapPoint(transform.position + trigger, LayerStopsMovement);
            if (entity)
            {
                // print(transform.name + " is detecting an entity in trigger near");

                // boulder di atase
                if (entity.CompareTag("Arrow") && entity.transform.position - trigger == transform.position)
                {
                    // print(transform.name + " is detecting an arrow pushing");

                    var arrow = entity.GetComponent<ArrowScript>();
                    // dengan condition boulder dischedule move, artinya yang bisa mendorong cuma boulder yang
                    // sudah triggered dan sedang moving (aka. di schedule move).
                    // UPDATE: Pakai IsTriggered yang artinya sudah pernah di schedule agar konsisten
                
                    if (arrow.IsTriggered)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void ScheduleLaunch()
    {
        var moveDir = Direction;
        if (CanMoveOrRedirect(ref moveDir))
        {
            isTriggeredNear = true;
            ScheduleMove(moveDir, true);
        }
    }

    public override void OnStartTick(Vector3 playerMoveDir)
    {
        // print(transform.name + " has start tick");
        base.OnStartTick(playerMoveDir);
        
        if (IsArrowPushing())
        {
            // print("Is arrow pushing");
            var dir = Direction;
            if (CanMoveOrRedirect(ref dir))
            {
                Move(dir);
            }
        }
    }

    // Same logic with boulder
    public override void OnEndTick()
    {
        base.OnEndTick();
        
        if (LastMoveDir != Vector3.zero)
        {
            // Was moved and that means is triggered
            isTriggeredNear = true;
            ScheduleLaunch();
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
                ScheduleLaunch();
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
					    // print("Calling farside trigger");
                        // fall
                        ScheduleLaunch();
                        return;
                    }
                    farSideTriggeredCount++;
                    if (farSideTriggeredCount == 2)
                    {
                        print("Calling farside trigger twice");
                        // fall
                        isTriggeredNear = true;
                        ScheduleLaunch();
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
                    // print("Calling nearside trigger");
                    isTriggeredNear = true;
                    break;
                }
            }
        }
        else
        {
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
        SetDirection(transform.eulerAngles.z);
    }
}
