// using System;
// using UnityEngine;
// using Object = System.Object;
//
// public class BoulderScript : MonoBehaviour
// {
//     private GameObject player;
//     private PlayerMovement playerMovement;
//     private LayerMask layerStopsMovement;
//     
//     private BoulderTriggerScript boulderTriggerScript;
//     
//     [SerializeField]
//     private Transform movePoint;
//     public bool isTriggered = false;
//     public bool isTriggeredTwice = false;
//     public bool isTickMoving = false;
//
//     private bool isOverHole = false;
//     private HoleScript currentHole;
//
//     private bool CanFall()
//     {
//         if (Physics2D.OverlapPoint(movePoint.position + Vector3.down, layerStopsMovement))
//         {
//             Debug.Log("Collision is blocking the boulder from falling");
//             return false;
//         }
//         if (movePoint.position + Vector3.down == player.transform.position)
//         {
//             Debug.Log("Player is blocking the boulder from falling");
//             return false;
//         }
//         return true;
//     }
//
//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.CompareTag("Hole"))
//         {
//             isOverHole = true;
//             currentHole = other.GetComponent<HoleScript>();
//         }
//     }
//
//     // Called at movement start tick
//     public bool CanMoveBoulder(Vector3 moveDirection)
//     {
//         // Move boulder
//         if (playerMovement.movePoint.position + moveDirection == transform.position)
//         {
//             if (transform.position - player.transform.position == Vector3.right)
//             {
//                 if (!Physics2D.OverlapPoint(movePoint.transform.position + Vector3.right, layerStopsMovement))
//                 {
//                     movePoint.transform.position += Vector3.right;
//                     // Debug.Log("Boulder is moving right");
//                     return true;
//                 }
//                 return false;
//             }
//             if (transform.position - player.transform.position == Vector3.left)
//             {
//                 if (!Physics2D.OverlapPoint(movePoint.transform.position + Vector3.left, layerStopsMovement))
//                 {
//                     movePoint.transform.position += Vector3.left;
//                     // Debug.Log("Boulder is moving left");
//                     return true;
//                 }
//                 return false;
//             }
//             if (transform.position - player.transform.position == Vector3.down)
//             {
//                 if (!Physics2D.OverlapPoint(movePoint.transform.position + Vector3.down, layerStopsMovement))
//                 {
//                     movePoint.transform.position += Vector3.down;
//                     return true;
//                 }
//                 return false;
//             }
//
//             if (transform.position - player.transform.position == Vector3.up) return false;
//         }
//         
//         return true;
//     }
//     
//     // Called at movement end tick
//     public void CheckEntityEnter()
//     {
//         if (boulderTriggerScript.entityCollider == null) return;
//         
//         Vector3 playerBoulderPosDiff = boulderTriggerScript.entityCollider.transform.position - transform.position;
//         
//         // If boulder is already triggered
//         if (isTriggeredTwice)
//         {
//             isTickMoving = CanFall();
//         } else if (isTriggered && Mathf.Max(Mathf.Abs(playerBoulderPosDiff.x), Mathf.Abs(playerBoulderPosDiff.y)) > 1)
//         {
//             isTickMoving = CanFall();
//         }
//         
//         // Check if boulder is triggered
//         if (Mathf.Max(Mathf.Abs(playerBoulderPosDiff.x), Mathf.Abs(playerBoulderPosDiff.y)) == 1)
//         {
//             if (isTriggered)
//             {
//                 isTriggeredTwice = true;
//             }
//             isTriggered = true;
//             if (playerBoulderPosDiff == Vector3.down)
//             {
//                 isTriggeredTwice = true;
//             }
//
//             if (isTriggered && playerMovement.lastMovementDirection == Vector3.down && playerBoulderPosDiff.y == -1)
//             {
//                 isTriggeredTwice = true;
//                 isTickMoving = CanFall();
//             }
//         }
//     }
//
//     private void Awake()
//     {
//         boulderTriggerScript = GetComponentInChildren<BoulderTriggerScript>();
//         player = GameObject.FindGameObjectWithTag("Player");
//         playerMovement = player.GetComponent<PlayerMovement>();
//         layerStopsMovement = LayerMask.GetMask("Collision");
//         
//         movePoint = transform.GetChild(0);
//         movePoint.parent = null;
//     }
//
//     private void Start()
//     {
//         ObjectManager.Instance.RegisterBoulder(this);
//     }
//
//     private void Update()
//     {
//         // Animation moving
//         transform.position = Vector3.MoveTowards(transform.position, movePoint.position, 5f * Time.deltaTime);
//
//         if (Vector3.Distance(transform.position, movePoint.position) == 0)
//         {
//             if (isOverHole && Vector3.Distance(transform.position, currentHole.transform.position) == 0)
//             {
//                 currentHole.Fill();
//                 ObjectManager.Instance.UnregisterBoulder(this);
//                 Destroy(gameObject);
//             }
//             
//             if (isTickMoving)
//             {
//                 // Tick update
//                 isTickMoving = CanFall();
//                 
//                 if (CanFall())
//                 {
//                     // Debug.Log("[Tick] Boulder is falling");
//                     movePoint.transform.position += Vector3.down;
//                 }
//             }
//         }
//         
//         
//     }
// }
