// using UnityEngine;
//
// public class PlayerMovementScript : MonoBehaviour
// {
//     public Transform movePoint;
//     [SerializeField] private float moveSpeed = 5f;
//     private float blockMovementTimer = 0f;
//
//     private bool hasMoved = false;
//     public Vector3 lastMovementDirection;
//
//     public void move(Vector3 moveDirection)
//     {
//         if (Vector3.Distance(transform.position, movePoint.position) == 0 && !GameLogic.Instance.waitForTick)
//         {
//             if (hasMoved)
//             {
//                 // Movement end tick
//                 GameLogic.Instance.EndTick();
//                 hasMoved = false;
//             }
//
//             if (!hasMoved && !GameLogic.Instance.waitForTick)
//             {
//                 // Movement start tick
//                 var canMove = GameLogic.Instance.StartTick(moveDirection);
//                 if (canMove)
//                 {
//                     movePoint.position += moveDirection;
//                     lastMovementDirection = moveDirection;
//                 }
//                 hasMoved = true;
//             }
//         }
//
//     }
//     
//     void Start()
//     {
//         movePoint.parent = null;
//     }
//
//     void Update()
//     {
//         transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
//         if (blockMovementTimer > 0f)
//         {
//             blockMovementTimer -= Time.deltaTime * 5;
//             blockMovementTimer = Mathf.Max(blockMovementTimer, 0f);
//         }
//
//         
//         if (Vector3.Distance(transform.position, movePoint.position) == 0 && !GameLogic.Instance.waitForTick && blockMovementTimer == 0f)
//         {
//             if (hasMoved)
//             {
//                 // Movement end tick
//                 GameLogic.Instance.EndTick();
//                 hasMoved = false;
//             }
//
//             if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1 && !hasMoved && !GameLogic.Instance.waitForTick)
//             {
//                 // Movement start tick
//                 var canMove = GameLogic.Instance.StartTick(new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f));
//                 if (canMove)
//                 {
//                     movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
//                     lastMovementDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
//                 }
//                 else
//                 {
//                     blockMovementTimer += 1;
//                 }
//                 hasMoved = true;
//             }
//
//             if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1 && !hasMoved && !GameLogic.Instance.waitForTick)
//             {
//                 var canMove = GameLogic.Instance.StartTick(new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f));
//                 if (canMove)
//                 {
//                     movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
//                     lastMovementDirection = new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
//                 }
//                 else
//                 {
//                     blockMovementTimer += 1;
//                 }
//                 hasMoved = true;
//             }
//         }
//     }
// }
