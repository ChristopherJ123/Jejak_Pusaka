// using System;
// using UnityEngine;
// using UnityEngine.Serialization;
//
// public class IceScriptOld : MonoBehaviour, ITickable
// {
//     public bool isEntitySteppedOn;
//     private GameObject entity;
//
//     private bool CanMove(Vector3 moveDirection)
//     {
//         if (GameLogic.Instance.getGameObjectAtCoordinates(transform.position + moveDirection) == null)
//         {
//             return true;
//         }
//         return false;
//     }
//
//     /// <summary>
//     /// Ice's own ticking method.
//     /// </summary>
//     public void Tick()
//     {
//         if (isEntitySteppedOn && entity != null)
//         {
//             if (entity.CompareTag("Player") || entity.CompareTag("Crate"))
//             {
//                 isTickTicking = true;
//             }
//         }
//     }
//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.CompareTag("Player") || other.CompareTag("Crate"))
//         {
//             Debug.Log(other.tag);
//             entity = other.gameObject;
//             isEntitySteppedOn = true;
//         }
//     }
//
//     private void OnTriggerExit2D(Collider2D other)
//     {
//         entity = null;
//         isEntitySteppedOn = false;
//         isTickTicking = false;
//     }
//
//     private void Update()
//     {
//         if (!isTickTicking) return;
//         if (entity)
//         {
//             if (entity.CompareTag("Player"))
//             {
//                 print("Player should be sliding");
//                 var player = entity.GetComponent<PlayerMovementScript>();
//                 if (player.IsStationary())
//                 {
//                     if (player.CanMove(player.lastMovementDirection))
//                     {
//                         player.Move(player.lastMovementDirection);
//                     }
//                     else
//                     {
//                         isTickTicking = false;
//                     }
//                 }
//             }
//
//             if (entity.CompareTag("Crate"))
//             {
//                 var crate = entity.GetComponent<CrateScript>();
//                 if (crate.IsStationary())
//                 {
//                     if (crate.CanMove(PlayerMovementScript.Instance.lastMovementDirection))
//                     {
//                         crate.Move(PlayerMovementScript.Instance.lastMovementDirection);
//                     }
//                     else
//                     {
//                         isTickTicking = false;
//                     }
//                 }
//             }
//         }
//         else
//         {
//             isTickTicking = false;
//         }
//     }
//
//     public bool IsStartTicking { get; set; }
//     public bool IsEndTicking { get; set; }
//
//     public void OnStartTick(Vector3 playerMoveDir)
//     {
//         
//     }
//
//     public void OnEndTick()
//     {
//         var entity = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("Collision"));
//         if (entity)
//         {
//             IsEndTicking = true;
//             {
//                 print("Player should be sliding");
//                 var player = entity.GetComponent<PlayerMovementScript>();
//                 if (player.IsStationary())
//                 {
//                     if (player.CanMove(player.lastMovementDirection))
//                     {
//                         player.Move(player.lastMovementDirection);
//                     }
//                     else
//                     {
//                         IsEndTicking = false;
//                     }
//                 }
//             }
//             
//             if (entity.CompareTag("Crate"))
//             {
//                 var crate = entity.GetComponent<CrateScript>();
//                 if (crate.IsStationary())
//                 {
//                     if (crate.CanMove(PlayerMovementScript.Instance.lastMovementDirection))
//                     {
//                         crate.Move(PlayerMovementScript.Instance.lastMovementDirection);
//                     }
//                     else
//                     {
//                         IsEndTicking = false;
//                     }
//                 }
//             }
//         }
//     }
// }
