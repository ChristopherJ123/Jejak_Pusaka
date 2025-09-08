using System;
using UnityEngine;

public interface ITickable
{
    /// <summary>
    /// The next predetermined random bit to use
    /// </summary>
    bool NextRandom { get; set; }
    bool IsNextTickDestroyScheduled { get; set; }
    bool IsNextTickMoveScheduled { get; set; }
    /// <summary>
    /// Complements IsNextTickScheduled or IsNextTickDestroyed if needed extra Tick after scheduling finishes.
    /// </summary>
    bool DoExtraTickLoop { get; set; }
    bool IsStartTicking { get; set; }
    bool IsEndTicking { get; set; }
    /// <summary>
    /// Use OnStartTick to indicate the logic that works with random ordering of all Tickable elements.
    /// </summary>
    /// <param name="playerMoveDir">Player move direction.</param>
    void OnStartTick(Vector3 playerMoveDir);
    /// <summary>
    /// Use PostStartTick to indicate the logic that's called after OnStartTick is finished. This method should
    /// only implement any logic that happens instantaneously and configurations that don't depend on random ordering.
    /// Only use for configuration variables only.
    /// <br/>
    /// <br/>
    /// Primarily should be used for setting non-random ordering dependant variables like IsNextTickScheduled so that
    /// other tickables could use another tickable's IsNextTickScheduled without interfering with the random ordering
    /// messing configuration variables up.
    /// </summary>
    /// <param name="playerMoveDir">Player move direction.</param>
    void PostStartTick(Vector3 playerMoveDir);
    void OnEndTick();
    void PostEndTick();
}