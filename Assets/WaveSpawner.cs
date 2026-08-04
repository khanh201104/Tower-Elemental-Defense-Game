using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Cài đặt đợt lính")]
    public GameObject enemyPrefab;    // Quái gì sẽ được đẻ?
    public Transform spawnPoint;      // Đẻ ở đâu?
    public int enemiesPerWave = 5;    // Số lượng quái mỗi đợt
    public float timeBetweenSpawns = 1.5f; // Thời gian nghỉ giữa 2 con quái

    void Start()
    {
        // Bắt đầu nhả đợt quái đầu tiên ngay khi vào game
        StartCoroutine(SpawnWave()); 
    }

    // IEnumerator (Coroutine) là một kỹ thuật đặc biệt để tạo ra sự trì hoãn (chờ 1.5s rồi đẻ tiếp)
    IEnumerator SpawnWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            // Sinh ra một con quái tại vị trí đẻ
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            
            // Chờ một lúc rồi mới quay lại vòng lặp đẻ con tiếp theo
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}