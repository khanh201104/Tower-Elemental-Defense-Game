using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DifficultyMode
{
    Manual,    // Chỉnh sửa từng Wave hoàn toàn thủ công trong Inspector
    AutoScale  // Tự động scale độ khó dựa theo file Dữ liệu chuẩn
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
    
    [Tooltip("Kéo file BaseWaveData (Dữ liệu gốc) vào đây nếu dùng AutoScale")]
    public BaseWaveData defaultWaveData; 

    public bool autoDetectLevelFromScene = true; 
    public int manualLevelNumber = 1;            

    [Header("--- Hệ Số Tăng Tiến Tự Động (AutoScale) ---")]
    public float enemyCountGrowthPerLevel = 0.25f;
    public float spawnSpeedBonusPerLevel = 0.08f;
    public float enemyHealthGrowthPerLevel = 0.2f;

    [Header("--- Cài đặt các Đợt lính (Dùng cho Manual) ---")]
    [Tooltip("Nếu chọn Manual, hãy chỉnh sửa danh sách này. Nếu chọn AutoScale, danh sách này sẽ tự động được ghi đè bằng dữ liệu tính toán từ BaseWaveData.")]
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
            currentLevel = Mathf.Max(1, SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            currentLevel = Mathf.Max(1, manualLevelNumber);
        }

        // Tự động nạp và scale dữ liệu nếu dùng AutoScale
        if (difficultyMode == DifficultyMode.AutoScale)
        {
            ApplyAutoDifficultyScaling();
        }
    }

    // Đọc từ file gốc, Copy ra bản mới và áp dụng hệ số Scale
    private void ApplyAutoDifficultyScaling()
    {
        if (defaultWaveData == null || defaultWaveData.defaultWaves == null)
        {
            Debug.LogError("LỖI: Bạn chọn AutoScale nhưng chưa kéo file Default Wave Data vào WaveSpawner!");
            return;
        }

        int levelDiff = currentLevel - 1; 
        float countMultiplier = 1f + (levelDiff * enemyCountGrowthPerLevel);
        float rateMultiplier = Mathf.Max(0.3f, 1f - (levelDiff * spawnSpeedBonusPerLevel));

        // Khởi tạo lại mảng waves cục bộ bằng đúng số lượng wave trong file gốc
        waves = new Wave[defaultWaveData.defaultWaves.Length];

        for (int w = 0; w < defaultWaveData.defaultWaves.Length; w++)
        {
            waves[w] = new Wave();
            
            int groupLength = defaultWaveData.defaultWaves[w].enemyGroups.Length;
            waves[w].enemyGroups = new EnemyGroup[groupLength];

            for (int g = 0; g < groupLength; g++)
            {
                EnemyGroup originalGroup = defaultWaveData.defaultWaves[w].enemyGroups[g];
                
                // Deep Copy (Tạo mới hoàn toàn) để không làm thay đổi file dữ liệu gốc
                EnemyGroup newGroup = new EnemyGroup
                {
                    enemyPrefab = originalGroup.enemyPrefab,
                    count = Mathf.RoundToInt(originalGroup.count * countMultiplier),
                    rate = Mathf.Max(0.2f, originalGroup.rate * rateMultiplier)
                };

                waves[w].enemyGroups[g] = newGroup;
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

        while (activeGroups > 0)
        {
            yield return null; 
        }

        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            yield return new WaitForSeconds(0.1f);
        }

        isSpawning = false;
        currentWaveIndex++; 

        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("🎉 BẠN ĐÃ CHIẾN THẮNG TOÀN BỘ CÁC WAVE!");
            if (GameManager.Instance != null)
            {
                // SỬA TẠI ĐÂY: Kích hoạt coroutine delay 1s trước khi bật VictoryPanel
                GameManager.Instance.TriggerVictory();
            }
        }
        else
        {
            Debug.Log($"✅ Đã dọn sạch Wave {currentWaveIndex}! Chuyển về trạng thái chuẩn bị.");
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

    private void ApplyEnemyHealthBuff(GameObject enemy)
    {
        float healthMultiplier = 1f + ((currentLevel - 1) * enemyHealthGrowthPerLevel);
        enemy.SendMessage("ApplyDifficultyMultiplier", healthMultiplier, SendMessageOptions.DontRequireReceiver);
    }
}