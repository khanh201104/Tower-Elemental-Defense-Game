using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerDrag : MonoBehaviour
{
    public Vector3 originalPosition; // Để public cho MergeManager đọc vị trí gốc
    private bool isDragging = false;
    
    public string elementType = "Lua"; 
    public int towerLevel = 1;

    public Tilemap placementTilemap;
    private TowerController towerController;

    // Biến đánh dấu: Tháp này xuất phát từ Hàng chờ hay từ Sân đấu
    private bool startedOnBench = false;

    void Start()
    {
        towerController = GetComponent<TowerController>();
    }

    void OnMouseDown()
    {
        // 1. KHÓA TƯƠNG TÁC KHI ĐANG CHIẾN ĐẤU (RESUME)
        if (GameManager.Instance != null && GameManager.Instance.currentState == GameState.Resume)
        {
            Debug.LogWarning("🔒 Wave đang chạy (Resume)! Không thể di chuyển/kéo thả tháp.");
            return;
        }

        Debug.Log("Đã nhấc tháp!");
        originalPosition = transform.position; 
        isDragging = true;

        if (BenchManager.Instance != null)
        {
            startedOnBench = (BenchManager.Instance.GetNearestSlotIndex(originalPosition) != -1);
        }

        // [MỚI FIX] Khi nhấc tháp lên cầm trên tay -> Làm mờ 50% (0.5f)
        if (towerController != null)
        {
            towerController.SetAlpha(0.5f);
        }
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; 
        transform.position = mousePos;
    }

    void OnMouseUp()
    {
        Vector3 dropPos = transform.position;

        if (!isDragging) return;
        isDragging = false;

        // Tự động tìm Tilemap sân đấu nếu chưa gán
        if (placementTilemap == null && TowerPlacementManager.Instance != null)
        {
            placementTilemap = TowerPlacementManager.Instance.mainTilemap;
        }

        Vector3Int cellPos = placementTilemap != null ? placementTilemap.WorldToCell(transform.position) : Vector3Int.zero;
        Vector3 snapPos = placementTilemap != null ? placementTilemap.GetCellCenterWorld(cellPos) : transform.position;

        // -------------------------------------------------------------
        // A. XỬ LÝ GỘP THÁP (Thả đè lên con tháp khác)
        // -------------------------------------------------------------
        Collider2D[] colliders = Physics2D.OverlapPointAll(snapPos);
        TowerDrag targetTower = null;

        foreach (Collider2D col in colliders)
        {
            TowerDrag tower = col.GetComponent<TowerDrag>();
            if (tower != null && tower != this)
            {
                targetTower = tower;
                break;
            }
        }

        if (targetTower != null)
        {
            bool canMerge = MergeManager.Instance.TryMerge(this, targetTower);
            if (!canMerge)
            {
                ResetToOriginalPosition();
            }
            return;
        }

        // -------------------------------------------------------------
        // B. XỬ LÝ KÉO THÁP VỀ HÀNG CHỜ (CẢ TỪ HÀNG CHỜ VÀ TỪ SÂN ĐẤU)
        // -------------------------------------------------------------
        if (BenchManager.Instance != null)
        {
            int benchSlotIdx = BenchManager.Instance.GetNearestSlotIndex(transform.position);

            if (benchSlotIdx != -1)
            {
                // Kiểm tra xem ô Hàng chờ đó có trống không (hoặc là chính ô nó vừa nhấc đi)
                if (BenchManager.Instance.IsSlotEmpty(benchSlotIdx) || 
                    BenchManager.Instance.benchSlots[benchSlotIdx].position == originalPosition)
                {
                    // 1. Giải phóng ô Tilemap nếu tháp trước đó nằm trên Sân đấu
                    if (!startedOnBench && TowerPlacementManager.Instance != null)
                    {
                        TowerPlacementManager.Instance.ClearTile(originalPosition);
                    }
                    // Nếu tháp trước đó nằm ở ô Hàng chờ khác -> Xóa đăng ký ô cũ
                    else if (startedOnBench)
                    {
                        BenchManager.Instance.RemoveTowerFromBench(gameObject);
                    }

                    // 2. Đăng ký tháp vào ô Hàng chờ mới
                    BenchManager.Instance.AddTowerToBenchSlot(gameObject, benchSlotIdx);

                    // 3. Ép trạng thái về INACTIVE (Tháp mờ 50%, ngưng bắn)
                    if (towerController != null)
                    {
                        towerController.SetOperational(false);
                    }

                    Debug.Log($"📦 Đã đặt tháp vào HÀNG CHỜ (Ô {benchSlotIdx}) -> Trạng thái: Inactive!");
                    return;
                }
            }
        }

        // -------------------------------------------------------------
        // C. XỬ LÝ KÉO ĐẶT VÀO Ô TILEMAP TRÊN SÂN ĐẤU
        // -------------------------------------------------------------
        if (TowerPlacementManager.Instance != null && TowerPlacementManager.Instance.IsValidPlacement(transform.position))
        {
            bool moveSuccess = TowerPlacementManager.Instance.TryMoveTower(gameObject, originalPosition, transform.position);

            if (moveSuccess)
            {
                // Nếu tháp này kéo từ Hàng chờ lên Sân đấu -> Xóa nó khỏi danh sách Hàng chờ
                if (startedOnBench && BenchManager.Instance != null)
                {
                    BenchManager.Instance.RemoveTowerFromBench(gameObject);
                }

                // Ép BẬT HOẠT ĐỘNG (Tháp sáng 100%, tự đếm time và bắn quái)
                if (towerController != null)
                {
                    towerController.SetOperational(true);
                }

                Debug.Log("⚔️ Đã đặt tháp lên SÂN ĐẤU -> Trạng thái: Active!");
                return;
            }
        }

        // -------------------------------------------------------------
        // D. VỊ TRÍ KHÔNG HỢP LỆ -> RESET VỀ VỊ TRÍ CŨ
        // -------------------------------------------------------------
        ResetToOriginalPosition();

        // -------------------------------------------------------------
        // X. KIỂM TRA BÁN THÁP (Kéo thả vào Ô Bán / Thùng Rác)
        // -------------------------------------------------------------
        if (SellZone.Instance != null && SellZone.Instance.IsInSellZone(dropPos))
        {
            SellZone.Instance.SellTower(this);
            return; // Đã bán xong -> Dừng hàm
        }
    }

    void ResetToOriginalPosition()
    {
        transform.position = originalPosition;

        // Nếu tháp vốn đứng ở Sân đấu (!startedOnBench) -> Trả về sân và BẬT lại hoạt động
        if (!startedOnBench && towerController != null)
        {
            towerController.SetOperational(true);
        }
        // Nếu tháp vốn thuộc Hàng chờ (startedOnBench) -> Trả về khay Hàng chờ và giữ INACTIVE
        else if (startedOnBench && towerController != null)
        {
            towerController.SetOperational(false);
        }
    }
}