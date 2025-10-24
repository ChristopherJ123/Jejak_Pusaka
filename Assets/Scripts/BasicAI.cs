using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// A* Algorithm
public class BasicAI : BasicLivingEntity
{
    private readonly Vector3[] _moveDirections = {Vector3.up, Vector3.down, Vector3.left, Vector3.right};
    private List<Node> GetNeighboursFromNode(Node node)
    {
        List<Node> neighbours = new List<Node>();
        foreach (var moveDir in _moveDirections)
        {
            var collisionPoint = Physics2D.OverlapPoint(node.WorldPos + moveDir, LayerStopsMovement);
            if (GameLogic.IsSpaceAvailable(node.WorldPos + moveDir) || (collisionPoint && collisionPoint.TryGetComponent<BasicLivingEntity>(out _)))
            {
                print($"Adding neighbour {moveDir} to {node.WorldPos}");
                neighbours.Add(new Node(node.WorldPos + moveDir));
            }
        }
        return neighbours;
    }

    private List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> pathPoints = new List<Vector3>();
        var currentNode = endNode;
        while (currentNode.WorldPos != startNode.WorldPos)
        {
            pathPoints.Add(currentNode.WorldPos);
            currentNode = currentNode.Parent;
        }

        pathPoints.Reverse();
        return pathPoints;
    }
    
    private Vector3 SnapToGrid(Vector3 worldPosition)
    {
        // Round to the nearest integer position
        return new Vector3(
            Mathf.Floor(worldPosition.x) + 0.5f,
            Mathf.Floor(worldPosition.y) + 0.5f,
            0
        );
    }
    
    private List<Vector3> FindShortestPathToPlayer()
    {
        Physics2D.SyncTransforms(); // <-- ADD THIS LINE // TETEP GAK ISO ANOMALI COKK, Unity ne sumpah gak jelas iki. Jadi pas awal di play aman" ae lancar, pas di restart scene e pake SceneManager.LoadScene(SceneManager.GetActiveScene().name); LANGSUNG EROR KABEH COKKK ai ne di first move player gak jalan entah knp, tapi lek kamu baru start game AI ne jalan kok pas player pertama kali jalan.
        
        // Snap positions to the grid
        Vector3 startPos = SnapToGrid(transform.position);
        Vector3 targetPos = SnapToGrid(PlayerScript.Instance.transform.position);
        
        Node startNode = new Node(startPos);
        Node targetNode = new Node(targetPos);
        
        List<Node> openList = new List<Node>{startNode};
        HashSet<Vector3> closedSet = new HashSet<Vector3>();
        Node bestNodeSoFar = startNode; // keep track of closest node to goal

        while (openList.Count > 0)
        {
            // Find node with the lowest FCost
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentNode.FCost)
                {
                    currentNode = openList[i];
                }
            }
            
            openList.Remove(currentNode);
            closedSet.Add(currentNode.WorldPos);
            
            // Update bestNodeSoFar if we're closer to goal
            if (currentNode.HCost < bestNodeSoFar.HCost)
                bestNodeSoFar = currentNode;
            
            // Check if we reached the goal
            if (currentNode.WorldPos == targetNode.WorldPos)
            {
                print("FOUND PLAYER");
                return RetracePath(startNode, currentNode);
            }

            
            // Expand neighbors
            foreach (var neighborNode in GetNeighboursFromNode(currentNode))
            {
                if (closedSet.Contains(neighborNode.WorldPos))
                    continue;
                
                var newGCost = currentNode.GCost + Vector3.Distance(currentNode.WorldPos, neighborNode.WorldPos);

                // Kalau GCost lebih rendah atau tidak ada di openList
                if (newGCost < neighborNode.GCost || openList.All(n => n.WorldPos != neighborNode.WorldPos))
                {
                    neighborNode.Parent = currentNode;
                    neighborNode.GCost = newGCost;
                    neighborNode.HCost = Vector3.Distance(neighborNode.WorldPos, targetNode.WorldPos);
                    
                    if (openList.All(n => n.WorldPos != neighborNode.WorldPos))
                        openList.Add(neighborNode);
                }
            }
        }
        
        // No full path found — fallback to closest reachable node
        if (bestNodeSoFar != startNode)
            return RetracePath(startNode, bestNodeSoFar);
        
        // Totally stuck
        return new List<Vector3>();
    }

    public override void ScheduleAutoMove()
    {
        base.ScheduleAutoMove();
        var path = FindShortestPathToPlayer();
        if (path.Count > 0)
        {
            var scheduledMoveDir = path[0] - transform.position;
            if (CanMoveOrRedirect(ref scheduledMoveDir))
            {
                print("Auto move is scheduled");
                ScheduleMove(scheduledMoveDir);
            }
            else
            {
                print("Auto move schedule failed");
            }
        }
        else
        {
            print("Auto move schedule failed");
        }
    }
    
    public override void OnStartTick()
    {
        // print("Player start tick");
        if (IsNextTickDestroyScheduled)
        {
            GameLogic.PlayAudioClipRandom(destroySounds);
            gameObject.SetActive(false);
        }

        var scheduledMoveDir = ScheduledMoveDir;
        if (CanMoveOrRedirect(ref scheduledMoveDir))
        {
            print($"{transform.name} can move to {scheduledMoveDir}");
            DoScheduledMove();
        }
        // Physics2D.SyncTransforms();
        StartTickPosition = transform.position;
    }

}