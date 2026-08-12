using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerPlacementManager : MonoBehaviour
{
    public static TowerPlacementManager Instance;

    [Header("Cấu hình Tilemap")]
    public Tilemap mainTilemap;                  
    public string placementTileName = "tile_placement"; 

    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool IsValidPlacement(Vector3 worldPosition)
    {
        if (mainTilemap == null) return false;

        Vector3Int cellPosition = mainTilemap.WorldToCell(worldPosition);
        TileBase currentTile = mainTilemap.GetTile(cellPosition);

        if (currentTile == null || currentTile.name != placementTileName)
        {
            Debug.Log("❌ Vị trí không hợp lệ! Ô này không phải ô: " + placementTileName);
            return false;
        }

        if (occupiedCells.Contains(cellPosition))
        {
            Debug.Log("❌ Vị trí không hợp lệ! Ô này đã có tháp đứng rồi.");
            return false;
        }

        return true;
    }

    public bool TryPlaceTower(GameObject towerPrefab, Vector3 worldPosition)
    {
        if (!IsValidPlacement(worldPosition)) return false;

        Vector3Int cellPosition = mainTilemap.WorldToCell(worldPosition);
        Vector3 cellCenterPos = mainTilemap.GetCellCenterWorld(cellPosition);

        GameObject newTower = Instantiate(towerPrefab, cellCenterPos, Quaternion.identity);
        occupiedCells.Add(cellPosition);

        TowerController controller = newTower.GetComponent<TowerController>();
        if (controller != null)
        {
            controller.SetOperational(true);
        }

        return true;
    }

    // === HÀM MỚI 1: XỬ LÝ DI CHUYỂN THÁP (Dời từ ô A sang ô B) ===
    public bool TryMoveTower(GameObject tower, Vector3 oldWorldPos, Vector3 newWorldPos)
    {
        if (mainTilemap == null) return false;

        Vector3Int oldCell = mainTilemap.WorldToCell(oldWorldPos);
        Vector3Int newCell = mainTilemap.WorldToCell(newWorldPos);

        // Nếu kéo thả vào cùng 1 ô cũ thì cho qua
        if (oldCell == newCell)
        {
            tower.transform.position = mainTilemap.GetCellCenterWorld(oldCell);
            return true;
        }

        // Kiểm tra ô mới có đặt được không
        if (!IsValidPlacement(newWorldPos)) return false;

        // XÓA ĐĂNG KÝ Ô CŨ & ĐĂNG KÝ Ô MỚI
        occupiedCells.Remove(oldCell);
        occupiedCells.Add(newCell);

        // Căn tháp vào chính giữa ô mới
        tower.transform.position = mainTilemap.GetCellCenterWorld(newCell);
        return true;
    }

    // === HÀM MỚI 2: XỬ LÝ SPAWN THÁP GỘP (Merge) ===
    public GameObject SpawnMergedTower(GameObject newTowerPrefab, Vector3 worldPosition)
    {
        if (mainTilemap == null || newTowerPrefab == null) return null;

        Vector3Int cellPosition = mainTilemap.WorldToCell(worldPosition);
        Vector3 cellCenterPos = mainTilemap.GetCellCenterWorld(cellPosition);

        // Sinh tháp cấp cao mới
        GameObject newTower = Instantiate(newTowerPrefab, cellCenterPos, Quaternion.identity);
        
        // Đánh dấu ô này bận
        occupiedCells.Add(cellPosition);

        // ÉP BẬT HOẠT ĐỘNG NGAY LẬP TỨC
        TowerController controller = newTower.GetComponent<TowerController>();
        if (controller != null)
        {
            controller.SetOperational(true);
        }

        return newTower;
    }

    public void ClearTile(Vector3 worldPosition)
    {
        if (mainTilemap == null) return;

        Vector3Int cellPosition = mainTilemap.WorldToCell(worldPosition);

        if (occupiedCells.Contains(cellPosition))
        {
            occupiedCells.Remove(cellPosition);
            Debug.Log("🧹 Đã giải phóng ô Tile tại tọa độ: " + cellPosition);
        }
    }
}