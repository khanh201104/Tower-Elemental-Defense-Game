using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopItemData
{
    public string itemName = "Tháp Lửa";
    public Sprite itemIcon;          // Ảnh hiển thị trong Shop
    public int itemPrice = 50;       // Giá tiền
    public GameObject towerPrefab;   // Prefab tháp tương ứng
}

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("Bảng Giao Diện Shop (Tab Pop-up)")]
    public GameObject shopPanel;       // Kéo khung Bảng Shop UI vào đây

    [Header("Cấu Hình Giao Diện Danh Sách Vật Phẩm")]
    public Transform itemContainer;   // Nơi chứa các ô Shop (Gán Grid Layout Group)
    public GameObject shopItemPrefab; // Prefab của 1 ô UI Vật phẩm

    [Header("Danh Sách Vật Phẩm Trong Shop")]
    public List<ShopItemData> shopItems;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        GenerateShopUI();

        // Mặc định ẨN TAB SHOP khi mới bắt đầu game
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    // --- HÀM ĐÓNG / MỚ TAB SHOP UI ---
    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    // Tự động đẻ ra danh sách ô UI vật phẩm theo danh sách shopItems
    public void GenerateShopUI()
    {
        if (itemContainer == null || shopItemPrefab == null) return;

        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in shopItems)
        {
            GameObject itemObj = Instantiate(shopItemPrefab, itemContainer);
            ShopItemUI itemUI = itemObj.GetComponent<ShopItemUI>();
            if (itemUI != null)
            {
                itemUI.Setup(item);
            }
        }
    }

    // Logic xử lý Mua Vật Phẩm
    public void BuyItem(ShopItemData item)
    {
        if (item == null || item.towerPrefab == null)
        {
            Debug.LogWarning("⚠️ Vật phẩm bị thiếu thông tin hoặc chưa gán Prefab tháp!");
            return;
        }

        // 1. Kiểm tra 6 ô Hàng chờ (Bench) còn trống không
        if (BenchManager.Instance == null || !BenchManager.Instance.HasEmptySlot())
        {
            Debug.LogWarning("❌ Hàng chờ (Bench) đã đầy 6/6 ô! Hãy kéo tháp ra sân hoặc gộp tháp trước.");
            return;
        }

        // 2. Kiểm tra Vàng
        if (GameEconomy.Instance != null)
        {
            if (GameEconomy.Instance.gold < item.itemPrice)
            {
                Debug.Log($"❌ Không đủ vàng để mua {item.itemName}! Giá: {item.itemPrice} Vàng.");
                return;
            }

            // Trừ vàng
            GameEconomy.Instance.AddGold(-item.itemPrice);
        }

        // 3. [MỚI FIX] ĐẺ THÁP RA SCENE (Instantiate) trước khi đẩy vào Hàng chờ
        GameObject newTower = Instantiate(item.towerPrefab);

        // 4. Chuyển tháp mới sinh ra xuống Hàng chờ (BenchManager tự ép Inactive)
        bool success = BenchManager.Instance.AddTowerToBench(newTower);

        if (success)
        {
            Debug.Log($"🛒 Mua thành công {item.itemName}! Đã xuất hiện dưới Hàng chờ.");
        }
        else
        {
            // Trường hợp hy hữu không vào được Hàng chờ -> Xóa tháp và hoàn tiền
            Destroy(newTower);
            if (GameEconomy.Instance != null)
            {
                GameEconomy.Instance.AddGold(item.itemPrice);
            }
            Debug.LogWarning("❌ Thất bại khi đẩy tháp vào Hàng chờ!");
        }
    }
}