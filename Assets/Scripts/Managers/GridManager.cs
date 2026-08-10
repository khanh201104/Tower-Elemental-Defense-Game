using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    void Awake() 
    {
        Instance = this;
    }

    // Trả về true hết để test kéo thả cho trơn tru
    public bool IsValidCell(int x, int y) { return true; }
    public bool IsCellEmpty(int x, int y) { return true; }
    public void OccupyCell(int x, int y, TowerDrag tower) { }
    public TowerDrag GetTowerAt(int x, int y) { return null; }
}