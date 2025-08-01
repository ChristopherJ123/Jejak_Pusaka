// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
//
// public class BoulderScriptOld2 : MonoBehaviour, ITickable
// {
//     private List<HoleScript> _mapHoles;
//     // mapWaters
//     // mapLavas
//     
//     // public BoulderTriggerScript triggerScript;
//     private LayerMask _layerStopsMovement;
//     [SerializeField]
//     private Transform movePoint;
//     public bool isTickTicking;
//     private Vector3 _moveDirection;
//     
//     public bool ScheduledToMove { get; private set; }
//     private Vector3 _scheduledMoveDir = Vector3.zero;
//     public bool scheduledToDelete;
//
//     private bool _isTriggeredNear;
//     private Vector3[] _triggerFar =
//     {
//         Vector3.up + Vector3.left,
//         Vector3.right + Vector3.up,
//         Vector3.down + Vector3.right,
//         Vector3.left + Vector3.down,
//     };
//     private Vector3[] _triggerNear =
//     {
//         Vector3.up,
//         Vector3.down,
//         Vector3.left,
//         Vector3.right,
//     };
//     private Dictionary<Vector3, GameObject> _entityInAreaBefore = new();
//     private Dictionary<Vector3, GameObject> _entityInArea = new();
//     
//     /// <summary>
//     /// Boulder is idle.
//     /// </summary>
//     /// <returns>bool</returns>
//     public bool IsStationary()
//     {
//         return Vector3.Distance(transform.position, movePoint.position) == 0;
//     }
//     
//     public bool IsPlayerPushing(Vector3 moveDirection)
//     {
//         // First check if player is heading toward this specific crate
//         return PlayerMovementScript.Instance.movePoint.position + moveDirection == transform.position;
//     }
//     
//     // Called at movement start tick
//     public bool CanMove(Vector3 moveDirection)
//     {
//         if (moveDirection == Vector3.up) return false;
//         
//         if (!Physics2D.OverlapPoint(movePoint.transform.position + moveDirection, _layerStopsMovement))
//         {
//             // Debug.Log("Boulder is moving right");
//             return true;
//         }
//         return false;
//     }
//
//     public void ScheduleMove(Vector3 moveDirection)
//     {
//         ScheduledToMove = true;
//         _scheduledMoveDir = moveDirection;
//     }
//
//     public void Move()
//     {
//         if (_scheduledMoveDir != Vector3.zero)
//         {
//             movePoint.transform.position += _scheduledMoveDir;
//             
//             ScheduledToMove = false;
//             _scheduledMoveDir = Vector3.zero;
//         }
//     }
//
//     public void Move(Vector3 moveDirection)
//     {
//         movePoint.transform.position += moveDirection;
//     }
//     
//     private Vector3 CanFall()
//     {
//         if (Physics2D.OverlapPoint(movePoint.position + Vector3.down, _layerStopsMovement))
//         {
//             Debug.Log("Collision is blocking the boulder from falling");
//             if (GameLogic.Instance.getGameObjectAtCoordinates(movePoint.position + Vector3.down).CompareTag("Boulder"))
//             {
//                 Debug.Log("Hitting another boulder");
//
//                 if (GameLogic.Instance.getGameObjectAtCoordinates(movePoint.position + Vector3.left) == null && 
//                     GameLogic.Instance.getGameObjectAtCoordinates(movePoint.position + Vector3.down + Vector3.left) == null)
//                 {
//                     // Fall left
//                     return new Vector3(-1, -1, 0);
//                 } if (GameLogic.Instance.getGameObjectAtCoordinates(movePoint.position + Vector3.right) == null && 
//                       GameLogic.Instance.getGameObjectAtCoordinates(movePoint.position + Vector3.down + Vector3.right) == null)
//                 {
//                     // Fall right
//                     return new Vector3(1, -1, 0);
//                 }
//             }
//             return Vector3.zero;
//         }
//         if (movePoint.position + Vector3.down == PlayerMovementScript.Instance.transform.position)
//         {
//             Debug.Log("Player is blocking the boulder from falling");
//             return Vector3.zero;
//         }
//         
//         // Debug.Log("Returning 1");
//         // 0: none, 1: down, 2: left, 3: right
//         return Vector3.down;
//     }
//     
//     bool AreDictionariesDifferent<TKey, TValue>(Dictionary<TKey, TValue> a, Dictionary<TKey, TValue> b)
//     {
//         if (a.Count != b.Count) return true;
//
//         foreach (var kvp in a)
//         {
//             if (!b.TryGetValue(kvp.Key, out TValue valueB)) return true;
//
//             if (!EqualityComparer<TValue>.Default.Equals(kvp.Value, valueB)) return true;
//         }
//
//         return false; // Dictionaries are equal
//     }
//
//     public void BoulderStateModifier()
//     {
//         _entityInArea.Clear();
//         Vector3[] allTrigger = _triggerFar.Concat(_triggerNear).ToArray();
//         foreach (Vector3 direction in allTrigger)
//         {
//             Collider2D entity = Physics2D.OverlapPoint(transform.position + direction, _layerStopsMovement);
//             if (entity)
//             {
//                 _entityInArea.Add(direction, entity.gameObject);
//             }
//         }
//         
//         if (AreDictionariesDifferent(_entityInArea, _entityInAreaBefore))
//         {
//             // Left the trigger
//             if (_entityInArea.Count < _entityInAreaBefore.Count)
//             {
//                 //fall
//                 print("Calling left trigger");
//                 _isTriggeredNear = true;
//                 isTickTicking = CanFall() != Vector3.zero;
//             }
//             
//             // Step onto farside trigger
//             foreach (Vector3 direction in _triggerFar)
//             {
//                 Collider2D entity = Physics2D.OverlapPoint(transform.position + direction, _layerStopsMovement);
//                 if (
//                     (entity && !_entityInAreaBefore.ContainsKey(direction)) 
//                     ||
//                     (entity &&
//                      _entityInAreaBefore.TryGetValue(direction, out var beforeEntity) &&
//                      _entityInArea.TryGetValue(direction, out var currentEntity) &&
//                      beforeEntity != currentEntity)
//                     )
//                 {
//                     if (_isTriggeredNear)
//                     {
// 					    print("Calling farside trigger");
//                         // fall
//                         isTickTicking = CanFall() != Vector3.zero;
//                     }
//                 }
//             }
//
//             // Step onto nearside trigger
//             foreach (Vector3 direction in _triggerNear)
//             {
//                 Collider2D entity = Physics2D.OverlapPoint(transform.position + direction, _layerStopsMovement);
//                 if (entity)
//                 {
//                     _isTriggeredNear = true;
//                 }
//             }
//         }
//         else
//         {
//             _isTriggeredNear = false;
//         }
//         
//         _entityInAreaBefore = new Dictionary<Vector3, GameObject>(_entityInArea);
//     }
//
//     // public void BoulderStateModifier()
//     // {
//     //     if (!triggerScript.entityInBoulderArea) return;
//     //
//     //     if (triggerScript.triggerCount == 3)
//     //     {
//     //         isTickTicking = CanFall() != Vector3.zero;
//     //         _moveDirection = CanFall();
//     //     }
//     //
//     //     if (triggerScript.triggerCount == 2)
//     //     {
//     //         if (triggerScript.entityInBoulderAreaRelativePos == new Vector3(1, -1, 0) ||
//     //             triggerScript.entityInBoulderAreaRelativePos == new Vector3(-1, -1, 0))
//     //         {
//     //             isTickTicking = CanFall() != Vector3.zero;
//     //             _moveDirection = CanFall();
//     //         }
//     //     }
//     // }
//
//     
//     // Called at movement end tick
//     // public void CheckEntityEnter()
//     // {
//     //     if (!triggerScript.entityInBoulderArea) return;
//     //     
//     //     Vector3 playerBoulderPosDiff = triggerScript.entityInBoulderArea.transform.position - movePoint.position;
//     //     
//     //     // If boulder is already triggered
//     //     if (isTriggeredTwice)
//     //     {
//     //         isTickMoving = CanFall() > 0;
//     //         _moveDirection = CanFall();
//     //     } else if (isTriggered && Mathf.Max(Mathf.Abs(playerBoulderPosDiff.x), Mathf.Abs(playerBoulderPosDiff.y)) > 1)
//     //     {
//     //         // If player leave boulder's trigger 
//     //         isTriggeredTwice = true;
//     //         isTickMoving = CanFall() > 0;
//     //         _moveDirection = CanFall();
//     //     }
//     //     
//     //     // Check if boulder is triggered
//     //     if (Mathf.Max(Mathf.Abs(playerBoulderPosDiff.x), Mathf.Abs(playerBoulderPosDiff.y)) == 1)
//     //     {
//     //         if (isTriggered)
//     //         {
//     //             isTriggeredTwice = true;
//     //         }
//     //         isTriggered = true;
//     //         if (playerBoulderPosDiff == Vector3.down)
//     //         {
//     //             isTriggeredTwice = true;
//     //         }
//     //
//     //         if (isTriggered && PlayerMovementScript.Instance.lastMovementDirection == Vector3.down && playerBoulderPosDiff.y == -1)
//     //         {
//     //             // If player goes down immediately inside boulder's trigger area
//     //             isTriggeredTwice = true;
//     //             isTickMoving = CanFall() > 0;
//     //             _moveDirection = CanFall();
//     //         }
//     //     }
//     //     
//     //     if (isTickMoving)
//     //     {
//     //         switch (_moveDirection)
//     //         {
//     //             case 1:
//     //                 movePoint.transform.position += Vector3.down;
//     //                 break;
//     //             case 2:
//     //                 movePoint.transform.position += Vector3.down + Vector3.left;
//     //                 break;
//     //             case 3:
//     //                 movePoint.transform.position += Vector3.down + Vector3.right;
//     //                 break;
//     //         }
//     //     }
//     // }
//
//     public void OnTickEnd()
//     {
//         BoulderStateModifier();
//     }
//
//     private void Awake()
//     {
//         // triggerScript = GetComponentInChildren<BoulderTriggerScript>();
//         // _playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementScript>();
//         _layerStopsMovement = LayerMask.GetMask("Collision");
//         
//         movePoint = transform.GetChild(0);
//         movePoint.parent = null;
//     }
//
//     private void Start()
//     {
//         _mapHoles = GameLogic.Instance.GetAllChildrenOfTag<HoleScript>("Hole");
//         
//         Vector3[] allTrigger = _triggerFar.Concat(_triggerNear).ToArray();
//         foreach (Vector3 direction in allTrigger)
//         {
//             Collider2D entity = Physics2D.OverlapPoint(transform.position + direction, _layerStopsMovement);
//             if (entity)
//             {
//                 _entityInArea.Add(direction, entity.gameObject);
//                 _entityInAreaBefore.Add(direction, entity.gameObject);
//             }
//         }
//         print(_entityInArea.Count);
//     }
//
//     private void Update()
//     {
//         // Animation moving
//         transform.position = Vector3.MoveTowards(transform.position, movePoint.position, 5f * Time.deltaTime);
//         if (!isTickTicking) return;
//         
//         if (IsStationary())
//         {
//             foreach (var hole in _mapHoles)
//             {
//                 if (hole.GetComponent<Collider2D>().IsTouching(GetComponent<Collider2D>()))
//                 {
//                     hole.Fill();
//                     Destroy(gameObject);
//                     return;
//                 }
//             }
//             
//             _moveDirection = CanFall();
//             Move(_moveDirection);
//             isTickTicking = CanFall() != Vector3.zero;
//         }
//     }
//
//     public bool IsStartTicking { get; set; }
//     public bool IsEndTicking { get; set; }
//
//     public void OnStartTick(Vector3 playerMoveDir)
//     {
//         throw new NotImplementedException();
//     }
//
//     public void OnEndTick()
//     {
//         throw new NotImplementedException();
//     }
// }
