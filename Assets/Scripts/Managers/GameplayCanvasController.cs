using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameplayCanvasController : MonoBehaviour
{
    public static GameplayCanvasController Instance; // Cho phép các Manager khác gọi cập nhật UI dễ dàng

    [Header("--- UI Header ---")]
    public GameObject headerPanel;      // Kéo Panel_Header vào đây
    public Text levelTitleText;         // Kéo Text_LevelTitle vào đây
    public Text goldText;               // Kéo Txt_Gold vào đây

    [Header("--- Toàn Bộ Panel Trong Canvas ---")]
    public GameObject footerPanel;
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    [Header("--- Nút Header / Màn Hình Chính ---")]
    public Button pauseButton;
    public Button nextWaveButton;
    public Button shopButton;

    [Header("--- Nút Trong Panel Pause ---")]
    public Button resumeButton;
    public Button settingsButton;
    public Button mainMenuButton;
    public Button quitButton;
    public Button closeSettingsButton;

    [Header("--- Nút Trong Panel Game Over & Victory ---")]
    public Button restartButtonGameOver;
    public Button restartButtonVictory;
    public Button nextLevelButton;
    public Button mainMenuButtonGameOver;
    public Button mainMenuButtonVictory;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 1. TỰ ĐỘNG CẬP NHẬT TÊN MÀN CHƠI
        UpdateLevelHeader();

        // 2. KHỞI TẠO TIỀN BAN ĐẦU
        if (GameEconomy.Instance != null)
        {
            UpdateGoldDisplay(GameEconomy.Instance.gold);
        }

        // 3. ĐĂNG KÝ CÁC PANEL VỚI GAMEMANAGER
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterUIPanels(footerPanel, pauseMenuPanel, settingsPanel, gameOverPanel, victoryPanel);
        }

        // 4. GẮN TOÀN BỘ SỰ KIỆN NÚT BẤM (AddListener)
        SetupButtonListeners();
    }

    // --- CÁC HÀM CẬP NHẬT HEADER ---

    public void UpdateLevelHeader()
    {
        if (levelTitleText != null)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            levelTitleText.text = sceneName.Replace("Map_", "Màn ").Replace("_", " ");
        }
    }

    // Hàm cập nhật hiển thị tiền vàng
    public void UpdateGoldDisplay(int gold)
    {
        if (goldText != null)
        {
            goldText.text = $"Vàng: {gold}"; // hoặc $"{gold} G"
        }
    }

    // --- GẮN SỰ KIỆN CHO BUTTON ---
    private void SetupButtonListeners()
    {
        if (pauseButton != null)
            pauseButton.onClick.AddListener(() => GameManager.Instance?.FreezeGame());

        if (nextWaveButton != null)
            nextWaveButton.onClick.AddListener(() => GameManager.Instance?.OnClickStartWaveButton());

        if (shopButton != null)
            shopButton.onClick.AddListener(() => ShopManager.Instance?.OpenShop());

        if (resumeButton != null)
            resumeButton.onClick.AddListener(() => GameManager.Instance?.ResumeGame());

        if (settingsButton != null)
            settingsButton.onClick.AddListener(() => GameManager.Instance?.OpenSettings());

        if (closeSettingsButton != null)
            closeSettingsButton.onClick.AddListener(() => GameManager.Instance?.CloseSettings());

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(() => GameManager.Instance?.ReturnToMainMenu());

        if (quitButton != null)
            quitButton.onClick.AddListener(() => GameManager.Instance?.QuitGame());

        if (restartButtonGameOver != null)
            restartButtonGameOver.onClick.AddListener(() => GameManager.Instance?.RestartGame());

        if (restartButtonVictory != null)
            restartButtonVictory.onClick.AddListener(() => GameManager.Instance?.RestartGame());

        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(() => GameManager.Instance?.NextLevel());

        if (mainMenuButtonGameOver != null)
            mainMenuButtonGameOver.onClick.AddListener(() => GameManager.Instance?.ReturnToMainMenu());

        if (mainMenuButtonVictory != null)
            mainMenuButtonVictory.onClick.AddListener(() => GameManager.Instance?.ReturnToMainMenu());
    }
}