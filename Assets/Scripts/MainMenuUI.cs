using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public event EventHandler OnClickSound;
    public static MainMenuUI Instance { get; private set; }
    [SerializeField] Button goToLevelsButton;
    [SerializeField] Button soundButton;
    [SerializeField] Button musicButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button tcButton;
    [SerializeField] Button ppButton;

    [SerializeField] Sprite soundOnSprite;
    [SerializeField] Sprite soundOffSprite;
    [SerializeField] Sprite musicOnSprite;
    [SerializeField] Sprite musicOffSprite;

    

    private void Awake()
    {
        Instance = this;
        goToLevelsButton.onClick.AddListener(() => {
            AudioManager.Instance.PlayClickSound();
            OnClickSound?.Invoke(this, EventArgs.Empty);
            Loader.Load(Loader.Scene.ChallengesScene);
        });
        soundButton.onClick.AddListener(() => {
            AudioManager.Instance.PlayClickSound();
            OnClickSound?.Invoke(this, EventArgs.Empty);
            AudioManager.Instance.ToggleSound();
            UpdateSoundButton();
        });
        musicButton.onClick.AddListener(() => {
            AudioManager.Instance.PlayClickSound();
            OnClickSound?.Invoke(this, EventArgs.Empty);
            AudioManager.Instance.ToggleMusic();
            UpdateMusicButton();
        });
        tcButton.onClick.AddListener(() => {
            AudioManager.Instance.PlayClickSound();
            OnClickSound?.Invoke(this, EventArgs.Empty);
            Application.OpenURL("https://atlasconciergeltd.store/privacypolicy/");
        });
        ppButton.onClick.AddListener(() => {
            AudioManager.Instance.PlayClickSound();
            OnClickSound?.Invoke(this, EventArgs.Empty);
            Application.OpenURL("https://atlasconciergeltd.store/privacypolicy/");
        });
        quitButton.onClick.AddListener(() => {
            AudioManager.Instance.PlayClickSound();
            OnClickSound?.Invoke(this, EventArgs.Empty);
            Application.Quit();
        });

        Time.timeScale = 1f;
    }

    private void Start()
    {
        goToLevelsButton.Select();
        quitButton.gameObject.SetActive(false);
         UpdateSoundButton();
         UpdateMusicButton();
    }
    
    private void UpdateSoundButton()
    {
        if (AudioManager.Instance.IsSoundMuted())
            soundButton.image.sprite = soundOffSprite;
        else
            soundButton.image.sprite = soundOnSprite;
    }

    private void UpdateMusicButton()
    {
        if (AudioManager.Instance.IsMusicMuted())
            musicButton.image.sprite = musicOffSprite;
        else
            musicButton.image.sprite = musicOnSprite;
    }
    
}
