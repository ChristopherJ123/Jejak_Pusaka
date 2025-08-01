// using UnityEngine;
//
// public class CrateScript : MonoBehaviour, ITickable, IMoveable
// {
//     private LayerMask _layerStopsMovement;
//     private Transform _movePoint;
//     
//     public bool IsStartTicking { get; set; }
//     public bool IsEndTicking { get; set; }
//
//     public Vector3 LastMoveDir { get; set; }
//     public bool IsNextTickScheduled { get; set; }
//     public Vector3 ScheduledMoveDir { get; set; }
//
//     /// <summary>
//     /// Crate is idle.
//     /// </summary>
//     /// <returns>bool</returns>
//     public bool IsStationary()
//     {
//         return Vector3.Distance(transform.position, _movePoint.position) == 0;
//     }
//
//     public bool IsPlayerPushing(Vector3 moveDirection)
//     {
//         // First check if player is heading toward this specific crate
//         // print(PlayerMovementScript.Instance.movePoint.position + " VS " + PlayerMovementScript.Instance.transform.position);
//         
//         // IsNextTickScheduled artinya player baru saja klik tombol buat gerak, artinya player moving.
//         // Kalau false artiyna player sedang tidak bergerak, dan entity lain yang bergerak seperti boulder
//         
//         // print(transform.name + " " + PlayerMovementScript.Instance.isScheduledMoveAndBeforeTickEnd + " " +
//         //       (PlayerMovementScript.Instance.transform.position + moveDirection == transform.position));
//         return PlayerMovementScript.Instance.IsNextTickScheduled &&
//                PlayerMovementScript.Instance.transform.position + moveDirection == transform.position;
//     }
//     
//     public bool CanMove(Vector3 moveDirection)
//     {
//         if (!Physics2D.OverlapPoint(_movePoint.transform.position + moveDirection, _layerStopsMovement))
//         {
//             return true;
//         }
//         return false;
//     }
//
//     public void ScheduleMove(Vector3 moveDir)
//     {
//         IsNextTickScheduled = true;
//         ScheduledMoveDir = moveDir;
//     }
//
//     public void DoScheduledMove()
//     {
//         if (IsNextTickScheduled)
//         {
//             Move(ScheduledMoveDir);
//             LastMoveDir = ScheduledMoveDir;
//             IsNextTickScheduled = false;
//             ScheduledMoveDir = Vector3.zero;
//         }
//     }
//
//     public void Move(Vector3 moveDir)
//     {
//         _movePoint.transform.position += moveDir;
//         LastMoveDir = moveDir;
//         IsStartTicking = true;
//     }
//     
//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         _layerStopsMovement = LayerMask.GetMask("Collision");
//         
//         _movePoint = transform.GetChild(0);
//         _movePoint.parent = null;
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         // Animation moving
//         if (IsStartTicking)
//         {
//             transform.position = Vector3.MoveTowards(transform.position, _movePoint.position, 5f * Time.deltaTime);
//             if (IsStationary())
//             {
//                 IsStartTicking = false;
//             }     
//         }
//     }
//
//     public void OnStartTick(Vector3 playerMoveDir)
//     {
//         // print(transform.name + " start tick");
//         DoScheduledMove();
//
//         if (IsPlayerPushing(playerMoveDir))
//         {
//             if (CanMove(playerMoveDir))
//             {
//                 Move(playerMoveDir);
//             }
//         }
//     }
//
//     public void PostStartTick(Vector3 playerMoveDir)
//     {
//         
//     }
//
//     public void OnEndTick()
//     {
//         
//     }
//
//     public void PostEndTick()
//     {
//         
//     }
// }
