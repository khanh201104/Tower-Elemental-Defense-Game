using UnityEngine;
using UnityEngine.UI; // Dùng cho Text và Image Legacy

[System.Serializable]
public class BasicTowerInfo
{
    public string towerName = "Tháp Lửa Lv1";
    public Sprite towerIcon;
    public GameObject towerPrefab;
}

public class WaveRewardManager : MonoBehaviour
{
    public static WaveRewardManager Instance;

    [Header("Bảng Panel Phần Thưởng")]
    public GameObject rewardPanel;

    [Header("Cấu Hình Giá Tháp Cơ Bản")]
    public int baseTowerPrice = 50; // Giá tháp Lv1 để tính % tiền thưởng

    [Header("Danh Sách 3 Tháp Nguyên Tố Cơ Bản (Lửa, Băng, Sét/Độc...)")]
    public BasicTowerInfo[] basicTowers;

    [Header("UI Card 1 - Tháp Công Khai")]
    public Image card1Icon;
    public Text card1Title;
    public Text card1Desc;

    [Header("UI Card 2 - Tháp Ẩn Danh + 10% Vàng")]
    public Sprite mysteryIcon; // Icon Dấu hỏi hoặc Hộp quà bí ẩn
    public Image card2Icon;
    public Text card2Title;
    public Text card2Desc;

    [Header("UI Card 3 - 90% Vàng")]
    public Sprite goldIcon;    // Icon Túi vàng
    public Image card3Icon;
    public Text card3Title;
    public Text card3Desc;

    // Lưu lại tháp ngẫu nhiên được chọn cho Thẻ 1 của wave này
    private BasicTowerInfo selectedOption1Tower;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (rewardPanel != null)
        {
            rewardPanel.SetActive(false);
        }
    }

    // --- HÀM TỰ ĐỘNG BỐC THẺ VÀ HIỂN THỊ UI ---
    public void ShowRewardPanel()
    {
        if (rewardPanel == null)
        {
            Debug.LogError("❌ LỖI: Chưa kéo Panel_WaveReward vào ô 'Reward Panel' trong Inspector của WaveRewardManager!");
            return;
        }

        if (basicTowers == null || basicTowers.Length == 0)
        {
            Debug.LogError("❌ LỖI: Mảng 'Basic Towers' đang rỗng (Size = 0)! Hãy nhập ít nhất 1 tháp cơ bản.");
            return;
        }

        // 1. Thẻ 1: Chọn ngẫu nhiên 1 trong các tháp cơ bản và gán Icon/Tên công khai
        selectedOption1Tower = basicTowers[Random.Range(0, basicTowers.Length)];
        if (card1Icon != null) card1Icon.sprite = selectedOption1Tower.towerIcon;
        if (card1Title != null) card1Title.text = selectedOption1Tower.towerName;
        if (card1Desc != null) card1Desc.text = $"Nhận ngay 1 {selectedOption1Tower.towerName} vào Hàng chờ.";

        // 2. Thẻ 2: Tháp ẩn danh + 10% giá trị tháp
        int bonusGold2 = Mathf.RoundToInt(baseTowerPrice * 0.1f);
        if (card2Icon != null && mysteryIcon != null) card2Icon.sprite = mysteryIcon;
        if (card2Title != null) card2Title.text = "Tháp Bí Ẩn";
        if (card2Desc != null) card2Desc.text = $"Nhận 1 Tháp Nguyên Tố ngẫu nhiên + {bonusGold2} Vàng.";

        // 3. Thẻ 3: 90% giá trị tháp bằng Vàng
        int bonusGold3 = Mathf.RoundToInt(baseTowerPrice * 0.9f);
        if (card3Icon != null && goldIcon != null) card3Icon.sprite = goldIcon;
        if (card3Title != null) card3Title.text = "Túi Vàng Thưởng";
        if (card3Desc != null) card3Desc.text = $"Nhận ngay {bonusGold3} Vàng (90% giá Tháp Lv1).";

        // Mở Panel
        rewardPanel.SetActive(true);
    }

    // --- XỬ LÝ KHI CHỌN CARD 1 (Tháp Công Khai) ---
    public void OnSelectOption1()
    {
        if (!CheckBenchSpace()) return;

        if (selectedOption1Tower != null && selectedOption1Tower.towerPrefab != null)
        {
            GameObject towerObj = Instantiate(selectedOption1Tower.towerPrefab);
            BenchManager.Instance.AddTowerToBench(towerObj);
            Debug.Log($"🎁 Đã nhận tháp công khai: {selectedOption1Tower.towerName}");
        }

        ClosePanel();
    }

    // --- XỬ LÝ KHI CHỌN CARD 2 (Tháp Ngẫu Nhiên + 10% Vàng) ---
    public void OnSelectOption2()
    {
        if (!CheckBenchSpace()) return;

        BasicTowerInfo randomTower = basicTowers[Random.Range(0, basicTowers.Length)];
        if (randomTower != null && randomTower.towerPrefab != null)
        {
            GameObject towerObj = Instantiate(randomTower.towerPrefab);
            BenchManager.Instance.AddTowerToBench(towerObj);
        }

        int bonusGold = Mathf.RoundToInt(baseTowerPrice * 0.1f);
        if (GameEconomy.Instance != null)
        {
            GameEconomy.Instance.AddGold(bonusGold);
        }

        Debug.Log($"🎁 Đã nhận: {randomTower.towerName} + {bonusGold} Vàng!");
        ClosePanel();
    }

    // --- XỬ LÝ KHI CHỌN CARD 3 (90% Tiền Mặt) ---
    public void OnSelectOption3()
    {
        int bonusGold = Mathf.RoundToInt(baseTowerPrice * 0.9f);
        if (GameEconomy.Instance != null)
        {
            GameEconomy.Instance.AddGold(bonusGold);
        }

        Debug.Log($"🎁 Đã nhận thưởng: {bonusGold} Vàng!");
        ClosePanel();
    }

    private bool CheckBenchSpace()
    {
        if (BenchManager.Instance == null || !BenchManager.Instance.HasEmptySlot())
        {
            Debug.LogWarning("❌ Hàng chờ (Bench) đã đầy! Hãy dọn chỗ trước.");
            return false;
        }
        return true;
    }

    private void ClosePanel()
    {
        if (rewardPanel != null)
        {
            rewardPanel.SetActive(false);
        }
    }
}