using UnityEngine;

public class TowerDrag : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool isDragging = false;
    
    public string elementType = "Lua"; 
    public int towerLevel = 1;

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
        
        // Tọa độ ô lưới mà chuột đang thả ra
        Vector2 snapPos = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        
        // Dùng tia vật lý quét xem ở ô này CÓ THÁP NÀO KHÁC đang đứng không?
        Collider2D[] colliders = Physics2D.OverlapPointAll(snapPos);
        TowerDrag targetTower = null;

        foreach (Collider2D col in colliders)
        {
            TowerDrag tower = col.GetComponent<TowerDrag>();
            if (tower != null && tower != this) // Tìm thấy tháp khác (không phải chính mình)
            {
                targetTower = tower;
                break;
            }
        }

        if (targetTower != null)
        {
            // CÓ THÁP KHÁC Ở ĐÓ -> Kích hoạt thuật toán Gộp
            bool canMerge = MergeManager.Instance.TryMerge(this, targetTower);
            
            if (!canMerge) 
            {
                // Nếu sai công thức, gộp thất bại -> Nảy về chỗ cũ lúc mới nhấc lên
                transform.position = originalPosition;
            }
        }
        else
        {
            // Ô TRỐNG -> Đặt tháp xuống bình thường
            transform.position = snapPos;
        }
    }
}