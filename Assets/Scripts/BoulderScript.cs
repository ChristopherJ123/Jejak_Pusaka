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
                
                if (boulder.IsNextTickScheduled)
                {
                    return true;
                }
            }
            // else
            // {
            //     print(transform.name + ", " + entity.transform.position + ", " + transform.position + ", " + entity.CompareTag("Boulder"));
            // }
        }
        // else
        // {
        //     print(transform.name + " is not detecting any overlap");
        // }
        return false;
    }
    
    // Called at movement start tick
    public override bool CanMoveOrPinballRedirect(ref Vector3 moveDir)
    {
        if (moveDir == Vector3.up) return false;
        
        return base.CanMoveOrPinballRedirect(ref moveDir);
    }

    private void ScheduleFall()
    {
        var fallDirection = CanFall();
        if (fallDirection != Vector3.zero && CanMoveOrPinballRedirect(ref fallDirection))
        {
            isTriggeredNear = true;
            ScheduleMove(fallDirection);
        }
    }
    
    private Vector3 CanFall()
    {
        Collider2D collide = Physics2D.OverlapPoint(MovePoint.position + Vector3.down, LayerStopsMovement);
        if (collide)
        {
            if (collide.CompareTag("Boulder"))
            {
                if (!GameLogic.Instance.GetGameObjectAtCoordinates(MovePoint.position + Vector3.left) && 
                    !GameLogic.Instance.GetGameObjectAtCoordinates(MovePoint.position + Vector3.down + Vector3.left))
                {
                    // Fall left
                    return new Vector3(-1, -1, 0);
                } if (!GameLogic.Instance.GetGameObjectAtCoordinates(MovePoint.position + Vector3.right) && 
                      !GameLogic.Instance.GetGameObjectAtCoordinates(MovePoint.position + Vector3.down + Vector3.right))
                {
                    // Fall right
                    return new Vector3(1, -1, 0);
                }
            }
            return Vector3.zero;
        }
        if (MovePoint.position + Vector3.down == PlayerScript.Instance.transform.position)
        {
            return Vector3.zero;
        }
        
        // Debug.Log("Returning 1");
        // 0: none, 1: down, 2: left, 3: right
        return Vector3.down;
    }
    
    bool AreDictionariesDifferent<TKey, TValue>(Dictionary<TKey, TValue> a, Dictionary<TKey, TValue> b)
    {
        if (a.Count != b.Count) return true;

        foreach (var kvp in a)
        {
            if (!b.TryGetValue(kvp.Key, out TValue valueB)) return true;

            if (!EqualityComparer<TValue>.Default.Equals(kvp.Value, valueB)) return true;
        }

        return false; // Dictionaries are equal
    }
    
    public override void OnStartTick(Vector3 playerMoveDir)
    {
        // print(transform.name + " start tick");
        base.OnStartTick(playerMoveDir);

        if (IsBoulderPushing())
        {
            var down = Vector3.down;
            if (CanMoveOrPinballRedirect(ref down))
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
        
        if (AreDictionariesDifferent(_entityInArea, _entityInAreaBefore))
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
