using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Kho Tháp Cơ Bản")]
    public GameObject thapLuaPrefab;
    public GameObject thapNuocPrefab;
    public GameObject thapDatPrefab;

    [Header("Vị trí xuất hiện")]
    public Transform spawnPoint; // Chỗ tháp rớt xuống sau khi mua

    // Nếu muốn mua mất tiền thì mở comment mấy dòng GameEconomy ra
    // public int towerPrice = 15; 

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
        /* Bật đoạn này lên nếu muốn trừ tiền thật
        if (GameEconomy.Instance.currentGold >= towerPrice)
        {
            GameEconomy.Instance.SpendGold(towerPrice);
            Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("Không đủ tiền!");
        }
        */

        // Tạm thời cứ bấm nút là đẻ tháp free để test cho lẹ
        Instantiate(prefab, spawnPoint.position, Quaternion.identity);
    }
}