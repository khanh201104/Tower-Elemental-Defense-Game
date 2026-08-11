using UnityEngine;
using UnityEngine.UI; // Thư viện để can thiệp vào giao diện (Chữ, Nút bấm)

public class GameEconomy : MonoBehaviour
{
    public static GameEconomy Instance;
    
    [Header("Tài sản")]
    public int gold = 100; // Cho sẵn 100 vàng làm vốn khởi nghiệp
    public Text goldText;  // Chữ hiển thị vàng

    [Header("Cửa hàng")]
    public GameObject towerPrefab;   // Bản mẫu của Tháp
    public int towerCost = 50;       // Giá 1 tháp
    public Transform shopSpawnPoint; // Vị trí tháp rơi xuống khi mua

    void Awake()
    {
        Instance = this;
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

    // Hàm này sẽ được gán vào Nút bấm trên màn hình
    public void BuyTower()
    {
        if (gold >= towerCost)
        {
            gold -= towerCost;
            UpdateUI();
            
            // Đẻ ra 1 cái tháp mới tại vị trí Shop
            Instantiate(towerPrefab, shopSpawnPoint.position, Quaternion.identity);
            Debug.Log("Mua tháp thành công!");
        }
        else
        {
            Debug.Log("Đỗ nghèo khỉ! Không đủ vàng.");
        }
    }

    void UpdateUI()
    {
        if (goldText != null)
        {
            goldText.text = "Vàng: " + gold;
        }
    }
}