using UnityEngine;
using UnityEngine.SceneManagement; // Dùng để load lại Scene khi chơi lại

public enum GameState
{
    Pause,    // Trạng thái chuẩn bị
    Resume,   // Trạng thái chiến đấu
    GameOver, // Thua
    Victory   // Thắng
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Trạng Thái Hiện Tại")]
    public GameState currentState = GameState.Pause;

    [Header("UI Panels (Kéo thả từ Canvas vào đây)")]
    public GameObject footerPanel;   // Thanh Footer dưới
    public GameObject gameOverPanel; // Bảng Thua
    public GameObject victoryPanel;  // Bảng Thắng (Tùy chọn)

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f; // Đảm bảo thời gian game luôn chạy bình thường khi bắt đầu
        SetState(GameState.Pause);
    }

    public void SetState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.Pause:
                Time.timeScale = 1f; // Mở lại dòng thời gian
                if (footerPanel != null) footerPanel.SetActive(true);
                if (gameOverPanel != null) gameOverPanel.SetActive(false);
                if (victoryPanel != null) victoryPanel.SetActive(false);

                // Hồi 100% HP cho toàn bộ tháp
                HealAllTowers();
                break;

            case GameState.Resume:
                Time.timeScale = 1f;
                if (footerPanel != null) footerPanel.SetActive(false);
                
                if (ShopManager.Instance != null)
                {
                    ShopManager.Instance.CloseShop();
                }
                break;

            case GameState.GameOver:
                Debug.Log("💀 GAME OVER: Nhà chính đã bị phá hủy!");
                if (footerPanel != null) footerPanel.SetActive(false);
                if (gameOverPanel != null) gameOverPanel.SetActive(true);
                
                Time.timeScale = 0f; // Dừng toàn bộ thời gian (tháp dừng bắn, quái dừng đi)
                break;

            case GameState.Victory:
                Debug.Log("🎉 VICTORY: Đã dọn sạch toàn bộ các Wave!");
                if (footerPanel != null) footerPanel.SetActive(false);
                if (victoryPanel != null) victoryPanel.SetActive(true);

                Time.timeScale = 0f; // Đóng băng thời gian mừng chiến thắng
                break;
        }
    }

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

    // HÀM CHƠI LẠI (Gán vào Nút Restart / Play Again trên Bảng Game Over / Victory)
    public void RestartGame()
    {
        Time.timeScale = 1f; // Mở lại thời gian trước khi reset scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}