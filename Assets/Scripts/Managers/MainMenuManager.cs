using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Các Nút Menu Chính")]
    public Button continueButton;     // Nút Tiếp tục (chỉ sáng khi đã qua màn 1)
    public Button newGameButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Các Bảng Phụ (Popup Panels)")]
    public GameObject levelSelectPanel; // Bảng Chọn màn chơi
    public GameObject settingsPanel;    // Bảng Cài đặt

    [Header("Các Nút Chọn Màn Trong Level Select")]
    public Button[] levelButtons;      // Element 0 = Màn 1, Element 1 = Màn 2, Element 2 = Màn 3

    void Start()
    {
        Time.timeScale = 1f;

        // 1. Lấy tiến trình cao nhất (Mặc định là 1 nếu là người mới)
        int highestUnlocked = PlayerPrefs.GetInt("HighestUnlockedLevel", 1);

        // 2. Nút "Tiếp tục" chỉ mở khi đã vượt qua ít nhất màn 1 (highest >= 2)
        if (continueButton != null)
        {
            continueButton.interactable = (highestUnlocked > 1);
        }

        // 3. Khóa/Mở các nút trong Bảng Chọn Màn
        UpdateLevelSelectButtons(highestUnlocked);

        // 4. Ẩn các popup phụ khi bắt đầu
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    void UpdateLevelSelectButtons(int highestUnlocked)
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                int levelIndex = i + 1; // Màn 1, Màn 2, Màn 3
                bool isUnlocked = (levelIndex <= highestUnlocked);

                // Khóa bấm nếu chưa qua màn trước
                levelButtons[i].interactable = isUnlocked;

                // Đổi text hoặc thêm nhãn khóa
                Text btnText = levelButtons[i].GetComponentInChildren<Text>();
                if (btnText != null)
                {
                    btnText.text = isUnlocked ? $"Màn {levelIndex}" : $"Màn {levelIndex} 🔒";
                }
            }
        }
    }

    // --- CÁC SỰ KIỆN NÚT BẤM ---

    // 1. CHƠI MỚI: Luôn tải Màn 1
    public void OnNewGameClicked()
    {
        // Có thể reset lại tiến trình nếu muốn, hoặc chơi thẳng Màn 1
        SceneManager.LoadScene(1); // Tải Map_1.1 (Scene Index 1)
    }

    // 2. TIẾP TỤC: Mở Bảng Chọn Màn để người chơi chọn màn đã mở
    public void OnContinueClicked()
    {
        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(true);
        }
    }

    // Chọn vào 1 màn cụ thể trong Bảng Chọn Màn
    public void LoadSelectedLevel(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // 3. CÀI ĐẶT: Mở Bảng Cài đặt
    public void OnSettingsClicked()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    // Đóng các popup để quay về Menu chính
    public void OnClosePopupClicked()
    {
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // 4. THOÁT GAME
    public void OnQuitGameClicked()
    {
        Debug.Log("Đang thoát game...");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}