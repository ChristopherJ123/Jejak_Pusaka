using System.Linq;
using UnityEngine;

public class SlopeGlobalScript : MonoBehaviour
{
    private static LayerMask _layerAllowsMovement;
    private static LayerMask _layerBlocksMovement;
    
    private static SlopeScript[] _allSlopes;
    private static BoulderScript[] _allBoulders;
    private static PinballScript[] _allPinballs;
    
    private static AudioClip[] _arrowTriggerSounds;

    public static Vector3 RedirectMoveFromSlopeIfAny(GameObject entity, Vector3 initialMoveDir)
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
                        if (!GameLogic.IsSpaceAvailable(entity.transform.position + newDirection))
                        {
                            return initialMoveDir;
                        }
                        if (entity.CompareTag("Arrow")) GameLogic.PlayAudioClipRandom(_arrowTriggerSounds);
                        return newDirection + initialMoveDir;
                    }
                    if (initialMoveDir == Vector3.up || initialMoveDir == Vector3.down)
                    {
                        // Rotate 90 CCW
                        var newDirection = new Vector3(-initialMoveDir.y, initialMoveDir.x);
                        if (!GameLogic.IsSpaceAvailable(entity.transform.position + newDirection))
                        {
                            return initialMoveDir;
                        }
                        if (entity.CompareTag("Arrow")) GameLogic.PlayAudioClipRandom(_arrowTriggerSounds);
                        return newDirection + initialMoveDir;
                    }
                } else if (slope.slopeDirection == Vector2.left)
                {
                    if (initialMoveDir == Vector3.right || initialMoveDir == Vector3.left)
                    {
                        // Rotate 90 CCW
                        var newDirection = new Vector3(-initialMoveDir.y, initialMoveDir.x);
                        if (!GameLogic.IsSpaceAvailable(entity.transform.position + newDirection))
                        {
                            return initialMoveDir;
                        }
                        if (entity.CompareTag("Arrow")) GameLogic.PlayAudioClipRandom(_arrowTriggerSounds);
                        return newDirection + initialMoveDir;
                    }
                    if (initialMoveDir == Vector3.up || initialMoveDir == Vector3.down)
                    {
                        // Rotate 90 clockwise
                        var newDirection = new Vector3(initialMoveDir.y, -initialMoveDir.x);
                        if (!GameLogic.IsSpaceAvailable(entity.transform.position + newDirection))
                        {
                            return initialMoveDir;
                        }
                        if (entity.CompareTag("Arrow")) GameLogic.PlayAudioClipRandom(_arrowTriggerSounds);
                        return newDirection + initialMoveDir;
                    }
                }
            }
        }
        return initialMoveDir;
    }
    
    public static Vector3 RedirectMoveFromBoulderIfAny(GameObject entity, Vector3 initialMoveDir)
    {
        if (entity.TryGetComponent<IMoveable>(out var moveable))
        {
            if (!moveable.IsBoulderSlopeMoveable)
            {
                return initialMoveDir;
            }
        }
        foreach (var boulder in _allBoulders)
        {
            if (!boulder) continue;
            Vector3 entityTargetPos = entity.transform.position + initialMoveDir;
            if (entityTargetPos == boulder.transform.position)
            {
                boulder.TryGetComponent<ITickable>(out var tickable);

                // Perpendicular directions
                Vector3 perp1, perp2;
                
                // Decide perpendicular directions based on initial move
                if (initialMoveDir == Vector3.up || initialMoveDir == Vector3.down)
                {
                    if (tickable.NextRandom)
                    {
                        perp1 = Vector3.right;
                        perp2 = Vector3.left;
                    }
                    else
                    {
                        perp1 = Vector3.left;
                        perp2 = Vector3.right;
                    }
                }
                else if (initialMoveDir == Vector3.right || initialMoveDir == Vector3.left)
                {
                    if (entity.CompareTag("Boulder")) return initialMoveDir;
                    if (tickable.NextRandom)
                    {
                        perp1 = Vector3.down;
                        perp2 = Vector3.up;
                    }
                    else
                    {
                        perp1 = Vector3.up;
                        perp2 = Vector3.down;
                    }
                }
                else
                {
                    // Fallback for diagonal or unsupported directions
                    return initialMoveDir;
                }
                tickable.NextRandom = !tickable.NextRandom;

                // First perpendicular check (e.g. left or up)
                if (GameLogic.IsSpaceAvailable(entity.transform.position + perp1) &&
                    GameLogic.IsSpaceAvailable(entity.transform.position + initialMoveDir + perp1))
                {
                    if (entity.CompareTag("Arrow")) GameLogic.PlayAudioClipRandom(_arrowTriggerSounds);
                    return initialMoveDir + perp1;
                }

                // Second perpendicular check (e.g. right or down)
                if (GameLogic.IsSpaceAvailable(entity.transform.position + perp2) &&
                    GameLogic.IsSpaceAvailable(entity.transform.position + initialMoveDir + perp2))
                {
                    if (entity.CompareTag("Arrow")) GameLogic.PlayAudioClipRandom(_arrowTriggerSounds);
                    return initialMoveDir + perp2;
                }

                // No valid redirect path found
                return initialMoveDir;
            }
        }

        // No boulder collision; move as usual
        return initialMoveDir;
    }
    
    public static Vector3 RedirectMoveFromPinballIfAny(GameObject entity, Vector3 initialMoveDir)
    {
        if (entity.TryGetComponent<IMoveable>(out var moveable))
        {
            if (!moveable.IsPinballSlopeMoveable)
            {
                return initialMoveDir;
            }
        }
        foreach (var pinball in _allPinballs)
        {
            Vector3 entityTargetPos = entity.transform.position + initialMoveDir;
            if (entityTargetPos == pinball.transform.position)
            {
                pinball.TryGetComponent<ITickable>(out var tickable);

                // Perpendicular directions
                Vector3 perp1, perp2;
                
                // Decide perpendicular directions based on initial move
                if (initialMoveDir == Vector3.up || initialMoveDir == Vector3.down)
                {
                    if (tickable.NextRandom)
                    {
                        perp1 = Vector3.right;
                        perp2 = Vector3.left;
                    }
                    else
                    {
                        perp1 = Vector3.left;
                        perp2 = Vector3.right;
                    }
                }
                else if (initialMoveDir == Vector3.right || initialMoveDir == Vector3.left)
                {
                    if (tickable.NextRandom)
                    {
                        perp1 = Vector3.down;
                        perp2 = Vector3.up;
                    }
                    else
                    {
                        perp1 = Vector3.up;
                        perp2 = Vector3.down;
                    }
                }
                else
                {
                    // Fallback for diagonal or unsupported directions
                    return initialMoveDir;
                }
                tickable.NextRandom = !tickable.NextRandom;

                // First perpendicular check (e.g. left or up)
                if (GameLogic.IsSpaceAvailable(entity.transform.position + perp1) &&
                    GameLogic.IsSpaceAvailable(entity.transform.position + initialMoveDir + perp1))
                {
                    // print("returning A");
                    if (entity.CompareTag("Arrow")) GameLogic.PlayAudioClipRandom(_arrowTriggerSounds);
                    return initialMoveDir + perp1;
                }

                // Second perpendicular check (e.g. right or down)
                if (GameLogic.IsSpaceAvailable(entity.transform.position + perp2) &&
                    GameLogic.IsSpaceAvailable(entity.transform.position + initialMoveDir + perp2))
                {
                    // print("returning B");
                    if (entity.CompareTag("Arrow")) GameLogic.PlayAudioClipRandom(_arrowTriggerSounds);
                    return initialMoveDir + perp2;
                }

                // No valid redirect path found
                print("Returning initialMoveDir");
                return initialMoveDir;
            }
        }

        // No boulder collision; move as usual
        return initialMoveDir;
    }

    private void Start()
    {
        _layerAllowsMovement = LayerMask.GetMask("Tile");
        _layerBlocksMovement = LayerMask.GetMask("Collision");
        
        _allSlopes = FindObjectsByType<SlopeScript>(FindObjectsSortMode.None);
        _allBoulders = GameObject.FindGameObjectsWithTag("Boulder").Select(go => go.GetComponent<BoulderScript>())
            .Where(component => component != null)
            .ToArray();
        _allPinballs = GameObject.FindGameObjectsWithTag("Pinball").Select(go => go.GetComponent<PinballScript>())
            .Where(component => component != null)
            .ToArray();
        var allArrows = FindObjectsByType<ArrowScript>(FindObjectsSortMode.None);
        if (allArrows.Length > 0)
        {
            _arrowTriggerSounds = allArrows[0].moveSounds;
        }
    }
}