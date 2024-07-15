using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameOverUI : MonoBehaviour
{
    public event EventHandler OnClickSound;
    public static GameOverUI Instance { get; private set; }
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private Button challengesButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI bestMolesPerSecondText;
    [SerializeField] private TextMeshProUGUI molesPerSecondText;
    [SerializeField] private Image newBestMolesPerSecondText;

    [SerializeField] private bool isNewBestMolesPerSecondText = false;

    private void Awake()
    {
        Instance = this;
        tryAgainButton.onClick.AddListener(() => {
            OnClickSound?.Invoke(this, EventArgs.Empty);
            AudioManager.Instance.PlayClickSound();
            Loader.Load(Loader.Scene.BeginnerChallengeScene);
        });
        challengesButton.onClick.AddListener(() => {
            OnClickSound?.Invoke(this, EventArgs.Empty);
            AudioManager.Instance.PlayClickSound();
            Loader.Load(Loader.Scene.ChallengesScene);
        });
        mainMenuButton.onClick.AddListener(() => {
            OnClickSound?.Invoke(this, EventArgs.Empty);
            AudioManager.Instance.PlayClickSound();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }
    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }
    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            Show();

            // Display current score
            molesPerSecondText.text = GameManager.Instance.GetMolesPerSecond().ToString("F3");

            // Display high score
            //bestMolesPerSecondText.text = GameManager.Instance.GetGamePlayingHighTimer().ToString("F3");
            bestMolesPerSecondText.text = "BEST " + GameManager.Instance.GetGamePlayingHighTimer().ToString("F3") + " MOLES/S";

            // Check if the current score is higher than the high score
            if (GameManager.Instance.GetIsNewBest())
            {
                // Show the new best score panel
                ShowNewBestMolesPerSecondText();
            }
            else
            {
                // Hide the new best score panel if the current score is not higher
                HideNewBestMolesPerSecondText();
            }
        }
        else
        {
            Hide();
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void ShowNewBestMolesPerSecondText()
    {
        newBestMolesPerSecondText.gameObject.SetActive(true);
    }
    private void HideNewBestMolesPerSecondText()
    {
        newBestMolesPerSecondText.gameObject.SetActive(false);
    }
}
