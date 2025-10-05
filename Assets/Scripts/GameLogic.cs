using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public static GameLogic Instance;
    [HideInInspector]
    public AudioSource audioSource;

    public string levelName = "";
    public float speed = 6;
    public bool shouldSaveScore = true;
    
    public int Score { get; private set; }
    private int _numberOfTreasures;

    [HideInInspector]
    public bool isCurrentlyTicking;
    [HideInInspector]
    public bool waitingForAllStartTickToFinish;
    [HideInInspector]
    public bool waitingForAllEndTickToFinish;
    [HideInInspector]
    public bool timeToCheckSchedule;
    
    public static LayerMask LayerAllowsMovement;
    public static LayerMask LayerBlocksMovement;
    
    void Awake()
    {
        Instance = this;
        LayerAllowsMovement = LayerMask.GetMask("Tile");
        LayerBlocksMovement = LayerMask.GetMask("Collision");
        audioSource = GetComponent<AudioSource>();
        
        _numberOfTreasures = FindObjectsByType<TreasureScript>(FindObjectsSortMode.None).Length;
    }

    private void Start()
    {
        GameLogicUI.Instance.ShowLevelName(levelName);
        GameLogicUI.Instance.ShowScore(Score, GlobalGameManager.Instance.scorePerTreasure*_numberOfTreasures);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void GameOver(string message)
    {
        PlayerScript.Instance.isAlive = false;
        GameLogicUI.Instance.ShowDeathScreen(message);
    }

    public void AddScore()
    {
        Score += GlobalGameManager.Instance.scorePerTreasure;;
        GameLogicUI.Instance.ShowScore(Score, GlobalGameManager.Instance.scorePerTreasure*_numberOfTreasures);
    }
    
    public static void PlayAudioClipRandom(AudioClip[] audioClips)
    {
        if (audioClips.Length == 0) return;
        var randomIndex = UnityEngine.Random.Range(0, audioClips.Length);
        Instance.audioSource.PlayOneShot(audioClips[randomIndex]);
    }

    /// <summary>
    /// Helper Function to get game object at a certain coordinate in the world.
    /// </summary>
    /// <param name="coordinates">Game object coordinates</param>
    /// <returns></returns>
    public static GameObject GetGameObjectAtCoordinates(Vector3 coordinates)
    {
        foreach (Transform child in Instance.transform)
        {
            if (child.position == coordinates)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    public static bool IsSpaceAvailable(Vector3 coordinates)
    {
        return Physics2D.OverlapPoint(coordinates, LayerAllowsMovement) && !Physics2D.OverlapPoint(coordinates, LayerBlocksMovement);
    }
    
    /// <summary>
    /// Helper Function to check if Dictionary A and B are different. Useful when comparing surrounding objects
    /// from a gameObject's perspective. E.g. Boulder, arrow.
    /// </summary>
    /// <param name="a">Dictionary A</param>
    /// <param name="b">Dictionary B</param>
    /// <typeparam name="TKey">Key</typeparam>
    /// <typeparam name="TValue">Value</typeparam>
    /// <returns>boolean</returns>
    public static bool AreDictionariesDifferent<TKey, TValue>(Dictionary<TKey, TValue> a, Dictionary<TKey, TValue> b)
    {
        if (a.Count != b.Count) return true;

        foreach (var kvp in a)
        {
            if (!b.TryGetValue(kvp.Key, out TValue valueB)) return true;

            if (!EqualityComparer<TValue>.Default.Equals(kvp.Value, valueB)) return true;
        }

        return false; // Dictionaries are equal
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
        isCurrentlyTicking = true;
        waitingForAllStartTickToFinish = true;
        
        // All Living Entities first
        var allLivingEntities = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ILivingEntity>();
        foreach (var livingEntity in allLivingEntities)
        {
            if (excludeTypes.Length > 0 && excludeTypes.Contains(livingEntity.GetType()))
            {
                continue;
            }

            if (livingEntity is PlayerScript)
            {
                // If player
                livingEntity.OnLivingEntityStartTick(playerMoveDir);
            }
            else
            {
                // If AI
                livingEntity.OnLivingEntityStartTick();
            }
        }
        
        // All Tickables
        var allTickables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ITickable>().OrderBy(obj =>
            {
                if (obj is PlayerScript) return 0;
                if (obj is BasicAI) return 1;
                return 2;
            });

        var tickables = allTickables.ToArray();
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

            var tickables = allTickables as ITickable[] ?? allTickables.ToArray();
            foreach (var tickable in tickables)
            {
                if (tickable.IsNextTickMoveScheduled || tickable.IsNextTickDestroyScheduled)
                {
                    // print("There is something scheduled");
                    StartTick(PlayerScript.Instance.ScheduledMoveDir);
                    timeToCheckSchedule = false;
                    return;
                }
            }

            foreach (var tickable in tickables)
            {
                if (tickable.DoExtraTickLoop)
                {
                    // print("There is something with Extra tick");
                    StartTick(PlayerScript.Instance.ScheduledMoveDir);
                    timeToCheckSchedule = false;
                    return;
                }
            }
            timeToCheckSchedule = false;
        }

        // Reset all tickables after ticking is done.
        if (isCurrentlyTicking)
        {
            var allTickables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<ITickable>();
            foreach (var tickable in allTickables)
            {
                tickable.OnReset();
            }
            isCurrentlyTicking = false;
        }
    }
}
