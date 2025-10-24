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
    private static AudioClip[] _pinballTriggerSounds;

    /// <summary>
    /// Redirect a Moveable's move dir if hit a pinball
    /// </summary>
    /// <param name="entity">Current entity</param>
    /// <param name="initialMoveDir">Initial move direction</param>
    /// <returns>Redirected moveDir if any else initialMoveDir.</returns>
    public static Vector3 RedirectMoveFromPinballIfAny(GameObject entity, Vector3 initialMoveDir)
    {
        if (_allPinballs == null) return initialMoveDir;
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
                    GameLogic.PlayAudioClipRandom(_pinballTriggerSounds);
                    // Move changed
                    return trigger;
                }
                if (entity.transform.position + initialMoveDir == pinball.transform.position)
                {
                    GameLogic.PlayAudioClipRandom(_pinballTriggerSounds);
                    // Move mirrored back
                    print($"{entity.name} move should be mirrored back.");
                    return -initialMoveDir;
                }
            }
        }
        return initialMoveDir;
    }

    private void Start()
    {
        _allPinballs = GameObject.FindGameObjectsWithTag("Pinball");
        if (_allPinballs.Length > 0)
        {
            _pinballTriggerSounds = _allPinballs[0].GetComponent<PinballScript>().triggerSounds;
        }
    }
}
