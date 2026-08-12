using UnityEngine;

public class BaseHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Khả năng Tự Hồi Máu (Regen)")]
    public bool enableRegen = false;       
    public float regenAmount = 5f;         
    public float regenInterval = 1f;       
    private float regenTimer = 0f;

    [Header("UI Máu Nhà Chính")]
    public HealthBar healthBar;

    [Header("UI Game Over")]
    public GameObject gameOverPanel; // Kéo Bảng Panel Game Over vào đây

    public static BaseHealth Instance; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }

        // Đảm bảo ẩn bảng Game Over lúc mới bắt đầu
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void Update()
    {
        HandleRegeneration();
    }

    void HandleRegeneration()
    {
        if (enableRegen && currentHealth < maxHealth)
        {
            regenTimer += Time.deltaTime;

            if (regenTimer >= regenInterval)
            {
                Heal(regenAmount);
                regenTimer = 0f;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0f, currentHealth);

        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }

        Debug.Log("🚨 BÁO ĐỘNG! Nhà chính bị cắn! Máu còn: " + currentHealth);

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);

        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }

        Debug.Log("💚 Nhà chính được HỒI MÁU! Máu hiện tại: " + currentHealth);
    }

    void GameOver()
    {
        Debug.Log("💀 GAME OVER! Nhà chính đã nổ!");

        // 1. Bật Bảng GameOver UI
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // 2. Tạm dừng thời gian trong game (Tháp ngừng bắn, quái ngừng đi)
        Time.timeScale = 0f; 
    }
}