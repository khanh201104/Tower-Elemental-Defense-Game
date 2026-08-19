using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameplayCanvasController : MonoBehaviour
{
    public static GameplayCanvasController Instance;

    public static bool IsGlobalRangeVisible = false;
    [Header("--- UI Header ---")]
    public GameObject headerPanel;
    public Text levelTitleText;
    public Text goldText;
    public Slider baseHealthSlider;      // Kéo Slider_BaseHealth vào đây
    public Image baseHealthFillImage;
    public Gradient healthGradient;
    public Text baseHealthText;          // Kéo Text hiển thị số máu vào đây (tùy chọn)
    public Button pauseButton;   
        // Kéo Button_Pause trong Header vào đây

    [Header("--- Toàn Bộ Panel Trong Canvas ---")]
    public GameObject footerPanel;
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    [Header("--- Nút Khác Ngoài Màn Hình ---")]
    public Button nextWaveButton;
    public Button shopButton;
    public Button toggleRangeButton;

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
        // Reset trạng thái hiển thị mỗi khi load lại Scene
        IsGlobalRangeVisible = false;
    }

    void Start()
    {
        UpdateLevelHeader();

        if (GameEconomy.Instance != null)
        {
            UpdateGoldDisplay(GameEconomy.Instance.gold);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterUIPanels(footerPanel, pauseMenuPanel, settingsPanel, gameOverPanel, victoryPanel);
        }

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

    public void UpdateGoldDisplay(int gold)
    {
        if (goldText != null)
        {
            goldText.text = "Vàng: " + gold;
        }
    }

    // Hàm cập nhật thanh máu và chữ số máu nhà chính
    public void UpdateBaseHealthDisplay(float currentHealth, float maxHealth)
    {
        if (baseHealthSlider != null)
        {
            baseHealthSlider.maxValue = maxHealth;
            baseHealthSlider.value = currentHealth;
        }

        // Đổi màu thanh Fill theo tỉ lệ % máu (0.0 -> 1.0)
        if (baseHealthFillImage != null && maxHealth > 0)
        {
            float healthPercent = currentHealth / maxHealth;
            
            // Cách 1: Dùng Gradient nếu đã thiết lập dải màu trong Inspector
            if (healthGradient != null)
            {
                baseHealthFillImage.color = healthGradient.Evaluate(healthPercent);
            }
            // Cách 2: Tự động chuyển Đỏ -> Xanh nếu chưa tạo Gradient
            else
            {
                baseHealthFillImage.color = Color.Lerp(Color.red, Color.green, healthPercent);
            }
        }

        if (baseHealthText != null)
        {
            baseHealthText.text = $"{Mathf.CeilToInt(currentHealth)}/{Mathf.CeilToInt(maxHealth)}";
        }
    }

    // --- GẮN SỰ KIỆN BUTTON ---
    private void SetupButtonListeners()
    {
        if (toggleRangeButton != null)
            toggleRangeButton.onClick.AddListener(ToggleAllTowerRanges);

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
    private void ToggleAllTowerRanges()
    {
        IsGlobalRangeVisible = !IsGlobalRangeVisible; // Đảo trạng thái

        // Tìm tất cả các tháp hiện có trên bản đồ (Kể cả trên sân và hàng chờ)
        TowerRange[] allTowers = FindObjectsByType<TowerRange>(FindObjectsSortMode.None);
        
        foreach (TowerRange tower in allTowers)
        {
            tower.ShowRange(IsGlobalRangeVisible);
        }
    }
    public void UpdateFooterButtons(GameState state)
    {
        // Chỉ cho phép bấm các nút này khi đang ở trạng thái Pause (Chuẩn bị)
        bool isInteractable = (state == GameState.Pause);

        if (nextWaveButton != null) 
            nextWaveButton.interactable = isInteractable;

        if (shopButton != null) 
            shopButton.interactable = isInteractable;

        // Lưu ý: Không can thiệp vào toggleRangeButton để nó luôn bấm được!
    }
}