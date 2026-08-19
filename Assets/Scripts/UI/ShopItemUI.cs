using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI Thành Phần")]
    public Image iconImage;
    public Text nameText;
    public Text priceText;
    public Button buyButton;

    private ShopItemData itemData;

    // Hàm thiết lập thông tin hiển thị cho ô vật phẩm
    public void Setup(ShopItemData data)
    {
        itemData = data;

        if (iconImage != null) iconImage.sprite = data.itemIcon;
        if (nameText != null) nameText.text = data.itemName;
        if (priceText != null) priceText.text = data.itemPrice + " Vàng";

        // Gán sự kiện click cho Nút Mua
        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyClicked);
        }
    }

    void OnBuyClicked()
    {
        if (ShopManager.Instance != null && itemData != null)
        {
            ShopManager.Instance.BuyItem(itemData);
        }
    }
}