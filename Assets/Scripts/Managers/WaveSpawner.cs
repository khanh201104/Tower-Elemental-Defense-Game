using System.Collections;
using System.Collections.Generic; 
using UnityEngine;

[System.Serializable] 
public class EnemyGroup
{
    public GameObject enemyPrefab;    
    public int count;                 
    public float rate;                
}

[System.Serializable]
public class Wave
{
    public EnemyGroup[] enemyGroups;  
}

public class WaveSpawner : MonoBehaviour
{
    [Header("Cài đặt các Đợt lính (Waves)")]
    public Wave[] waves;              
    public Transform spawnPoint;      

    private int currentWaveIndex = 0; 
    private bool isSpawning = false;  
    
    // Biến đếm xem đang có bao nhiêu nhóm quái đang đẻ
    private int activeGroups = 0; 

    void Start()
    {
        if (waves.Length > 0)
        {
            StartCoroutine(SpawnWaveCoroutine());
        }
    }

    public void CallNextWave()
    {
        if (isSpawning)
        {
            Debug.LogWarning("Quái đang ra rồi, từ từ hẵng bấm!");
            return;
        }

        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("Đã hết tất cả các đợt quái. Chờ Win game!");
            return;
        }
        
        StartCoroutine(SpawnWaveCoroutine());
    }

    IEnumerator SpawnWaveCoroutine()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("LỖI: Chưa kéo điểm xuất phát (Spawn Point) vào WaveSpawner!");
            yield break;
        }

        isSpawning = true;
        Wave currentWave = waves[currentWaveIndex];
        
        // Cập nhật số lượng nhóm quái sẽ xuất hiện trong đợt này
        activeGroups = currentWave.enemyGroups.Length;

        // Bắn tín hiệu cho TẤT CẢ các nhóm cùng đẻ MỘT LÚC
        foreach (EnemyGroup group in currentWave.enemyGroups)
        {
            if (group.enemyPrefab == null)
            {
                Debug.LogError("LỖI: Có nhóm quái trống Prefab!");
                activeGroups--; // Trừ đi vì nhóm này bị lỗi không chạy
                continue; 
            }

            // Gọi hàm đẻ quái chạy song song cho từng nhóm
            StartCoroutine(SpawnGroupCoroutine(group));
        }

        // Chờ đến khi TẤT CẢ các nhóm đều đẻ xong (biến activeGroups đếm ngược về 0)
        while (activeGroups > 0)
        {
            yield return null; // Dừng lại 1 frame rồi check tiếp
        }

        // Đẻ xong sạch sẽ mọi loại quái
        isSpawning = false;
        currentWaveIndex++; 
    }

    // Hàm đẻ quái riêng biệt cho từng nhóm (chạy song song)
    IEnumerator SpawnGroupCoroutine(EnemyGroup group)
    {
        for (int i = 0; i < group.count; i++)
        {
            // Tạo một độ lệch ngẫu nhiên nhỏ xíu để quái không bị đè dính chặt lên nhau
            Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0);
            Vector3 finalSpawnPos = spawnPoint.position + randomOffset;

            Instantiate(group.enemyPrefab, finalSpawnPos, Quaternion.identity);
            
            // Chờ thời gian nghỉ của riêng loại quái này
            yield return new WaitForSeconds(group.rate);
        }
        
        // Nhóm này đẻ xong rồi -> Trừ bộ đếm đi 1
        activeGroups--; 
    }
}