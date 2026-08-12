using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Kho Tháp Cơ Bản")]
    public GameObject thapLuaPrefab;
    public GameObject thapNuocPrefab;
    public GameObject thapDatPrefab;

    [Header("Vị trí xuất hiện")]
    public Transform spawnPoint; // Chỗ tháp rớt xuống (Đảm bảo vị trí này nằm trên ô tile_placement)

    public void BuyThapLua()
    {
        SpawnTower(thapLuaPrefab);
    }

    public void BuyThapNuoc()
    {
        SpawnTower(thapNuocPrefab);
    }

    public void BuyThapDat()
    {
        SpawnTower(thapDatPrefab);
    }

    void SpawnTower(GameObject prefab)
    {
        if (prefab == null || spawnPoint == null)
        {
            Debug.LogWarning("⚠️ Chưa gán Prefab tháp hoặc SpawnPoint trong Inspector!");
            return;
        }

        // 1. Thử đặt tháp thông qua TowerPlacementManager (Kiểm tra đúng ô tile_placement & căn giữa ô)
        if (TowerPlacementManager.Instance != null)
        {
            bool success = TowerPlacementManager.Instance.TryPlaceTower(prefab, spawnPoint.position);
            
            if (!success)
            {
                Debug.Log("❌ Không thể spawn! Vị trí spawnPoint không nằm trên ô 'tile_placement' hoặc ô này đã có tháp.");
            }
        }
        else
        {
            // 2. Dự phòng: Nếu chưa có PlacementManager trên Scene thì đẻ thẳng và ép kích hoạt tháp
            GameObject newTower = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            TowerController controller = newTower.GetComponent<TowerController>();
            if (controller != null)
            {
                controller.SetOperational(true);
            }
        }
    }
}