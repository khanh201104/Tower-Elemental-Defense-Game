using System.Collections.Generic;
using UnityEngine;

public class MergeManager : MonoBehaviour
{
    public static MergeManager Instance;

    // TỪ ĐIỂN CÔNG THỨC: Key là "Hệ1_Hệ2", Value là "HệLai"
    private Dictionary<string, string> mergeRecipes = new Dictionary<string, string>();

    void Awake()
    {
        Instance = this;
        // Nạp công thức ngay khi game bật lên
        AddRecipe("Lua", "Dat", "NhamThach");
        AddRecipe("Lua", "Moc", "ThieuDot");
        AddRecipe("Bang", "Set", "SieuDan");
    }

    // Hàm phụ trợ giúp nạp cả 2 chiều để kéo Lửa vào Đất hay Đất vào Lửa đều nhận
    void AddRecipe(string element1, string element2, string resultElement)
    {
        mergeRecipes[element1 + "_" + element2] = resultElement;
        mergeRecipes[element2 + "_" + element1] = resultElement;
    }

    // HÀM XỬ LÝ GỘP THÁP
    public bool TryMerge(TowerDrag towerA, TowerDrag towerB)
    {
        // 1. Khác cấp độ hoặc đã Max cấp 3 -> Không cho gộp
        if (towerA.towerLevel != towerB.towerLevel || towerA.towerLevel >= 3)
            return false; 

        // 2. Nâng cấp cơ bản (Cùng hệ + Cùng cấp)
        if (towerA.elementType == towerB.elementType)
        {
            Debug.Log($"NÂNG CẤP THÀNH CÔNG: Ra tháp {towerA.elementType} cấp {towerA.towerLevel + 1}");
            ExecuteMerge(towerA, towerB); 
            return true;
        }

        // 3. Lai tạo hệ mới (Khác hệ + Cùng cấp)
        string recipeKey = towerA.elementType + "_" + towerB.elementType;
        if (mergeRecipes.ContainsKey(recipeKey))
        {
            Debug.Log($"LAI TẠO THÀNH CÔNG: Ra tháp {mergeRecipes[recipeKey]} cấp {towerA.towerLevel}");
            ExecuteMerge(towerA, towerB);
            return true;
        }

        return false; // Sai công thức -> Thất bại
    }

    void ExecuteMerge(TowerDrag a, TowerDrag b)
    {
        // Tạm thời xóa sổ (Destroy) 2 tháp cũ. 
        // Lát nữa test xong mình sẽ code sinh ra tháp mới ở đây!
        Destroy(a.gameObject);
        Destroy(b.gameObject);
    }
}