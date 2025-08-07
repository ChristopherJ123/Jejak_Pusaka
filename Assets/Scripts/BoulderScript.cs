using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoulderScript : BasicMoveable
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
    
    public override void OnStartTick(Vector3 playerMoveDir)
    {
        // print(transform.name + " start tick");
        base.OnStartTick(playerMoveDir);

        if (IsBoulderPushing())
        {
            var down = Vector3.down;
            if (CanMoveOrRedirect(ref down))
            {
                Move(down);
            }
        }
    }
    
    public override void OnEndTick()
    {
        base.OnEndTick();
        
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
					    // print("Calling farside trigger");
                        // fall
                        ScheduleFall();
                        return;
                    }
                    farSideTriggeredCount++;
                    if (farSideTriggeredCount == 2)
                    {
                        print("Calling farside trigger twice");
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
