// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class GameLogicOld : MonoBehaviour
// {
//     public static GameLogic Instance;
//     public bool waitForEndTickToFinish = false;
//     List<ITickable> _tickablesEntities = new List<ITickable>();
//     
//     void Awake()
//     {
//     }
//
//     public GameObject getGameObjectAtCoordinates(Vector3 coordinates)
//     {
//         foreach (Transform child in transform)
//         {
//             if (child.position == coordinates)
//             {
//                 return child.gameObject;
//             }
//         }
//         return null;
//     }
//
//     public List<T> GetAllChildrenOfTag<T>(string tag)
//     {
//         List<T> childrens = new List<T>();
//
//         foreach (Transform child in transform)
//         {
//             if (child.CompareTag(tag))
//             {
//                 T component = child.GetComponent<T>();
//                 if (component != null) childrens.Add(component);
//             }
//         }
//         
//         return childrens;
//     }
//
//     /// <summary>
//     /// Determine whether Player can start a tick (can move). If cannot then don't move any crates/entity.
//     /// If can then will schedule a entity movement operation.
//     /// </summary>
//     /// <param name="playerMoveDir">Player move direction</param>
//     /// <returns></returns>
//     public bool PreStartTickConditions(Vector3 playerMoveDir)
//     {
//         var boulders = GetAllChildrenOfTag<BoulderScript>("Boulder");
//         var crates = GetAllChildrenOfTag<CrateScript>("Crate");
//
//         foreach (var boulder in boulders)
//         {
//             if (boulder.IsPlayerPushing(playerMoveDir))
//             {
//                 if (boulder.CanMove(playerMoveDir))
//                 {
//                     boulder.ScheduleMove(playerMoveDir);
//                     return true;
//                 }
//                 return false;
//             }
//         }
//
//         foreach (var crate in crates)
//         {
//             if (crate.IsPlayerPushing(playerMoveDir))
//             {
//                 if (crate.CanMove(playerMoveDir))
//                 {
//                     crate.ScheduleMove(playerMoveDir);
//                     return true;
//                 }
//                 return false;
//             }
//         }
//
//         return true;
//     }
//     
//     /// <summary>
//     /// Start tick.
//     /// </summary>
//     /// <param name="playerMoveDir">Player move direction</param>
//     /// <returns></returns>
//     public void StartTick(Vector3 playerMoveDir)
//     {
//         var boulders = GetAllChildrenOfTag<BoulderScript>("Boulder");
//         var crates = GetAllChildrenOfTag<CrateScript>("Crate");
//
//         foreach (var boulder in boulders)
//         {
//             if (boulder.ScheduledToMove)
//             {
//                 boulder.Move();
//             }
//         }
//
//         foreach (var crate in crates)
//         {
//             if (crate.ScheduledToMove)
//             {
//                 crate.Move();
//             }
//         }
//     }
//
//
//     // Called each movement
//     public void EndTick()
//     {
//         waitForEndTickToFinish = true;
//
//         var boulders = GetAllChildrenOfTag<BoulderScript>("Boulder");
//         var bouldersToDelete = new List<BoulderScript>();
//         var crates = GetAllChildrenOfTag<CrateScript>("Crate");
//         var cratesToDelete = new List<CrateScript>();
//         var holes = GetAllChildrenOfTag<HoleScript>("Hole");
//         var ice = GetComponentInChildren<IceScript>();
//
//         foreach(var boulder in boulders)
//         {
//             boulder.OnTickEnd();
//         }
//         
//         if (ice.isEntitySteppedOn)
//         {
//             ice.Tick();
//         }
//         
//     }
//
//     public void EndTick(params ITickable[] tickables)
//     {
//         foreach (var tickable in tickables)
//         {
//             tickable.OnEndTick();
//         }
//     }
//     private void Update()
//     {
//         if (waitForEndTickToFinish)
//         {
//             // Wait for boulders to finish ticking
//             var boulders = GetAllChildrenOfTag<BoulderScript>("Boulder");
//             var boulderIsTicking = false;
//             foreach (var boulder in boulders)
//             {
//                 boulderIsTicking = boulder.isTickTicking;
//                 if (boulderIsTicking) break;
//             }
//             
//             // Wait for ice to finish ticking
//             var iceIsTicking = GetComponentInChildren<IceScript>().isTickTicking;
//             
//             if (!boulderIsTicking && !iceIsTicking) waitForEndTickToFinish = false;
//         }
//     }
// }
