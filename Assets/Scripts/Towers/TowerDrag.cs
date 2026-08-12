using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerDrag : MonoBehaviour
{
    public Vector3 originalPosition; // Để public để MergeManager đọc vị trí gốc
    private bool isDragging = false;
    
    public string elementType = "Lua"; 
    public int towerLevel = 1;

    public Tilemap placementTilemap;
    private TowerController towerController;

    void Start()
    {
        towerController = GetComponent<TowerController>();
    }

    void OnMouseDown()
    {
        Debug.Log("Đã nhấc tháp lên!");
        originalPosition = transform.position; // Lưu lại vị trí ô gốc
        isDragging = true;

        // BỔ SUNG: Tạm thời TẮT HOẠT ĐỘNG khi đang cầm/kéo tháp
        if (towerController != null)
        {
            towerController.SetOperational(false); // Tháp mờ đi 50%, ngưng bắn, quái không đánh được
        }
    }

    void OnMouseDrag()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; 
        transform.position = mousePos;
    }

    void OnMouseUp()
    {
        isDragging = false;
        
        // Tự động tìm Tilemap nếu chưa gán
        if (placementTilemap == null)
        {
            if (TowerPlacementManager.Instance != null && TowerPlacementManager.Instance.mainTilemap != null)
            {
                placementTilemap = TowerPlacementManager.Instance.mainTilemap;
            }
            else
            {
                GameObject tilemapObj = GameObject.Find("Tilemap"); 
                if (tilemapObj != null)
                {
                    placementTilemap = tilemapObj.GetComponent<Tilemap>();
                }
            }
        }

        if (placementTilemap == null)
        {
            Debug.LogError("Không tìm thấy Tilemap nào trong Scene!");
            ResetToOriginalPosition();
            return;
        }

        Vector3Int cellPos = placementTilemap.WorldToCell(transform.position);
        Vector3 snapPos = placementTilemap.GetCellCenterWorld(cellPos);

        // 1. KIỂM TRA XEM CÓ MUỐN THẢ LÊN ĐẦU CON THÁP KHÁC ĐỂ GỘP KHÔNG
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

        // Trường hợp A: Thả đè lên tháp khác -> Thử Gộp Tháp
        if (targetTower != null)
        {
            bool canMerge = MergeManager.Instance.TryMerge(this, targetTower);
            if (!canMerge) 
            {
                // Không gộp được -> Trả về vị trí cũ và BẬT LẠI HOẠT ĐỘNG
                ResetToOriginalPosition();
            }
        }
        // Trường hợp B: Thả vào ô trống -> Gọi TowerPlacementManager để Di Chuyển Tháp
        else
        {
            if (TowerPlacementManager.Instance != null)
            {
                bool moveSuccess = TowerPlacementManager.Instance.TryMoveTower(gameObject, originalPosition, transform.position);
                
                if (moveSuccess)
                {
                    // Di chuyển thành công sang ô mới -> BẬT HOẠT ĐỘNG THÁP
                    if (towerController != null)
                    {
                        towerController.SetOperational(true);
                    }
                }
                else
                {
                    // Ô mới không hợp lệ -> Trả về vị trí cũ và BẬT LẠI HOẠT ĐỘNG
                    ResetToOriginalPosition();
                }
            }
            else
            {
                if (placementTilemap.HasTile(cellPos))
                {
                    transform.position = snapPos;
                    if (towerController != null) towerController.SetOperational(true);
                }
                else
                {
                    ResetToOriginalPosition();
                }
            }
        }
    }

    // Hàm trả tháp về vị trí cũ và bật lại trạng thái hoạt động
    void ResetToOriginalPosition()
    {
        transform.position = originalPosition;
        if (towerController != null)
        {
            towerController.SetOperational(true);
        }
    }
}