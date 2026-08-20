using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState
{
    Pause,    // Trạng thái chuẩn bị giữa các Wave (TFT)
    Resume,   // Trạng thái chiến đấu (Quái đang ra)
    Freeze,   // Trạng thái dừng hệ thống (Mở Pause Menu hoặc Reward)
    GameOver, // Thua
    Victory   // Thắng
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Trạng Thái Hiện Tại")]
    public GameState currentState = GameState.Pause;
    private GameState stateBeforeFreeze;

    [Header("Cấu Hình Delay (Giây)")]
    public float waveEndDelay = 1f; // Chờ 0.5s sau khi xong Wave
    public float endGameDelay = 1f; // Chờ 1s cho Thắng/Thua

    [Header("UI Panels (Kéo thả từ Canvas vào đây)")]
    public GameObject footerPanel;      // Thanh Footer dưới
    public GameObject pauseMenuPanel;   // Bảng Menu Tạm dừng (Pause Popup)
    public GameObject settingsPanel;    // Bảng Cài đặt (Settings Popup)
    public GameObject gameOverPanel;    // Bảng Thua
    public GameObject victoryPanel;     // Bảng Thắng

    private Coroutine delayRoutine;
    private bool isEndingGame = false;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        isEndingGame = false;
        Time.timeScale = 1f;
        SetState(GameState.Pause);
    }

    public void SetState(GameState newState)
    {
        if (newState == GameState.GameOver && !isEndingGame)
        {
            TriggerGameOver();
            return;
        }

        if (newState == GameState.Victory && !isEndingGame)
        {
            TriggerVictory();
            return;
        }

        currentState = newState;

        switch (currentState)
        {
            case GameState.Pause:
                Time.timeScale = 1f;
                if (footerPanel != null) footerPanel.SetActive(true);
                if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
                if (settingsPanel != null) settingsPanel.SetActive(false);
                if (gameOverPanel != null) gameOverPanel.SetActive(false);
                if (victoryPanel != null) victoryPanel.SetActive(false);                
                break;

            case GameState.Resume:
                Time.timeScale = 1f;
                if (footerPanel != null) footerPanel.SetActive(true); 
                if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
                if (settingsPanel != null) settingsPanel.SetActive(false);
                
                if (ShopManager.Instance != null)
                {
                    ShopManager.Instance.CloseShop();
                }
                break;

            case GameState.Freeze:
                Time.timeScale = 0f; // Đóng băng game
                break;

            case GameState.GameOver:
                if (footerPanel != null) footerPanel.SetActive(false);
                if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
                if (gameOverPanel != null) gameOverPanel.SetActive(true);
                Time.timeScale = 0f;
                break;

            case GameState.Victory:
                if (footerPanel != null) footerPanel.SetActive(false);
                if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
                if (victoryPanel != null) victoryPanel.SetActive(true);
                SaveLevelProgress();
                Time.timeScale = 0f;
                break;
        }

        if (GameplayCanvasController.Instance != null)
        {
            GameplayCanvasController.Instance.UpdateFooterButtons(currentState);
        }
    }

    // --- PAUSE MENU ---

    public void FreezeGame()
    {
        if (currentState != GameState.Freeze)
        {
            stateBeforeFreeze = currentState;
            SetState(GameState.Freeze);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        SetState(stateBeforeFreeze);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // --- XỬ LÝ GAME OVER VÀ VICTORY ---

    public void TriggerGameOver()
    {
        if (isEndingGame) return;
        isEndingGame = true;

        if (delayRoutine != null) StopCoroutine(delayRoutine);
        delayRoutine = StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        yield return new WaitForSecondsRealtime(endGameDelay);
        SetState(GameState.GameOver);
    }

    public void TriggerVictory()
    {
        if (isEndingGame) return;
        isEndingGame = true;

        if (delayRoutine != null) StopCoroutine(delayRoutine);
        delayRoutine = StartCoroutine(VictoryRoutine());
    }

    private IEnumerator VictoryRoutine()
    {
        yield return new WaitForSecondsRealtime(endGameDelay);
        SetState(GameState.Victory);
    }

    // --- HỆ THỐNG WAVE ---

    private void HealAllTowers()
    {
        TowerHealth[] allTowers = FindObjectsByType<TowerHealth>(FindObjectsSortMode.None);
        foreach (TowerHealth tower in allTowers)
        {
            tower.HealToFull();
        }
    }

    public void OnClickStartWaveButton()
    {
        if (currentState == GameState.Pause)
        {
            SetState(GameState.Resume);

            if (WaveSpawner.Instance != null)
            {
                WaveSpawner.Instance.StartCurrentWave();
            }
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    public void SaveLevelProgress()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int highestUnlocked = PlayerPrefs.GetInt("HighestUnlockedLevel", 1);

        if (currentSceneIndex >= highestUnlocked)
        {
            PlayerPrefs.SetInt("HighestUnlockedLevel", currentSceneIndex + 1);
            PlayerPrefs.Save();
        }
    }
    
    public void RegisterUIPanels(GameObject footer, GameObject pauseMenu, GameObject settings, GameObject gameOver, GameObject victory, Text levelTitle = null)
    {
        footerPanel = footer;
        pauseMenuPanel = pauseMenu;
        settingsPanel = settings;
        gameOverPanel = gameOver;
        victoryPanel = victory;
        
        SetState(GameState.Pause);
    }

    // Xử lý khi hoàn thành Wave: Delay 0.5s -> Freeze -> Bật Reward
    public void OnWaveCompleted()
    {
        if (delayRoutine != null) StopCoroutine(delayRoutine);
        delayRoutine = StartCoroutine(WaveCompletedRoutine());
    }

    private IEnumerator WaveCompletedRoutine()
    {
        // Chờ 0.5s theo thời gian thực để hoàn tất hiệu ứng tiêu diệt quái cuối
        yield return new WaitForSecondsRealtime(waveEndDelay);

        HealAllTowers();
        SetState(GameState.Freeze);

        if (WaveRewardManager.Instance != null)
        {
            WaveRewardManager.Instance.ShowRewardPanel();
        }
    }

    public void OnRewardClaimed()
    {
        SetState(GameState.Pause);
    }
}