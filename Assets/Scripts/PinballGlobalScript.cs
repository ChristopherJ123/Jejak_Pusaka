using UnityEngine;

public class PinballGlobalScript : MonoBehaviour
{
    private static GameObject[] _allPinballs;
    private static readonly Vector3[] TriggerNear =
    {
        Vector3.up,
        Vector3.down,
        Vector3.left,
        Vector3.right,
    };

    /// <summary>
    /// Redirect a Moveable's move dir if hit a pinball
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="initialMoveDir"></param>
    /// <returns>Redirected moveDir if any else initialMoveDir.</returns>
    public static Vector3 PinballMoveRedirectIfAny(GameObject entity, Vector3 initialMoveDir)
    {
        if (entity.TryGetComponent<IMoveable>(out var moveable))
        {
            if (!moveable.IsPinballMoveable)
            {
                return initialMoveDir;
            }
        }
        foreach (var pinball in _allPinballs)
        {
            foreach (var trigger in TriggerNear)
            {
                if (entity.transform.position + initialMoveDir == pinball.transform.position + trigger)
                {
                    if (initialMoveDir == -trigger)
                    {
                        return initialMoveDir;
                    }
                    return trigger;
                }
                if (entity.transform.position + initialMoveDir == pinball.transform.position)
                {
                    return -initialMoveDir;
                }
            }
        }
        return initialMoveDir;
    }

    private void Start()
    {
        _allPinballs = GameObject.FindGameObjectsWithTag("Pinball");
    }
}
