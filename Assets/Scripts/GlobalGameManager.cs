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
    public float timeLimit = 480f;
    public float elapsedTime;
    public bool isTimerStarted;
    public int scorePerTreasure = 100;
    
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

        GameLogicUI.Instance.ShowTimer(timeLimit - elapsedTime);;
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