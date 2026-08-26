using UnityEngine;

public class SellZone : MonoBehaviour
{
    public static SellZone Instance;

    [Header("Cấu hình Tỷ lệ Hoàn tiền")]
    [Range(0.1f, 1f)]
    public float refundRatio = 0.7f; // Hoàn lại 70% giá trị tháp

    private RectTransform rectTransform;
    private Collider2D zoneCollider;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Tự kiểm tra xem SellZone là UI Canvas hay là World Object 2D
        rectTransform = GetComponent<RectTransform>();
        zoneCollider = GetComponent<Collider2D>();
    }

    // Hàm kiểm tra vị trí thả tháp có trúng SellZone không
    public bool IsInSellZone(Vector3 dropWorldPosition)
    {
        // TH 1: NẾU SELLZONE LÀ UI CANVAS (Nằm trong Panel_Footer / Canvas)
        if (rectTransform != null)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(dropWorldPosition);
            return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint);
        }

        // TH 2: NẾU SELLZONE LÀ OBJECT 2D NGOÀI WORLD (Có BoxCollider2D)
        if (zoneCollider != null)
        {
            return zoneCollider.OverlapPoint(dropWorldPosition);
        }

        return false;
    }

    // Xử lý bán tháp
    public void SellTower(TowerDrag tower)
    {
        if (tower == null) return;

        // 1. Tính tiền hoàn trả dựa trên cấp tháp
        int basePrice = 50; // Giá tháp cơ bản cấp 1
        int calculatedValue = basePrice * tower.towerLevel; 
        int refundAmount = Mathf.RoundToInt(calculatedValue * refundRatio);

        // 2. Cộng vàng cho người chơi
        if (GameEconomy.Instance != null)
        {
            GameEconomy.Instance.AddGold(refundAmount);
            Debug.Log($"💰 Đã bán {tower.gameObject.name} (Lv {tower.towerLevel})! Nhận lại: {refundAmount} Vàng.");
        }

        // 3. Giải phóng ô đứng (Tilemap hoặc Hàng chờ)
        if (BenchManager.Instance != null)
        {
            BenchManager.Instance.RemoveTowerFromBench(tower.gameObject);
        }

        if (TowerPlacementManager.Instance != null)
        {
            TowerPlacementManager.Instance.ClearTile(tower.originalPosition);
        }

        // 4. Hủy GameObject tháp
        Destroy(tower.gameObject);
    }
}