using System.Collections.Generic;
using UnityEngine;

public class MergeManager : MonoBehaviour
{
    public static MergeManager Instance;

    private Dictionary<string, string> mergeRecipes = new Dictionary<string, string>();

    [Header("Kho Tháp Cơ Bản")]
    public GameObject luaLv2; public GameObject luaLv3;
    public GameObject nuocLv2; public GameObject nuocLv3;
    public GameObject datLv2; public GameObject datLv3;

    [Header("Kho Tháp Lai - Cấp 1")]
    public GameObject thapBomNhietPrefab;
    public GameObject thapDungNhamPrefab;
    public GameObject thapDamLayPrefab;

    [Header("Kho Tháp Lai - Cấp 2")]
    public GameObject bomNhietLv2;
    public GameObject dungNhamLv2;
    public GameObject damLayLv2;

    [Header("Kho Tháp Lai - Cấp 3")]
    public GameObject bomNhietLv3;
    public GameObject dungNhamLv3;
    public GameObject damLayLv3;

    void Awake()
    {
        Instance = this;
        
        AddRecipe("Lua", "Nuoc", "BomNhiet"); 
        AddRecipe("Lua", "Dat", "DungNham");   
        AddRecipe("Dat", "Nuoc", "DamLay");    
    }

    void AddRecipe(string element1, string element2, string resultElement)
    {
        mergeRecipes[element1 + "_" + element2] = resultElement;
        mergeRecipes[element2 + "_" + element1] = resultElement;
    }

    public bool TryMerge(TowerDrag towerA, TowerDrag towerB)
    {
        if (towerA.towerLevel != towerB.towerLevel)
            return false; 

        // Lấy vị trí của tháp bị đè (towerB) làm vị trí sinh tháp mới
        Vector3 spawnPos = towerB.transform.position;
        string elemA = towerA.elementType;
        string elemB = towerB.elementType;
        
        // KIỂM TRA: Vị trí gộp đang ở trên Hàng chờ hay Sân đấu
        bool isOnBench = false;
        if (BenchManager.Instance != null)
        {
            isOnBench = (BenchManager.Instance.GetNearestSlotIndex(spawnPos) != -1);
        }

        // 1. NÂNG CẤP LEVEL (Ghép 2 tháp giống hệt nhau)
        if (elemA == elemB)
        {
            if (towerA.towerLevel >= 3)
            {
                Debug.Log("Tháp đã Max Level 3, không thể nâng cấp thêm!");
                return false; 
            }

            int nextLevel = towerA.towerLevel + 1;
            ExecuteMerge(towerA, towerB); 
            SpawnUpgradedTower(elemA, nextLevel, spawnPos, isOnBench);
            return true;
        }

        // 2. LAI TẠO THÁP (Ghép 2 tháp khác hệ)
        string recipeKey = elemA + "_" + elemB;
        if (mergeRecipes.ContainsKey(recipeKey))
        {
            string heLai = mergeRecipes[recipeKey];
            ExecuteMerge(towerA, towerB);
            SpawnHybridTower(heLai, towerA.towerLevel, spawnPos, isOnBench);
            return true;
        }

        return false; 
    }

    void ExecuteMerge(TowerDrag a, TowerDrag b)
    {
        // Giải phóng ô đăng ký trên Hàng chờ (nếu có tháp nằm ở Hàng chờ)
        if (BenchManager.Instance != null)
        {
            BenchManager.Instance.RemoveTowerFromBench(a.gameObject);
            BenchManager.Instance.RemoveTowerFromBench(b.gameObject);
        }

        // GIẢI PHÓNG ĐÚNG Ô TILE BAN ĐẦU CỦA 2 THÁP CỦ
        if (TowerPlacementManager.Instance != null)
        {
            TowerPlacementManager.Instance.ClearTile(a.originalPosition);
            TowerPlacementManager.Instance.ClearTile(b.originalPosition);
        }

        Destroy(a.gameObject);
        Destroy(b.gameObject);
    }

