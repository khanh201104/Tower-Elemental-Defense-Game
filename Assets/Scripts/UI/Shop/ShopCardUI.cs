using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopCardUI : MonoBehaviour
{
    [Header("UI Components")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI priceText;
    public Button buyButton;

    private ShopItemData currentData;

    // Nhận dữ liệu từ ShopManager truyền sang khi sinh UI
    public void Setup(ShopItemData data)
    {
        currentData = data;

        if (itemIcon != null && data.itemIcon != null)
        {
            itemIcon.sprite = data.itemIcon;
        }

        if (itemNameText != null)
        {
            itemNameText.text = data.itemName;
        }

        if (priceText != null)
        {
            priceText.text = $"{data.itemPrice} Vàng";
        }

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnClickBuyButton);
        }
    }

    private void OnClickBuyButton()
    {
        if (ShopManager.Instance != null && currentData != null)
        {
            ShopManager.Instance.BuyItem(currentData);
        }
    }
}