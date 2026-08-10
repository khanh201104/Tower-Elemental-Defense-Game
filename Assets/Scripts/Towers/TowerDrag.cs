using UnityEngine;
using UnityEngine.Tilemaps; // Bắt buộc phải thêm dòng này để Unity hiểu Tilemap là gì

public class TowerDrag : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool isDragging = false;
    
    public string elementType = "Lua"; 
    public int towerLevel = 1;

    // Khai báo Tilemap để nhận diện ô Đặt Tháp
    public Tilemap placementTilemap;

    // Các hàm phải nằm ngoài cùng thế này, không bị lọt vào hàm nào khác
    void OnMouseDown()
    {
        Debug.Log("Đã click chuột vào tháp!");
        originalPosition = transform.position;
        isDragging = true;
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
    
    // Tự động tìm cái Tilemap có tên chính xác là "Tilemap" trong Scene
    if (placementTilemap == null)
    {
        GameObject tilemapObj = GameObject.Find("Tilemap"); 
        if (tilemapObj != null)
        {
            placementTilemap = tilemapObj.GetComponent<Tilemap>();
        }
    }

    // Nếu vẫn không tìm thấy thì báo lỗi để kiểm tra lại tên
    if (placementTilemap == null)
    {
        Debug.LogError("Không tìm thấy Tilemap nào tên là 'Tilemap' trong Scene!");
        transform.position = originalPosition;
        return;
    }

    // --- Các đoạn code bên dưới giữ nguyên y cũ của m ---
    Vector3Int cellPos = placementTilemap.WorldToCell(transform.position);
    
    if (placementTilemap.HasTile(cellPos))
    {
        Vector3 snapPos = placementTilemap.GetCellCenterWorld(cellPos);
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
                transform.position = originalPosition;
            }
        }
        else
        {
            transform.position = snapPos;
        }
    }
    else
    {
        transform.position = originalPosition;
    }
}
}