using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GameLogicUI : MonoBehaviour
{
    public static GameLogicUI Instance;
    
    private TextMeshProUGUI _hintText;
    private Coroutine _hintRoutine;
    private TextMeshProUGUI _teamNameText;
    private TextMeshProUGUI _levelNameText;
    private TextMeshProUGUI _scoreText;
    private TextMeshProUGUI _totalScoreText;
    private TMP_Text _timerText;
    public bool isTimerStarted = true;
    
    private Canvas _deathScreenCanvas;
    private TextMeshProUGUI _deathScreenMessage;

    private void Awake()
    {
        Instance = this;
        _hintText = GameObject.Find("HintUI").GetComponent<TextMeshProUGUI>();
        _hintText.text = "";
        _teamNameText = GameObject.Find("TeamNameUI").GetComponent<TextMeshProUGUI>();
        _levelNameText = GameObject.Find("LevelNameUI").GetComponent<TextMeshProUGUI>();
        _scoreText = GameObject.Find("ScoreUI").GetComponent<TextMeshProUGUI>();
        _totalScoreText = GameObject.Find("TotalScoreUI").GetComponent<TextMeshProUGUI>();
        _timerText = GameObject.Find("TimerUI").GetComponent<TMP_Text>();
        
        _deathScreenCanvas = GameObject.Find("GameOverCanvas").GetComponent<Canvas>();
        _deathScreenMessage = GameObject.Find("DeathMessage").GetComponent<TextMeshProUGUI>();
        _deathScreenCanvas.gameObject.SetActive(false);
    }

    private void Start()
    {
        _teamNameText.text = GlobalGameManager.Instance.teamName ?? "Team Name";

        GlobalGameManager.Instance.isTimerStarted = isTimerStarted;
        _timerText.gameObject.SetActive(isTimerStarted);
        
        if (GlobalGameManager.Instance.TotalScore != 0)
        {
            _totalScoreText.text = $"Total Score: {GlobalGameManager.Instance.TotalScore}";
        }
        else
        {
            _totalScoreText.gameObject.SetActive(false);
        }
    }
    
    public void SetTimerColor(Color color)
    {
        _timerText.color = color;
    }

    public void ShowDeathScreen(string message)
    {
        _deathScreenCanvas.gameObject.SetActive(true);
        _deathScreenMessage.text = message;
    }

    public void ShowHint(string message)
    {
        // Stop any running hint transition so they don't overlap
        if (_hintRoutine != null)
            StopCoroutine(_hintRoutine);

        _hintRoutine = StartCoroutine(HintTransition(message));
    }

    private IEnumerator HintTransition(string newMessage)
    {
        // Fade out
        for (float t = 0; t < 0.25f; t += Time.deltaTime)
        {
            _hintText.alpha = 1 - (t / 0.25f);
            yield return null;
        }
        _hintText.alpha = 0;

        // Wait in black screen
        yield return new WaitForSeconds(0.5f);

        // Change text
        _hintText.text = newMessage;

        // Fade in
        for (float t = 0; t < 0.25f; t += Time.deltaTime)
        {
            _hintText.alpha = t / 0.25f;
            yield return null;
        }
        _hintText.alpha = 1;
    }
    
    public void ShowTeamName(string message)
    {
        _teamNameText.text = message;
    }

    public void ShowLevelName(string message)
    {
        _levelNameText.text = message;
    }
    public void ShowScore(int currentScore, int totalScore)
    {
        _scoreText.text = $"Score: {currentScore}/{totalScore}";
    }

    public void ShowTotalScore(int score)
    {
        _totalScoreText.text = $"Total Score: {score}";   
    }
    
    public void ShowTimer(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        _timerText.text = $"{minutes:00}:{seconds:00}";
    }
}