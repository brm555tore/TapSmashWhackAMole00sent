using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;
    public LevelDifficultySO levelDifficultySO;

    private enum State { WaitingToStart, GamePlaying, GameOver }

    private State state;
    private HashSet<Mole> currentMoles = new HashSet<Mole>();

    private const string HIGH_SCORE_KEY = "HighScore";


    private bool isGamePaused = false;
    private float waitingToStartTimer = 1f;
    private float gamePlayingHighTimer;
    private float gamePlayingTimer = 0f;
    private bool isNewBest = false;
    private float gamePlayingTimerMax = 200f;
    private float nextMoleSpawnTime;
    private float timeBetweenMoleSpawns;
    private int levelDifficulty;
    private float timeElapsed = 0f;
    private float increaseRate = 0.5f;
    private float timeToUpdateDifficulty = 60f;

    [SerializeField] private List<Mole> moles;
    [SerializeField] private TMPro.TextMeshProUGUI molesPerSecondText;
    [SerializeField] private TextMeshProUGUI bestMolesPerSecondText;
    [SerializeField] private Button resumeButton;
    [SerializeField] private ChallengeDataHolder challengeDataHolder;
    

    private void Awake() {
        Instance = this;
        state = State.WaitingToStart;
        resumeButton.onClick.AddListener(() => {
            TogglePauseGame();
        });
    }

    private void Start() {
        challengeDataHolder = FindObjectOfType<ChallengeDataHolder>(true);

        levelDifficultySO = challengeDataHolder.GetLevelDifficultySO();
        FindAllMoleInScene();
        nextMoleSpawnTime = Time.time + timeBetweenMoleSpawns;
        HideAndClearMoles();
        LoadHighScore();
        SetLevelDifficulties();
    }


    void Update() {
        switch (state) {
            case State.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer < 0f) {
                    state = State.GamePlaying;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer += Time.deltaTime;
                timeElapsed += Time.deltaTime;

                molesPerSecondText.text = gamePlayingTimer.ToString("F3");
                bestMolesPerSecondText.text = GetGamePlayingHighTimer().ToString("F3");

                // Check if it's time to update difficulty
                if (timeElapsed >= timeToUpdateDifficulty) {
                    UpdateDifficulty();
                    timeElapsed = 0f;
                }

                if (Time.time >= nextMoleSpawnTime) {
                    int index = UnityEngine.Random.Range(0, moles.Count);
                    if (!currentMoles.Contains(moles[index])) {
                        currentMoles.Add(moles[index]);
                        moles[index].Activate(levelDifficulty);
                        nextMoleSpawnTime = Time.time + timeBetweenMoleSpawns;
                    }
                }
                break;
            case State.GameOver:
                break;
        }
    }
    private void UpdateDifficulty() {
        // Increase level difficulty based on elapsed time
        levelDifficulty += Mathf.FloorToInt(timeElapsed * increaseRate);

        // Get the new time between mole spawns based on the updated level difficulty
        timeBetweenMoleSpawns = GetTimeBetweenMoleSpawnsForLevelDifficulty(levelDifficulty);
    }
        private float GetTimeBetweenMoleSpawnsForLevelDifficulty(int difficulty) {
        if (difficulty >= 40) {
            return 2f;
        } else if (difficulty >= 20) {
            return 2.5f;
        } else {
            return 3f;
        }
    }
    private void FindAllMoleInScene() {
        Mole[] foundMoles = FindObjectsOfType<Mole>();
        moles.AddRange(foundMoles);
    }

    private void HideAndClearMoles() {
        for (int i = 0; i < moles.Count; i++) {
            moles[i].Hide();
            moles[i].SetIndex(i);
        }
        currentMoles.Clear();
    }

    private void LoadHighScore() {
        if (PlayerPrefs.HasKey(HIGH_SCORE_KEY)) {
            gamePlayingHighTimer = PlayerPrefs.GetFloat(HIGH_SCORE_KEY);
        }
        else {
            gamePlayingHighTimer = 0f;
        }
    }

    private void SaveHighScore() {
        PlayerPrefs.SetFloat(HIGH_SCORE_KEY, gamePlayingHighTimer);
        PlayerPrefs.Save();
    }

    private void SetLevelDifficulties() {
        timeBetweenMoleSpawns = levelDifficultySO.GetTimeBetweenMoleSpawns();
        levelDifficulty = levelDifficultySO.GetLevelDifficulty();
    }
    private void UpdateLevelBasedOnScore() {
        float increaseRate = 0.5f; // Adjust this value as needed
        levelDifficulty += Mathf.FloorToInt(Time.deltaTime * increaseRate);
    }
    public bool IsGamePlaying() {
        return state == State.GamePlaying;
    }

    public bool IsGameOver() {
        return state == State.GameOver;
    }

    public float GetGamePlayinTimerNormalized() {
        return 1 - (gamePlayingTimer / gamePlayingTimerMax);
    }

    public void TogglePauseGame() {
        isGamePaused = !isGamePaused;
        if (isGamePaused) {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    public void GameOver() {
        if (gamePlayingTimer > gamePlayingHighTimer) {
            gamePlayingHighTimer = gamePlayingTimer;
            SaveHighScore();
            isNewBest = true;
        }
        foreach (Mole mole in moles) {
            mole.StopGame();
        }
        state = State.GameOver;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public float GetMolesPerSecond() {
        return gamePlayingTimer;
    }

    public float GetGamePlayingHighTimer() {
        if (PlayerPrefs.HasKey(HIGH_SCORE_KEY)) {
            gamePlayingHighTimer = PlayerPrefs.GetFloat(HIGH_SCORE_KEY);
        }
        else {
            gamePlayingHighTimer = 0f;
        }
        return gamePlayingHighTimer;
    }

    public bool GetIsNewBest() {
        return isNewBest;
    }

    public void MoleMissed(int moleIndex, bool isMole) {
        currentMoles.Remove(moles[moleIndex]);

        if (isMole) {
            GameOver();
        }
    }

    public void MoleHited(int moleIndex) {
        currentMoles.Remove(moles[moleIndex]);
        UpdateLevelBasedOnScore();
    }
}
