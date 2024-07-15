using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using static UnityEditor.Progress;

public class GamePauseUI : MonoBehaviour
{
    public event EventHandler OnClickSound;
    public static GamePauseUI Instance { get; private set; }
    [SerializeField] Button resumeButton;
    [SerializeField] Button challengesButton;

    private void Awake()
    {
        Instance = this;
        resumeButton.onClick.AddListener(() =>
        {
            OnClickSound?.Invoke(this, EventArgs.Empty);
            AudioManager.Instance.PlayClickSound();
            GameManager.Instance.TogglePauseGame();
        });
        challengesButton.onClick.AddListener(() =>
        {
            OnClickSound?.Invoke(this, EventArgs.Empty);
            AudioManager.Instance.PlayClickSound();
            Loader.Load(Loader.Scene.ChallengesScene);
        });
    }
    private void Start()
    {
        GameManager.Instance.OnGamePaused += GameManagerOnGamePaused;
        GameManager.Instance.OnGameUnpaused += GameManagerOnGameUnpaused;

        Hide();
    }

    private void GameManagerOnGamePaused(object sender, System.EventArgs e)
    {
        Show();
    }
    private void GameManagerOnGameUnpaused(object sender, System.EventArgs e)
    {
        Hide();
    }
    void Show()
    {
        gameObject.SetActive(true);

        resumeButton.Select();
    }
    void Hide()
    {
        gameObject.SetActive(false);
    }
}
