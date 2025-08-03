using System;
using System.Linq;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public static GameLogic Instance;
    public bool waitingForAllStartTickToFinish;
    public bool waitingForAllEndTickToFinish;
    public bool timeToCheckSchedule;
    
    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Helper Function to get game object at a certain coordinate in the world.
    /// </summary>
    /// <param name="coordinates">Game object coordinates</param>
    /// <returns></returns>
    public GameObject GetGameObjectAtCoordinates(Vector3 coordinates)
    {
        foreach (Transform child in transform)
        {
            if (child.position == coordinates)
            {
                return child.gameObject;
            }
        }
        return null;
    }
    
    /// <summary>
    /// Start tick.
    /// </summary>
    /// <param name="playerMoveDir">Player move direction</param>
    /// <param name="excludeTypes">Entity types to exclude from start ticking</param>
    /// <returns></returns>
    public void StartTick(Vector3 playerMoveDir, params Type[] excludeTypes)
    {
        // print("start tick");
        waitingForAllStartTickToFinish = true;
        var allTickables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ITickable>();

        var tickables = allTickables as ITickable[] ?? allTickables.ToArray();
        foreach (var tickable in tickables)
        {
            if (excludeTypes.Length > 0 && excludeTypes.Contains(tickable.GetType()))
            {
                continue;
            }
            tickable.OnStartTick(playerMoveDir);
        }
        
        // After start tick, do all the post start tick methods
        // Remember: This method should only contain configuration variables only. This method should only be used
        // for functions that cannot be achievable with random ordering from the former Start Tick method.
        foreach (var tickable in tickables)
        {
            tickable.PostStartTick(PlayerScript.Instance.LastMoveDir);
        }
    }

    public void EndTick(params Type[] excludeTypes)
    {
        // print("end tick");
        waitingForAllEndTickToFinish = true;
        var allTickables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ITickable>();

        foreach (var tickable in allTickables)
        {
            if (excludeTypes.Length > 0 && excludeTypes.Contains(tickable.GetType()))
            {
                continue;
            }
            tickable.OnEndTick();
        }
    }
    private void Update()
    {
        if (waitingForAllStartTickToFinish)
        {
            // Kinda bad practice tho ngl, but there isn't a performance warning so
            var allTickables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<ITickable>();
            foreach (var tickable in allTickables)
            {
                if (tickable.IsStartTicking)
                {
                    return;
                }
            }
            waitingForAllStartTickToFinish = false;
            EndTick();
        }
        
        if (waitingForAllEndTickToFinish)
        {
            var allTickables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<ITickable>();

            var tickables = allTickables as ITickable[] ?? allTickables.ToArray();
            foreach (var tickable in tickables)
            {
                if (tickable.IsEndTicking)
                {
                    return;
                }
            }
            
            // After doing all End tich, now should do all Post end tick
            // Remember: This method should only contain configuration variables only. This method should only be used
            // for functions that cannot be achievable with random ordering from the former End Tick method.
            foreach (var tickable in tickables)
            {
                tickable.PostEndTick();
            }
            
            waitingForAllEndTickToFinish = false;
            timeToCheckSchedule = true;
        }

        if (timeToCheckSchedule)
        {
            // print("Is checking schedule");
            var allTickables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<ITickable>();
            
            foreach (var tickable in allTickables)
            {
                if (tickable.IsNextTickScheduled)
                {
                    // print("There is something scheduled");
                    StartTick(PlayerScript.Instance.ScheduledMoveDir);
                    timeToCheckSchedule = false;
                    return;
                }
            }
            timeToCheckSchedule = false;
        }
    }
}
