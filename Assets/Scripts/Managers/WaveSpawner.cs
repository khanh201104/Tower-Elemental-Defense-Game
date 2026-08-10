using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Cài đặt đợt lính")]
    public GameObject enemyPrefab;    // Quái gì sẽ được đẻ?
    public Transform spawnPoint;      // Đẻ ở đâu?
    public int enemiesPerWave = 5;    // Số lượng quái mỗi đợt
    public float timeBetweenSpawns = 1.5f; // Thời gian nghỉ giữa 2 con quái

    private bool isSpawning = false;  // Biến khóa để chặn spam click nút

    void Start()
    {
        // Tự động nhả đợt đầu tiên lúc mới vào game
        StartCoroutine(SpawnWaveCoroutine()); 
    }

    // Nút Bấm ngoài UI sẽ gọi vào hàm này
    public void CallNextWave()
    {
        // Nếu đang đẻ quái rồi thì không cho đẻ thêm để tránh lỗi đè lên nhau
        if (isSpawning)
        {
            Debug.LogWarning("Quái đang ra rồi, từ từ hẵng bấm!");
            return;
        }
        
        StartCoroutine(SpawnWaveCoroutine());
    }

    IEnumerator SpawnWaveCoroutine()
    {
        // --- HỆ THỐNG CHECK LỖI TRƯỚC KHI CHẠY ---
        if (enemyPrefab == null)
        {
            Debug.LogError("LỖI: Chưa kéo con Quái (Enemy Prefab) vào slot của WaveSpawner!");
            yield break; // Ngừng luôn, không chạy code bên dưới nữa
        }
        if (spawnPoint == null)
        {
            Debug.LogError("LỖI: Chưa kéo điểm xuất phát (Spawn Point) vào slot của WaveSpawner!");
            yield break;
        }
        // ------------------------------------------

        isSpawning = true; // Khóa lại không cho bấm nữa
        Debug.Log("Bắt đầu đẻ " + enemiesPerWave + " con quái!");

        for (int i = 0; i < enemiesPerWave; i++)
        {
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            
            // Chờ một khoảng thời gian (1.5s) rồi đẻ con tiếp theo
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        isSpawning = false; // Đẻ xong hết rồi thì mở khóa ra cho nút bấm hoạt động lại
    }
}