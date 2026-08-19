using UnityEngine;

public class BenchManager : MonoBehaviour
{
    public static BenchManager Instance;

    public Transform[] benchSlots; // Array chứa 6 Transform của BenchSlot_0 -> BenchSlot_5
    private GameObject[] slotOccupants; // Mảng lưu trữ con tháp đang đứng ở từng slot

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        slotOccupants = new GameObject[benchSlots.Length];
    }

    // HÀM TỰ ĐỘNG ÉP THÁP VỀ INACTIVE KHI VÀO Ô HÀNG CHỜ
    public bool AddTowerToBenchSlot(GameObject tower, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= benchSlots.Length) return false;

        // 1. Lưu tháp vào mảng quản lý
        slotOccupants[slotIndex] = tower;
        tower.transform.position = benchSlots[slotIndex].position;

        // 2. [QUY TẮC BẤT BIẾN] Ép tháp về trạng thái Inactive (Mờ 50%, ngưng bắn)
        TowerController controller = tower.GetComponent<TowerController>();
        if (controller != null)
        {
            controller.SetOperational(false);
        }

        Debug.Log($"📦 {tower.name} đã vào Hàng chờ (Ô {slotIndex}) -> Trạng thái: INACTIVE");
        return true;
    }

    // Hàm thêm tháp vào ô trống đầu tiên (Dùng khi mua từ Shop)
    public bool AddTowerToBench(GameObject tower)
    {
        int emptySlot = GetFirstEmptySlotIndex();
        if (emptySlot != -1)
        {
            return AddTowerToBenchSlot(tower, emptySlot);
        }

        Debug.LogWarning("❌ Hàng chờ đã đầy! Không thể thêm tháp.");
        return false;
    }

    // Lấy chỉ số ô trống đầu tiên
    public int GetFirstEmptySlotIndex()
    {
        for (int i = 0; i < slotOccupants.Length; i++)
        {
            if (slotOccupants[i] == null) return i;
        }
        return -1; // Đầy hàng chờ
    }
    public bool HasEmptySlot()
{
    return GetFirstEmptySlotIndex() != -1;
}

    // Kiểm tra ô slot có trống không
    public bool IsSlotEmpty(int index)
    {
        if (index < 0 || index >= slotOccupants.Length) return false;
        return slotOccupants[index] == null;
    }

    // Tìm ô hàng chờ gần nhất với tọa độ nhả chuột
    public int GetNearestSlotIndex(Vector3 position, float maxDistance = 0.8f)
    {
        int nearestIndex = -1;
        float shortestDistance = maxDistance;

        for (int i = 0; i < benchSlots.Length; i++)
        {
            float dist = Vector2.Distance(position, benchSlots[i].position);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                nearestIndex = i;
            }
        }
        return nearestIndex;
    }

    // Xóa đăng ký tháp khỏi Hàng chờ khi tháp bị kéo lên sân hoặc bị xóa/gộp
    public void RemoveTowerFromBench(GameObject tower)
    {
        for (int i = 0; i < slotOccupants.Length; i++)
        {
            if (slotOccupants[i] == tower)
            {
                slotOccupants[i] = null;
                break;
            }
        }
    }
}