    // Xử lý đẻ tháp nâng cấp
    void SpawnUpgradedTower(string element, int level, Vector3 pos, bool isOnBench)
    {
        GameObject prefabToSpawn = null;

        if (level == 2)
        {
            if (element == "Lua") prefabToSpawn = luaLv2;
            else if (element == "Nuoc") prefabToSpawn = nuocLv2;
            else if (element == "Dat") prefabToSpawn = datLv2;
            else if (element == "BomNhiet") prefabToSpawn = bomNhietLv2;
            else if (element == "DungNham") prefabToSpawn = dungNhamLv2;
            else if (element == "DamLay") prefabToSpawn = damLayLv2;
        }
        else if (level == 3)
        {
            if (element == "Lua") prefabToSpawn = luaLv3;
            else if (element == "Nuoc") prefabToSpawn = nuocLv3;
            else if (element == "Dat") prefabToSpawn = datLv3;
            else if (element == "BomNhiet") prefabToSpawn = bomNhietLv3;
            else if (element == "DungNham") prefabToSpawn = dungNhamLv3;
            else if (element == "DamLay") prefabToSpawn = damLayLv3;
        }

        SpawnTowerWithPlacementManager(prefabToSpawn, pos, isOnBench, $"Nâng cấp {element} Cấp {level}");
    }

    // Xử lý đẻ tháp lai
    void SpawnHybridTower(string resultElement, int level, Vector3 pos, bool isOnBench)
    {
        GameObject prefabToSpawn = null;

        if (level == 1)
        {
            if (resultElement == "BomNhiet") prefabToSpawn = thapBomNhietPrefab;
            else if (resultElement == "DungNham") prefabToSpawn = thapDungNhamPrefab;
            else if (resultElement == "DamLay") prefabToSpawn = thapDamLayPrefab;
        }
        else if (level == 2)
        {
            if (resultElement == "BomNhiet") prefabToSpawn = bomNhietLv2;
            else if (resultElement == "DungNham") prefabToSpawn = dungNhamLv2;
            else if (resultElement == "DamLay") prefabToSpawn = damLayLv2;
        }
        else if (level == 3)
        {
            if (resultElement == "BomNhiet") prefabToSpawn = bomNhietLv3;
            else if (resultElement == "DungNham") prefabToSpawn = dungNhamLv3;
            else if (resultElement == "DamLay") prefabToSpawn = damLayLv3;
        }

        SpawnTowerWithPlacementManager(prefabToSpawn, pos, isOnBench, $"Lai tạo {resultElement} Cấp {level}");
    }

    // HÀM BỔ SUNG: Quản lý trạng thái ACTIVE / INACTIVE dựa vào vị trí sinh tháp
    void SpawnTowerWithPlacementManager(GameObject prefab, Vector3 pos, bool isOnBench, string debugText)
    {
        if (prefab == null) return;

        if (isOnBench)
        {
            // A. NẾU GỘP TRÊN HÀNG CHỜ -> Ép về trạng thái INACTIVE (Mờ 50%, dừng bắn)
            GameObject newTower = Instantiate(prefab, pos, Quaternion.identity);

            // Căn thẳng vào tâm của ô Bench gần nhất
            if (BenchManager.Instance != null)
            {
                int slotIdx = BenchManager.Instance.GetNearestSlotIndex(pos);
                if (slotIdx != -1)
                {
                    newTower.transform.position = BenchManager.Instance.benchSlots[slotIdx].position;
                }
            }

            TowerController controller = newTower.GetComponent<TowerController>();
            if (controller != null)
            {
                controller.SetOperational(false); // Inactive
            }

            Debug.Log($"✅ Đã {debugText} thành công trên HÀNG CHỜ (Trạng thái: Inactive)!");
        }
        else
        {
            // B. NẾU GỘP TRÊN SÂN ĐẤU -> Bật ACTIVE (Sáng 100%, bắt đầu bắn)
            if (TowerPlacementManager.Instance != null)
            {
                TowerPlacementManager.Instance.SpawnMergedTower(prefab, pos);
            }
            else
            {
                GameObject newTower = Instantiate(prefab, pos, Quaternion.identity);
                TowerController controller = newTower.GetComponent<TowerController>();
                if (controller != null)
                {
                    controller.SetOperational(true); // Active
                }
            }

            Debug.Log($"✅ Đã {debugText} thành công trên SÂN ĐẤU (Trạng thái: Active)!");
        }
    }
}