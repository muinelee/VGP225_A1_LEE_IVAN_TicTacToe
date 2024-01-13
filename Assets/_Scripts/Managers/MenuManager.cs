using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("HUD Reference")]
    [SerializeField] private GameObject hud;

    [Header("HUD Elements")]
    [SerializeField] public TMP_Text lapTimer;
    [SerializeField] public TMP_Text bestLapTime;
    [SerializeField] public TMP_Text lastLapTime;
    [SerializeField] public TMP_Text CP1;
    [SerializeField] public TMP_Text CP2;
    [SerializeField] public TMP_Text CP3;
    [SerializeField] public TMP_Text CP4;
    [SerializeField] public TMP_Text currentLapCounter;
    [SerializeField] public GameObject countdownDisplay;
    [SerializeField] public TMP_Text countdownText;

    [Header("Menus References")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject winMenu;

    [Header("Main Menu Buttons")]
    [SerializeField] private Button mm_PlayButton;
    [SerializeField] private Button mm_SettingsButton;
    [SerializeField] private Button mm_QuitButton;

    [Header("Pause Menu Buttons")]
    [SerializeField] private Button pm_ResumeButton;
    [SerializeField] private Button pm_RestartButton;
    [SerializeField] private Button pm_SettingsButton;
    [SerializeField] private Button pm_MainMenuButton;
    [SerializeField] private Button pm_QuitButton;

    [Header("GameOver Menu Buttons")]
    [SerializeField] private Button go_RestartButton;
    [SerializeField] private Button go_SettingsButton;
    [SerializeField] private Button go_MainMenuButton;
    [SerializeField] private Button go_QuitButton;

    [Header("Win Menu Buttons")]
    [SerializeField] public TMP_Text wm_bestLapTime;
    [SerializeField] private Button wm_RestartButton;
    [SerializeField] private Button wm_MainMenuButton;
    [SerializeField] private Button wm_QuitButton;

    [Header("Settings Menu Buttons, Sliders, Texts")]
    [SerializeField] private Button closeButton;

    [Header("Sliders")]
    [SerializeField] private Slider masterVolSlider;
    [SerializeField] private Slider sfxVolSlider;
    [SerializeField] private Slider musicVolSlider;

    [Header("Texts")]
    [SerializeField] private TMP_Text masterVolText;
    [SerializeField] private TMP_Text sfxVolText;
    [SerializeField] private TMP_Text musicVolText;

    private GameObject lastActiveMenu;

    #region Unity Methods
    private void Start()
    {
        SettingsManager.Instance.LoadVolumeSettings();
        InitializeButtons();
        InitializeSliders();
        InitializeBestLapTime();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }
    #endregion

    private void HandleGameStateChanged()
    {
        // Initialize menu based on game state
        switch (GameManager.Instance.CurrentGameState)
        {
            case GameState.MAIN:
                ActivateMainMenu();
                break;
            case GameState.PLAY:
                PlayGame();
                ResetUIForPlay();
                break;
            case GameState.PAUSE:
                ActivatePauseMenu();
                break;
            case GameState.GAMEOVER:
                ActivateGameOverMenu();
                break;
            case GameState.WIN:
                ActivateWinMenu();
                break;
            default:
                break;
        }
    }

    private void InitializeButtons()
    {
        mm_PlayButton.onClick.AddListener(PlayGame);
        mm_SettingsButton.onClick.AddListener(ActivateSettingsMenu);
        mm_QuitButton.onClick.AddListener(QuitGame);

        pm_ResumeButton.onClick.AddListener(ResumeGame);
        pm_RestartButton.onClick.AddListener(RestartGame);
        pm_SettingsButton.onClick.AddListener(ActivateSettingsMenu);
        pm_MainMenuButton.onClick.AddListener(GoToMainMenu);
        pm_QuitButton.onClick.AddListener(QuitGame);

        go_RestartButton.onClick.AddListener(RestartGame);
        go_SettingsButton.onClick.AddListener(ActivateSettingsMenu);
        go_MainMenuButton.onClick.AddListener(GoToMainMenu);
        go_QuitButton.onClick.AddListener(QuitGame);

        wm_RestartButton.onClick.AddListener(RestartGame);
        wm_MainMenuButton.onClick.AddListener(GoToMainMenu);
        wm_QuitButton.onClick.AddListener(QuitGame);

        closeButton.onClick.AddListener(Close);
    }

    private void InitializeSliders()
    {
        // Load saved settings
        masterVolSlider.value = SettingsManager.Instance.MasterVolume;
        sfxVolSlider.value = SettingsManager.Instance.SFXVolume;
        musicVolSlider.value = SettingsManager.Instance.MusicVolume;

        // Initialize slider texts
        UpdateSliderText(masterVolSlider.value, masterVolText);
        UpdateSliderText(sfxVolSlider.value, sfxVolText);
        UpdateSliderText(musicVolSlider.value, musicVolText);

        // Subscribe to slider value changes
        masterVolSlider.onValueChanged.AddListener(value => SetVolume(value, masterVolText, "Master"));
        sfxVolSlider.onValueChanged.AddListener(value => SetVolume(value, sfxVolText, "SFX"));
        musicVolSlider.onValueChanged.AddListener(value => SetVolume(value, musicVolText, "Music"));
    }

    private void UpdateSliderText(float value, TMP_Text text)
    {
        text.text = (value * 100).ToString("0");
    }

    private void InitializeBestLapTime()
    {
        // Initialize best lap time display
        float savedBestLapTime = PlayerPrefs.GetFloat("BestLapTime", float.MaxValue);
        if (savedBestLapTime != float.MaxValue)
        {
            UpdateBestLapTimeDisplay(savedBestLapTime);
        }
        else
        {
            bestLapTime.text = "BEST LAP TIME: N/A";
        }
    }

    public void UpdateLastLapTimeDisplay(float lastTime)
    {
        TimeSpan lastTimeSpan = TimeSpan.FromSeconds(lastTime);
        lastLapTime.text = "LAST LAP TIME: " + GameManager.Instance.FormatTime(lastTimeSpan);
    }

    public void UpdateCheckpointTimesDisplay(ArrayList checkpointTimes)
    {
        for (int i = 0; i < checkpointTimes.Count; i++)
        {
            float checkpointTime = (float)checkpointTimes[i];
            TimeSpan timeSpan = TimeSpan.FromSeconds(checkpointTime);
            string formattedTime = GameManager.Instance.FormatTime(timeSpan);

            switch (i)
            {
                case 0:
                    CP1.text = "CHECKPOINT 1: " + formattedTime;
                    break;
                case 1:
                    CP2.text = "CHECKPOINT 2: " + formattedTime;
                    break;
                case 2:
                    CP3.text = "CHECKPOINT 3: " + formattedTime;
                    break;
                case 3:
                    CP4.text = "CHECKPOINT 4: " + formattedTime;
                    break;
            }
        }
    }

    private void SetVolume(float value, TMP_Text text, string name)
    {
        // Update the text
        UpdateSliderText(value, text);

        // Save the settings
        SettingsManager.Instance.SaveVolumeSettings(masterVolSlider.value, sfxVolSlider.value, musicVolSlider.value);

        AudioManager.Instance.InitializeMixer();
    }

    public void UpdateBestLapTimeDisplay(float bestTime)
    {
        TimeSpan bestTimeSpan = TimeSpan.FromSeconds(bestTime);
        bestLapTime.text = "BEST LAP TIME: " + GameManager.Instance.FormatTime(bestTimeSpan);
    }

    private void HideAllMenus()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(false);
        hud.SetActive(false);
    }

    private void ActivateMainMenu()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(false);
    }

    private void ActivateSettingsMenu()
    {
        if (mainMenu.activeSelf)
        {
            lastActiveMenu = mainMenu;
            ToggleMenu(mainMenu, settingsMenu);
        }
        else if (pauseMenu.activeSelf)
        {
            lastActiveMenu = pauseMenu;
            ToggleMenu(pauseMenu, settingsMenu);
        }
        else if (gameOverMenu.activeSelf)
        {
            lastActiveMenu = gameOverMenu;
            ToggleMenu(gameOverMenu, settingsMenu);
        }
    }

    private void Close()
    {
        if (settingsMenu.activeSelf && lastActiveMenu != null)
        {
            ToggleMenu(settingsMenu, lastActiveMenu);        
        }
    }

    private void ActivatePauseMenu()
    {
        if (GameManager.Instance.CurrentGameState == GameState.PAUSE)
        {
            pauseMenu.SetActive(true);
            hud.SetActive(false);
        }
    }

    private void ActivateGameOverMenu()
    {
        if (GameManager.Instance.CurrentGameState == GameState.GAMEOVER)
        {
            gameOverMenu.SetActive(true);
            hud.SetActive(false);
        }
    }

    private void ActivateWinMenu()
    {
        if (GameManager.Instance.CurrentGameState == GameState.WIN)
        {
            winMenu.SetActive(true);
            hud.SetActive(false);

            float bestLapTime = PlayerPrefs.GetFloat("BestLapTime", float.MaxValue);
            if (bestLapTime != float.MaxValue)
            {
                TimeSpan bestTimeSpan = TimeSpan.FromSeconds(bestLapTime);
                wm_bestLapTime.text = "BEST LAP TIME: " + GameManager.Instance.FormatTime(bestTimeSpan);
            }
            else
            {
                wm_bestLapTime.text = "BEST LAP TIME: N/A";
            }
        }
    }

    private void PlayGame()
    {
        if (GameManager.Instance.CurrentGameState == GameState.PLAY)
        {
            HideAllMenus();
            hud.SetActive(true);
        }
        else if (GameManager.Instance.CurrentGameState == GameState.MAIN || GameManager.Instance.CurrentGameState == GameState.GAMEOVER || GameManager.Instance.CurrentGameState == GameState.PAUSE)
        {
            GameManager.Instance.LoadPlayScene();
        }
    }

    private void ResetUIForPlay()
    {
        countdownDisplay.SetActive(GameManager.Instance.isCountingDown);
        if (GameManager.Instance.isCountingDown)
        {
            countdownText.text = GameManager.Instance.countdownTime.ToString("0");
        }
        lapTimer.text = "LAP TIME: 00:00:000";
    }

    private void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }

    private void ResumeGame()
    {
        GameManager.Instance.TogglePause();
        hud.SetActive(true);
    }

    private void GoToMainMenu()
    {
        GameManager.Instance.LoadTitleScene();
    }

    private void QuitGame()
    {
        // Quit game logic
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ToggleMenu(GameObject off, GameObject on)
    {
        off.SetActive(false);
        on.SetActive(true);
    }
}