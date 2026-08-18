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
    public static WaveSpawner Instance;

    [Header("Cài đặt các Đợt lính (Waves)")]
    public Wave[] waves;              
    public Transform spawnPoint;      

    public int currentWaveIndex = 0; 
    private bool isSpawning = false;  
    
    // Biến đếm xem đang có bao nhiêu nhóm quái đang đẻ
    private int activeGroups = 0; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Bắt đầu game ở trạng thái PAUSE (Chuẩn bị)
    }

    public void CallNextWave()
    {
        if (isSpawning)
        {
            Debug.LogWarning("Quái đang ra rồi, từ từ hẵng bấm!");
            return;
        }

        // Nếu đã hoàn thành tất cả Wave thì không cho bấm thêm
        if (currentWaveIndex >= waves.Length)
        {
            Debug.LogWarning("Đã hoàn thành toàn bộ Wave!");
            return;
        }
        
        StartCoroutine(SpawnWaveCoroutine());
    }

    // Biệt danh (alias) tương thích với GameManager
    public void StartCurrentWave()
    {
        CallNextWave();
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
        
        // Cập nhật số lượng nhóm quái xuất hiện trong đợt này
        activeGroups = currentWave.enemyGroups.Length;

        Debug.Log($"🔥 Bắt đầu Wave {currentWaveIndex + 1}/{waves.Length}!");

        // Bắn tín hiệu cho TẤT CẢ các nhóm cùng đẻ MỘT LÚC
        foreach (EnemyGroup group in currentWave.enemyGroups)
        {
            if (group.enemyPrefab == null)
            {
                Debug.LogError("LỖI: Có nhóm quái trống Prefab!");
                activeGroups--; 
                continue; 
            }

            StartCoroutine(SpawnGroupCoroutine(group));
        }

        // 1. Chờ TẤT CẢ các nhóm đẻ xong
        while (activeGroups > 0)
        {
            yield return null; 
        }

        // 2. Chờ cho đến khi TẤT CẢ quái trên sân (Tag "Enemy") bị tiêu diệt
        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            yield return new WaitForSeconds(0.1f); // Check mỗi 0.1s để phản hồi Victory nhạy hơn
        }

        // Đã đẻ xong và dọn sạch quái trên sân
        isSpawning = false;
        currentWaveIndex++; 

        // -------------------------------------------------------------
        // [CẬP NHẬT MỚI] XỬ LÝ KẾT THÚC WAVE / CHIẾN THẮNG
        // -------------------------------------------------------------
        if (currentWaveIndex >= waves.Length)
        {
            // Nếu đây là Wave cuối cùng -> BẬT VICTORY NGAY LẬP TỨC!
            Debug.Log("🎉 BẠN ĐÃ CHIẾN THẮNG TOÀN BỘ GAME!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.Victory);
            }
        }
        else
        {
            // Nếu chưa phải Wave cuối -> Chuyển về PAUSE để người chơi chuẩn bị
            Debug.Log($"✅ Đã dọn sạch Wave {currentWaveIndex}! Chuyển về trạng thái PAUSE.");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.Pause);
            }
        }
    }

    // Hàm đẻ quái song song cho từng nhóm
    IEnumerator SpawnGroupCoroutine(EnemyGroup group)
    {
        for (int i = 0; i < group.count; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0);
            Vector3 finalSpawnPos = spawnPoint.position + randomOffset;

            Instantiate(group.enemyPrefab, finalSpawnPos, Quaternion.identity);
            
            yield return new WaitForSeconds(group.rate);
        }
        
        activeGroups--; 
    }
}