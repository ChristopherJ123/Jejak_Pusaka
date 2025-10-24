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
        PlayerScript.Instance.IsAlive = false;
        PlayerScript.Instance.Deactivate();
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

    /// <summary>
    /// Checks if a space at the provided coordinates is available for movement.
    /// </summary>
    /// <param name="coordinates">The world coordinates to check for space availability.</param>
    /// <returns>Returns true if the space is available for movement; otherwise, false.</returns>
    public static bool IsSpaceAvailable(Vector3 coordinates)
    {
        return Physics2D.OverlapPoint(coordinates, LayerAllowsMovement) && !Physics2D.OverlapPoint(coordinates, LayerBlocksMovement);
    }
    
    /// <summary>
    /// Check if space is available at a certain coordinate in the world.
    /// </summary>
    /// <param name="coordinates">World coordinates</param>
    /// <param name="excludeTypes">Types in excludeTypes will get ignored and returns true</param>
    /// <returns>Returns true if the space is available for movement; otherwise, false.</returns>
    public static bool IsSpaceAvailable(Vector3 coordinates, params Type[] excludeTypes)
    {
        var collider = Physics2D.OverlapPoint(coordinates, LayerBlocksMovement);
        if (collider && excludeTypes.Length > 0)
        {
            foreach (var type in excludeTypes)
            {
                if (collider.GetComponent(type))
                {
                    return Physics2D.OverlapPoint(coordinates, LayerAllowsMovement);
                }
            }
        }
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
    public void StartTick(params Type[] excludeTypes)
    {
        // print("start tick");
        
        // Quick check if player can even move to desired direction
        if (PlayerScript.Instance.IsNextTickMoveScheduled)
        {
            var check = PlayerScript.Instance.ScheduledMoveDir;
            if (!PlayerScript.Instance.CanMoveOrRedirect(ref check))
            {
                return;
            }
        }
        
        isCurrentlyTicking = true;
        waitingForAllStartTickToFinish = true;
        
        // All Living Entities first
        // (No ordering neede because for now there is only mummy and player (player doesn't do anything on OnLivingEntityStartTick))
        var allLivingEntities = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ILivingEntity>();
        foreach (var livingEntity in allLivingEntities)
        {
            if (excludeTypes.Length > 0 && excludeTypes.Contains(livingEntity.GetType()))
            {
                continue;
            }
            livingEntity.OnLivingEntityStartTick();
        }
        
        // All Tickables
        // Alasan di order seperti ini: Pertama, kita utamain bisa dorong moveable dulu seperti crate
        // sebelum mummy jalan duluan. Jadi ordernya Player -> Crate -> Mummy
        var allTickables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ITickable>().OrderBy(obj =>
            {
                if (obj is PlayerScript) return 0;
                if (obj is BasicAI) return 2;
                return 1;
            });

        var tickables = allTickables.ToArray();
        foreach (var tickable in tickables)
        {
            if (excludeTypes.Length > 0 && excludeTypes.Contains(tickable.GetType()))
            {
                continue;
            }
            tickable.OnStartTick();
            
            // Apa yang terjadi kalau enable ini?
            // print("Physics2D Sync transforms");
            Physics2D.SyncTransforms();
        }
        
        // After start tick, do all the post start tick methods
        // Remember: This method should only contain configuration variables only. This method should only be used
        // for functions that cannot be achievable with random ordering from the former Start Tick method.
        foreach (var tickable in tickables)
        {
            tickable.OnPostStartTick();
        }
    }

    public void EndTick(params Type[] excludeTypes)
    {
        // print("end tick");
        waitingForAllEndTickToFinish = true;
        var allTickables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ITickable>();
        var tickables = allTickables.ToArray();
        foreach (var tickable in tickables)
        {
            if (excludeTypes.Length > 0 && excludeTypes.Contains(tickable.GetType()))
            {
                continue;
            }
            tickable.OnEndTick();
        }
        
        // After doing all End tick, now should do all Post end tick
        // Remember: This method should only contain configuration variables only. This method should only be used
        // for functions that cannot be achievable with random ordering from the former End Tick method.
        foreach (var tickable in tickables)
        {
            tickable.OnPostEndTick();
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
                    StartTick();
                    timeToCheckSchedule = false;
                    return;
                }
            }

            foreach (var tickable in tickables)
            {
                if (tickable.DoExtraTickLoop)
                {
                    // print("There is something with Extra tick");
                    StartTick();
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
