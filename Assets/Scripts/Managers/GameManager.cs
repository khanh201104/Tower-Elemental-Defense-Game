using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState
{
    Pause,    // Trạng thái chuẩn bị giữa các Wave (TFT)
    Resume,   // Trạng thái chiến đấu (Quái đang ra)
    Freeze,   // Trạng thái tạm dừng hệ thống (Mở Pause Menu)
    GameOver, // Thua
    Victory   // Thắng
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Trạng Thái Hiện Tại")]
    public GameState currentState = GameState.Pause;
    private GameState stateBeforeFreeze; // Ghi nhớ trạng thái trước khi bấm Pause để khôi phục

    [Header("UI Panels (Kéo thả từ Canvas vào đây)")]
    public GameObject footerPanel;      // Thanh Footer dưới
    public GameObject pauseMenuPanel;   // Bảng Menu Tạm dừng (Pause Popup)
    public GameObject settingsPanel;     // Bảng Cài đặt (Settings Popup)
    public GameObject gameOverPanel;    // Bảng Thua
    public GameObject victoryPanel;     // Bảng Thắng
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f; // Đảm bảo thời gian luôn chạy bình thường khi load màn
        SetState(GameState.Pause);
    }

    public void SetState(GameState newState)
    {
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

                // 1. Hồi máu cho toàn bộ tháp
                HealAllTowers();

                // 2. Kiểm tra gọi Panel Thưởng Wave
                if (WaveSpawner.Instance != null && WaveSpawner.Instance.currentWaveIndex > 0)
                {
                    if (WaveRewardManager.Instance != null)
                    {
                        WaveRewardManager.Instance.ShowRewardPanel();
                    }
                    else
                    {
                        Debug.LogError("❌ LỖI: WaveRewardManager.Instance bị NULL! (Chưa tạo GameObject WaveRewardManager trong Scene)");
                    }
                }
                break;

            case GameState.Resume:
                Time.timeScale = 1f;
                if (footerPanel != null) footerPanel.SetActive(false);
                if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
                if (settingsPanel != null) settingsPanel.SetActive(false);
                
                if (ShopManager.Instance != null)
                {
                    ShopManager.Instance.CloseShop();
                }
                break;

            case GameState.Freeze:
                Time.timeScale = 0f; // Đóng băng toàn bộ hoạt ảnh, đường đạn và di chuyển
                if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
                if (settingsPanel != null) settingsPanel.SetActive(false);
                break;

            case GameState.GameOver:
                Debug.Log("💀 GAME OVER: Nhà chính đã bị phá hủy!");
                if (footerPanel != null) footerPanel.SetActive(false);
                if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
                if (gameOverPanel != null) gameOverPanel.SetActive(true);
                
                Time.timeScale = 0f;
                break;

            case GameState.Victory:
                Debug.Log("🎉 VICTORY: Đã dọn sạch toàn bộ các Wave!");
                if (footerPanel != null) footerPanel.SetActive(false);
                if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
                if (victoryPanel != null) victoryPanel.SetActive(true);
                SaveLevelProgress();
                Time.timeScale = 0f;
                break;
        }
    }

    // --- CÁC HÀM XỬ LÝ CHO PAUSE MENU ---

    // 1. Gán vào Nút Pause góc màn hình
    public void FreezeGame()
    {
        if (currentState != GameState.Freeze)
        {
            stateBeforeFreeze = currentState; // Lưu lại xem trước đó đang là Pause hay Resume
            SetState(GameState.Freeze);
        }
    }

    // 2. Gán vào Nút "TIẾP TỤC" trong Panel Pause
    public void ResumeGame()
    {
        if (currentState == GameState.Freeze)
        {
            SetState(stateBeforeFreeze); // Trả lại đúng trạng thái trước khi Pause
        }
    }

    // 3. Gán vào Nút "CÀI ĐẶT" trong Panel Pause
    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    // 4. Gán vào Nút "QUAY LẠI" trong Panel Settings
    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // 5. Gán vào Nút "VỀ MENU CHÍNH" trong Panel Pause
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Luôn reset timeScale về 1 trước khi chuyển Scene
        SceneManager.LoadScene(0); // Scene 0 là MainMenu
    }

    // 6. Gán vào Nút "THOÁT GAME" trong Panel Pause
    public void QuitGame()
    {
        Debug.Log("Đang thoát game...");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // --- HỆ THỐNG WAVE & TIẾN TRÌNH ---

    private void HealAllTowers()
    {
        TowerHealth[] allTowers = FindObjectsByType<TowerHealth>(FindObjectsSortMode.None);
        foreach (TowerHealth tower in allTowers)
        {
            tower.HealToFull();
            Debug.Log("Đã hồi phục cho toàn bộ tháp");
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
            Debug.Log("🏆 Bạn đã phá đảo toàn bộ các Map!");
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
            Debug.Log($"💾 Đã mở khóa Màn tiếp theo: {currentSceneIndex + 1}");
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
}