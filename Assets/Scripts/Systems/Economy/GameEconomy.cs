using UnityEngine;

public class GameEconomy : MonoBehaviour
{
    public static GameEconomy Instance;

    [Header("Tài sản")]
    public int gold = 100; // Cho sẵn 100 vàng làm vốn khởi nghiệp

    [Header("Cửa hàng")]
    public GameObject towerPrefab;   // Bản mẫu của Tháp
    public int towerCost = 50;       // Giá 1 tháp
    public Transform shopSpawnPoint; // Vị trí tháp rơi xuống khi mua

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateUI();
    }

    // Hàm mua tháp
    public void BuyTower()
    {
        if (gold >= towerCost)
        {
            gold -= towerCost;
            UpdateUI();

            // Sinh ra 1 tháp mới tại vị trí Shop
            Instantiate(towerPrefab, shopSpawnPoint.position, Quaternion.identity);
            Debug.Log("Mua tháp thành công!");
        }
        else
        {
            Debug.Log("Đỗ nghèo khỉ! Không đủ vàng.");
        }
    }

    public void UpdateUI()
    {
        // Tự động báo sang Canvas Prefab để cập nhật số tiền hiển thị
        if (GameplayCanvasController.Instance != null)
        {
            GameplayCanvasController.Instance.UpdateGoldDisplay(gold);
        }
    }
}