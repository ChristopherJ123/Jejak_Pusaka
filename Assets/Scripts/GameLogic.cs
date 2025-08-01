using System;
using System.Collections.Generic;
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

    public GameObject getGameObjectAtCoordinates(Vector3 coordinates)
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

    public List<T> GetAllChildrenOfTag<T>(string tag)
    {
        List<T> childrens = new List<T>();

        foreach (Transform child in transform)
        {
            if (child.CompareTag(tag))
            {
                T component = child.GetComponent<T>();
                if (component != null) childrens.Add(component);
            }
        }
        
        return childrens;
    }

    /// <summary>
    /// Determine whether Player can move in a certain direction.
    /// </summary>
    /// <param name="playerMoveDir">Player move direction</param>
    /// <returns></returns>
    public bool PlayerMoveCondition(Vector3 playerMoveDir)
    {
        var boulders = GetAllChildrenOfTag<BoulderScript>("Boulder");
        var crates = GetAllChildrenOfTag<CrateScript>("Crate");

        foreach (var boulder in boulders)
        {
            if (boulder.IsPlayerPushing(playerMoveDir))
            {
                if (boulder.CanMove(playerMoveDir))
                {
                    return true;
                }
                return false;
            }
        }

        foreach (var crate in crates)
        {
            // print(crate.transform.name);
            if (crate.IsPlayerPushing(playerMoveDir))
            {
                // print(crate.transform.name + " IsPlayerPushing");
                if (crate.CanMove(playerMoveDir))
                {
                    // print(crate.transform.name + " CanMove");
                    return true;
                }
                else
                {
                    print(crate.transform.name + " Can't Move");
                }
                return false;
            }
        }

        // print("Returning true for normal movement");
        return true;
    }
    
    /// <summary>
    /// Start tick.
    /// </summary>
    /// <param name="playerMoveDir">Player move direction</param>
    /// <param name="excludeTypes">Entity types to exclude from start ticking</param>
    /// <returns></returns>
    public void StartTick(Vector3 playerMoveDir, params Type[] excludeTypes)
    {
        waitingForAllStartTickToFinish = true;
        var allTickables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ITickable>();

        foreach (var tickable in allTickables)
        {
            if (excludeTypes.Length > 0 && excludeTypes.Contains(tickable.GetType()))
            {
                continue;
            }
            tickable.OnStartTick(playerMoveDir);
        }
    }

    public void EndTick(params Type[] excludeTypes)
    {
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
            
            var tickables = allTickables as ITickable[] ?? allTickables.ToArray();
            foreach (var tickable in tickables)
            {
                if (tickable.IsStartTicking)
                {
                    return;
                }
            }
            waitingForAllStartTickToFinish = false;
            
            // After start tick, do all the post start tick methods
            // Remember: This method should only contain configuration variables only. This method should only be used
            // for functions that cannot be achievable with random ordering from the former Start Tick method.
            foreach (var tickable in tickables)
            {
                tickable.PostStartTick(PlayerMovementScript.Instance.LastMoveDir);
            }
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
                    StartTick(PlayerMovementScript.Instance.LastMoveDir);
                    timeToCheckSchedule = false;
                    return;
                }
            }
            timeToCheckSchedule = false;
        }
    }
}
