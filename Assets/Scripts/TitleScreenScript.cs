using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class TitleScreenScript : MonoBehaviour
{
    private GameObject _teamInputPanel;
    private GameObject _aboutPanel;
    private GameObject _gameEndPanel;
    private TMP_InputField _teamNameInput;
    [SerializeField]
    private TMP_Text teamNameEndText;
    [SerializeField]
    private TMP_Text finalScoreText;
    
    private void Awake()
    {
        _teamInputPanel = GameObject.Find("TeamInputPanel");
        _teamNameInput = GameObject.Find("TeamNameInput").GetComponent<TMP_InputField>();
        _teamInputPanel.SetActive(false);
        _aboutPanel = GameObject.Find("AboutPanel");
        _aboutPanel.SetActive(false);
        
        _gameEndPanel = GameObject.Find("GameEndPanel");
        _gameEndPanel.SetActive(false);
    }

    private void Start()
    {
        if (GlobalGameManager.Instance.isGameEnd)
        {
            int minutes = Mathf.FloorToInt(GlobalGameManager.Instance.elapsedTime / 60);
            int seconds = Mathf.FloorToInt(GlobalGameManager.Instance.elapsedTime % 60);
            teamNameEndText.text = GlobalGameManager.Instance.teamName;
            finalScoreText.text = $"Final score = {GlobalGameManager.Instance.TotalScore.ToString()}\nElapsed time = {minutes:00}:{seconds:00}";
            _gameEndPanel.SetActive(true);
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