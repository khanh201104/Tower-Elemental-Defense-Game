using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ShopItemData
{
    public string itemName = "Tháp Lửa";
    public Sprite itemIcon;          
    public int itemPrice = 50;       
    public GameObject towerPrefab;   
}

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("Bảng Cửa Hàng (Shop Modal)")]
    public GameObject shopModal;       
    public Button closeButton;         
    public TextMeshProUGUI currentGoldText; 

    [Header("Cấu Hình Thẻ Cửa Hàng (Cards)")]
    public Transform contentArea;      
    public GameObject shopCardPrefab;  

    [Header("Danh Sách Tháp Bày Bán")]
    public List<ShopItemData> shopItems;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (shopModal != null)
        {
            shopModal.SetActive(false);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseShop);
        }

        GenerateShopCards();
    }

    public void OpenShop()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameState.Pause)
        {
            Debug.LogWarning("⚠️ Chỉ có thể mở Cửa Hàng trong giai đoạn chuẩn bị giữa các Wave!");
            return;
        }

        if (shopModal != null)
        {
            shopModal.SetActive(true);
            UpdateGoldUI(); 
        }
    }

    public void CloseShop()
    {
        if (shopModal != null)
        {
            shopModal.SetActive(false);
        }
    }

    private void UpdateGoldUI()
    {
        if (currentGoldText != null && GameEconomy.Instance != null)
        {
            currentGoldText.text = $"Gold: {GameEconomy.Instance.gold}";
        }
    }

    public void GenerateShopCards()
    {
        if (contentArea == null || shopCardPrefab == null) return;

        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in shopItems)
        {
            GameObject cardObj = Instantiate(shopCardPrefab, contentArea);
            
            ShopCardUI cardUI = cardObj.GetComponent<ShopCardUI>();
            if (cardUI != null)
            {
                cardUI.Setup(item);
            }
        }
    }

    public void BuyItem(ShopItemData item)
    {
        if (item == null || item.towerPrefab == null) return;

        if (BenchManager.Instance == null || !BenchManager.Instance.HasEmptySlot())
        {
            Debug.LogWarning("❌ Hàng chờ đã đầy! Hãy dọn chỗ hoặc gộp tháp trước.");
            return;
        }

        if (GameEconomy.Instance != null)
        {
            if (GameEconomy.Instance.gold < item.itemPrice)
            {
                Debug.Log($"❌ Không đủ vàng để mua {item.itemName}! Cần: {item.itemPrice} Vàng.");
                return;
            }

            GameEconomy.Instance.AddGold(-item.itemPrice);
            UpdateGoldUI(); 
        }

        GameObject newTower = Instantiate(item.towerPrefab);
        bool success = BenchManager.Instance.AddTowerToBench(newTower);

        if (success)
        {
            Debug.Log($"🛒 Mua thành công {item.itemName}!");
        }
        else
        {
            Destroy(newTower);
            if (GameEconomy.Instance != null)
            {
                GameEconomy.Instance.AddGold(item.itemPrice);
                UpdateGoldUI(); 
            }
        }
    }
}