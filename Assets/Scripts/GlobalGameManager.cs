using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GlobalGameManager : MonoBehaviour
{
    public static GlobalGameManager Instance { get; private set; }
    
    public AudioSource audioSource;
    public AudioClip exitSound;
    
    [HideInInspector]
    public string teamName = "";
    [HideInInspector]
    public bool isGameEnd;
    private readonly float _timeLimit = 600;
    public float elapsedTime;
    public bool isTimerStarted;
    public int scorePerTreasure = 100;
    
    private bool _hasTriggeredZeroEvent = false;
    
    public int TotalScore { get; private set; }

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            audioSource = GetComponent<AudioSource>();
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // destroy duplicates
        }
    }
    
    private void Update()
    {
        if (!isTimerStarted) return;

        elapsedTime += Time.deltaTime;

        GameLogicUI.Instance.ShowTimer(_timeLimit - elapsedTime);
        
        float remainingTime = Mathf.Max(0, _timeLimit - elapsedTime);

        // --- Color transitions ---
        if (remainingTime <= 60f)
        {
            GameLogicUI.Instance.SetTimerColor(Color.red);
        }
        else if (remainingTime <= 120f)
        {
            GameLogicUI.Instance.SetTimerColor(Color.yellow);
        }
        else
        {
            GameLogicUI.Instance.SetTimerColor(Color.white);
        }

        // --- Update the timer display ---
        GameLogicUI.Instance.ShowTimer(remainingTime);

        // --- Trigger event when timer hits zero ---
        if (remainingTime <= 0f && !_hasTriggeredZeroEvent)
        {
            _hasTriggeredZeroEvent = true;
            OnTimerEnd();
        }
    }
    
    private void OnTimerEnd()
    {
        Debug.Log("⏰ Time's up!");
        isGameEnd = true;

        // Example: play sound, change level, or show results
        // PlayExitSound();
        // ChangeLevel("GameOver");
        ChangeLevel("0", GameLogic.Instance.Score);
    }

    public void ChangeLevel(string level, int score = 0, bool addScore = true)
    {
        if (addScore)
        {
            AddGlobalScore(score);
        }
        PlayExitSound();
        if (level != "")
        {
            if (int.TryParse(level, out var levelInt))
            {
                SceneManager.LoadScene(levelInt);
            }
            else
            {
                SceneManager.LoadScene(level);
            }
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    
    public void AddGlobalScore(int score)
    {
        TotalScore += score;
    }

    public void PlayExitSound()
    {
        audioSource.PlayOneShot(exitSound);
    }
}