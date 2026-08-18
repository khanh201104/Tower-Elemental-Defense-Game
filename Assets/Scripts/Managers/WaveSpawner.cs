using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DifficultyMode
{
    Manual,    // Chỉnh sửa từng Wave hoàn toàn thủ công trong Inspector
    AutoScale  // Tự động scale độ khó dựa theo dữ liệu chuẩn của Màn 1
}

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

    [Header("--- Chế Độ Cân Bằng Độ Khó ---")]
    public DifficultyMode difficultyMode = DifficultyMode.AutoScale;
    public bool autoDetectLevelFromScene = true; // Tự lấy level theo Scene Build Index (Scene 1 -> Lv1, Scene 2 -> Lv2)
    public int manualLevelNumber = 1;            // Tự gõ số Level nếu không muốn lấy theo Scene

    [Header("--- Hệ Số Tăng Tiến Tự Động (Áp dụng khi chọn AutoScale) ---")]
    [Tooltip("Tỉ lệ tăng số lượng quái mỗi màn (0.25 = tăng thêm 25% quái mỗi màn)")]
    public float enemyCountGrowthPerLevel = 0.25f;

    [Tooltip("Tỉ lệ quái ra dồn dập hơn mỗi màn (0.08 = giảm thời gian chờ đẻ quái 8% mỗi màn)")]
    public float spawnSpeedBonusPerLevel = 0.08f;

    [Tooltip("Hệ số tăng máu quái theo từng màn (0.2 = tăng thêm 20% máu mỗi màn)")]
    public float enemyHealthGrowthPerLevel = 0.2f;

    [Header("--- Cài đặt các Đợt lính (Waves) ---")]
    public Wave[] waves;              
    public Transform spawnPoint;      

    [HideInInspector] public int currentLevel = 1;
    public int currentWaveIndex = 0; 
    private bool isSpawning = false;  
    private int activeGroups = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Xác định số thứ tự màn chơi
        if (autoDetectLevelFromScene)
        {
            // Map 1.1 (Scene 1) -> Level 1; Map 1.2 (Scene 2) -> Level 2
            currentLevel = Mathf.Max(1, SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            currentLevel = Mathf.Max(1, manualLevelNumber);
        }

        // Nếu bật AutoScale và đây là từ Màn 2 trở đi -> Tự động tính toán lại thông số
        if (difficultyMode == DifficultyMode.AutoScale && currentLevel > 1)
        {
            ApplyAutoDifficultyScaling();
        }
    }

    // Tự động nâng cấp số lượng quái và tốc độ ra quái dựa trên dữ liệu gốc
    private void ApplyAutoDifficultyScaling()
    {
        int levelDiff = currentLevel - 1; // Độ lệch level so với màn 1

        float countMultiplier = 1f + (levelDiff * enemyCountGrowthPerLevel);
        float rateMultiplier = Mathf.Max(0.3f, 1f - (levelDiff * spawnSpeedBonusPerLevel));

        for (int w = 0; w < waves.Length; w++)
        {
            if (waves[w] == null || waves[w].enemyGroups == null) continue;

            for (int g = 0; g < waves[w].enemyGroups.Length; g++)
            {
                EnemyGroup group = waves[w].enemyGroups[g];
                if (group == null) continue;

                // 1. Tăng số lượng quái
                group.count = Mathf.RoundToInt(group.count * countMultiplier);

                // 2. Quái ra nhanh hơn (giảm thời gian delay)
                group.rate = Mathf.Max(0.2f, group.rate * rateMultiplier);
            }
        }

        Debug.Log($"⚙️ Đã Auto-Scale độ khó cho Level {currentLevel}: Quái x{countMultiplier:F2}, Ra nhanh x{1f / rateMultiplier:F2}");
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
            Debug.LogWarning("Đã hoàn thành toàn bộ Wave!");
            return;
        }
        
        StartCoroutine(SpawnWaveCoroutine());
    }

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
        activeGroups = currentWave.enemyGroups.Length;

        Debug.Log($"🔥 Bắt đầu Wave {currentWaveIndex + 1}/{waves.Length}!");

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

        // Chờ đẻ xong hết các nhóm
        while (activeGroups > 0)
        {
            yield return null; 
        }

        // Chờ dọn sạch quái trên sân
        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            yield return new WaitForSeconds(0.1f);
        }

        isSpawning = false;
        currentWaveIndex++; 

        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("🎉 BẠN ĐÃ CHIẾN THẮNG TOÀN BỘ GAME!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.Victory);
            }
        }
        else
        {
            Debug.Log($"✅ Đã dọn sạch Wave {currentWaveIndex}! Chuyển về trạng thái PAUSE.");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnWaveCompleted();
            }
        }
    }

    IEnumerator SpawnGroupCoroutine(EnemyGroup group)
    {
        for (int i = 0; i < group.count; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0);
            Vector3 finalSpawnPos = spawnPoint.position + randomOffset;

            GameObject enemy = Instantiate(group.enemyPrefab, finalSpawnPos, Quaternion.identity);

            // Nâng máu quái theo Level nếu chọn AutoScale
            if (difficultyMode == DifficultyMode.AutoScale && currentLevel > 1)
            {
                ApplyEnemyHealthBuff(enemy);
            }
            
            yield return new WaitForSeconds(group.rate);
        }
        
        activeGroups--; 
    }

    // Tự động tìm script máu của Quái và tăng máu theo Level
    private void ApplyEnemyHealthBuff(GameObject enemy)
    {
        float healthMultiplier = 1f + ((currentLevel - 1) * enemyHealthGrowthPerLevel);

        // Hỗ trợ nếu quái có hàm hoặc script máu
        enemy.SendMessage("ApplyDifficultyMultiplier", healthMultiplier, SendMessageOptions.DontRequireReceiver);
    }
}