using UnityEngine;

public class GameEconomy : MonoBehaviour
{
    public static GameEconomy Instance;

    [Header("Tài sản")]
    public int gold = 50; // Cho sẵn 100 vàng làm vốn khởi nghiệp


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
    

    public void UpdateUI()
    {
        // Tự động báo sang Canvas Prefab để cập nhật số tiền hiển thị
        if (GameplayCanvasController.Instance != null)
        {
            GameplayCanvasController.Instance.UpdateGoldDisplay(gold);
        }
    }
}