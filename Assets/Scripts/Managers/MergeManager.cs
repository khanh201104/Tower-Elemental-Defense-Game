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
        // Bỏ điều kiện chặn level 3 ở đây đi, chỉ cần check xem có cùng level không thôi
        if (towerA.towerLevel != towerB.towerLevel)
            return false; 

        Vector3 spawnPos = towerA.transform.position;
        string elemA = towerA.elementType;
        string elemB = towerB.elementType;
        
        // 1. NÂNG CẤP LEVEL (Ghép 2 tháp giống hệt nhau)
        if (elemA == elemB)
        {
            // Chốt chặn Max Level đặt ở đây: Tháp giống nhau thì Cấp 3 là kịch kim
            if (towerA.towerLevel >= 3)
            {
                Debug.Log("Tháp đã Max Level 3, không thể nâng cấp thêm!");
                return false; 
            }

            int nextLevel = towerA.towerLevel + 1;
            ExecuteMerge(towerA, towerB); 
            SpawnUpgradedTower(elemA, nextLevel, spawnPos);
            return true;
        }

        // 2. LAI TẠO THÁP (Ghép 2 tháp khác hệ)
        string recipeKey = elemA + "_" + elemB;
        if (mergeRecipes.ContainsKey(recipeKey))
        {
            // Lai tạo thì cấp nào cũng chơi, truyền đúng level nguyên liệu sang tháp lai
            string heLai = mergeRecipes[recipeKey];
            ExecuteMerge(towerA, towerB);
            SpawnHybridTower(heLai, towerA.towerLevel, spawnPos);
            return true;
        }

        return false; 
    }

    void ExecuteMerge(TowerDrag a, TowerDrag b)
    {
        Destroy(a.gameObject);
        Destroy(b.gameObject);
    }

    // Xử lý việc đẻ tháp khi người chơi ghép 2 tháp cùng loại (Lửa1+Lửa1, hoặc BomNhiet1+BomNhiet1)
    void SpawnUpgradedTower(string element, int level, Vector3 pos)
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

        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, pos, Quaternion.identity);
            Debug.Log($"Đã nâng cấp {element} lên Cấp {level}");
        }
    }

    // Xử lý việc đẻ tháp lai khi người chơi ghép 2 tháp cơ bản (Lửa2+Nước2 = BomNhiet2)
    void SpawnHybridTower(string resultElement, int level, Vector3 pos)
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

        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, pos, Quaternion.identity);
            Debug.Log($"Đã lai tạo ra {resultElement} Cấp {level}");
        }
    }
}