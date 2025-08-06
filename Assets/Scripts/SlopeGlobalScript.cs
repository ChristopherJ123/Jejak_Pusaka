using UnityEngine;

public class SlopeGlobalScript : MonoBehaviour
{
    private static SlopeScript[] _allSlopes;

    public static Vector3 MoveRedirectFromSlopeIfAny(GameObject entity, Vector3 initialMoveDir)
    {
        if (entity.TryGetComponent<IMoveable>(out var moveable))
        {
            if (!moveable.IsSlopeMoveable)
            {
                return initialMoveDir;
            }
        }
        foreach (var slope in _allSlopes)
        {
            // First check entity hits this slope, then it checks if the distance is 1 (aka. is not moving diagonally)
            if (entity.transform.position + initialMoveDir == slope.transform.position &&
                Mathf.Approximately(Vector3.Distance(Vector3.zero, initialMoveDir), 1))
            {
                if (slope.slopeDirection == Vector2.right)
                {
                    // print("Hitting a right slope");
                    if (initialMoveDir == Vector3.right || initialMoveDir == Vector3.left)
                    {
                        // Rotate 90 clockwise
                        var newDirection = new Vector3(initialMoveDir.y, -initialMoveDir.x);
                        if (Physics2D.OverlapPoint(entity.transform.position + newDirection, LayerMask.GetMask("Collision")))
                        {
                            return initialMoveDir;
                        }
                        return newDirection + initialMoveDir;
                    }
                    if (initialMoveDir == Vector3.up || initialMoveDir == Vector3.down)
                    {
                        // Rotate 90 CCW
                        var newDirection = new Vector3(-initialMoveDir.y, initialMoveDir.x);
                        if (Physics2D.OverlapPoint(entity.transform.position + newDirection, LayerMask.GetMask("Collision")))
                        {
                            return initialMoveDir;
                        }
                        return newDirection + initialMoveDir;
                    }
                } else if (slope.slopeDirection == Vector2.left)
                {
                    if (initialMoveDir == Vector3.right || initialMoveDir == Vector3.left)
                    {
                        // Rotate 90 CCW
                        var newDirection = new Vector3(-initialMoveDir.y, initialMoveDir.x);
                        if (Physics2D.OverlapPoint(entity.transform.position + newDirection, LayerMask.GetMask("Collision")))
                        {
                            return initialMoveDir;
                        }
                        return newDirection + initialMoveDir;
                    }
                    if (initialMoveDir == Vector3.up || initialMoveDir == Vector3.down)
                    {
                        // Rotate 90 clockwise
                        var newDirection = new Vector3(initialMoveDir.y, -initialMoveDir.x);
                        if (Physics2D.OverlapPoint(entity.transform.position + newDirection, LayerMask.GetMask("Collision")))
                        {
                            return initialMoveDir;
                        }
                        return newDirection + initialMoveDir;
                    }
                }
            }
        }
        return initialMoveDir;
    }

    private void Start()
    {
        _allSlopes = FindObjectsByType<SlopeScript>(FindObjectsSortMode.None);
    }
}