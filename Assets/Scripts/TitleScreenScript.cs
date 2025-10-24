using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class TitleScreenScript : MonoBehaviour
{
    private GameObject _teamInputPanel;
    private GameObject _aboutPanel;
    private GameObject _gameEndPanel;
    private GameObject _cheatsheetPanel;
    private GameObject _playButton;
    private GameObject _tutorialButton;
    private TMP_InputField _teamNameInput;
    private GameObject _leaderboardPanel;
    private Transform _leaderboardContent;
    [SerializeField]
    private TMP_Text teamNameEndText;
    [SerializeField]
    private TMP_Text finalScoreText;
    public GameObject leaderboardEntryPrefab;
    
    private void Awake()
    {
        _teamInputPanel = GameObject.Find("TeamInputPanel");
        _teamNameInput = GameObject.Find("TeamNameInput").GetComponent<TMP_InputField>();
        _teamInputPanel.SetActive(false);
        _aboutPanel = GameObject.Find("AboutPanel");
        _aboutPanel.SetActive(false);
        _cheatsheetPanel = GameObject.Find("CheatSheetPanel");
        _cheatsheetPanel.SetActive(false);
        _leaderboardContent = GameObject.Find("LeaderboardContent").transform;
        _leaderboardPanel = GameObject.Find("LeaderboardPanel");
        _leaderboardPanel.SetActive(false);
        _playButton = GameObject.Find("Play Button");
        _tutorialButton = GameObject.Find("Tutorial Button");
        _gameEndPanel = GameObject.Find("GameEndPanel");
        _gameEndPanel.SetActive(false);
        Debug.Log(Application.persistentDataPath);
    }

    private void Start()
    {
        if (GlobalGameManager.Instance.isGameEnd)
        {
            int minutes = Mathf.FloorToInt(GlobalGameManager.Instance.elapsedTime / 60);
            int seconds = Mathf.FloorToInt(GlobalGameManager.Instance.elapsedTime % 60);
            WebBridge.SendScore(GlobalGameManager.Instance.TotalScore);
            teamNameEndText.text = GlobalGameManager.Instance.teamName;
            finalScoreText.text = $"Final score = {GlobalGameManager.Instance.TotalScore.ToString()}\nElapsed time = {minutes:00}:{seconds:00}";
            _gameEndPanel.SetActive(true);
            // _playButton.SetActive(false);
            // _tutorialButton.SetActive(false);
            
            GlobalGameManager.Instance.persistentScoreManager.RecordScore(GlobalGameManager.Instance.teamName, GlobalGameManager.Instance.TotalScore);
            
            // Reset Global Game Manager
            GlobalGameManager.Instance.isGameEnd = false;
            GlobalGameManager.Instance.elapsedTime = 0;
            GlobalGameManager.Instance.TotalScore = 0;
            GlobalGameManager.Instance.isTimerStarted = false;
        }
    }

    public void ShowTeamInput()
    {
        _teamInputPanel.SetActive(true);
    }
    
    public void HideTeamInput()
    {
        _teamInputPanel.SetActive(false);
    }
    
    public void ShowCheatsheet()
    {
        _cheatsheetPanel.SetActive(true);
    }
    
    public void HideCheatsheet()
    {
        _cheatsheetPanel.SetActive(false);
    }

    public void ShowAbout()
    {
        _aboutPanel.SetActive(true);
    }
    
    public void HideAbout()
    {
        _aboutPanel.SetActive(false);
    }
    
    public void ShowGameEnd()
    {
        _gameEndPanel.SetActive(true);
    }
    
    public void HideGameEnd()
    {
        _gameEndPanel.SetActive(false);
    }
    
    public void ShowLeaderboard()
    {
        _leaderboardPanel.gameObject.SetActive(true);
        
        // Clean up old entries
        foreach (Transform child in _leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // Get leaderboard data
        var scores = new List<TeamScore>(GlobalGameManager.Instance.persistentScoreManager.GetLeaderboard());
        scores.Sort((a, b) => b.highscore.CompareTo(a.highscore)); // Sort by score descending

        // Instantiate UI rows
        foreach (var team in scores)
        {
            var entry = Instantiate(leaderboardEntryPrefab, _leaderboardContent);
            var text = entry.GetComponent<TextMeshProUGUI>(); // or Text if not using TMP
            text.text = $"{team.teamName} - {team.highscore}";
        }
    }

    public void HideLeaderboard()
    {
        _leaderboardPanel.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        GlobalGameManager.Instance.teamName = _teamNameInput.text;
        LoadSceneByName("2");
    }
    
    public void LoadSceneByName(string sceneName)
    {
        GlobalGameManager.Instance.ChangeLevel(sceneName);
    }
}