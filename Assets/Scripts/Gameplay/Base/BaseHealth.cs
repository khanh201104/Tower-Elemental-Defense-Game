using UnityEngine;

public class BaseHealth : MonoBehaviour
{
    public static BaseHealth Instance;

    [Header("Chỉ số Máu Nhà Chính")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false; // Cờ chặn gọi GameOver nhiều lần khi quái đánh dồn dập

    [Header("Khả năng Tự Hồi Máu (Regen)")]
    public bool enableRegen = false;       
    public float regenAmount = 5f;         
    public float regenInterval = 1f;       
    private float regenTimer = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        isDead = false;
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        HandleRegeneration();
    }

    void HandleRegeneration()
    {
        if (!isDead && enableRegen && currentHealth < maxHealth)
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
        if (isDead) return; // Bỏ qua sát thương nếu nhà chính đã nổ

        currentHealth -= amount;
        currentHealth = Mathf.Max(0f, currentHealth);

        UpdateHealthUI();

        Debug.Log("🚨 BÁO ĐỘNG! Nhà chính bị cắn! Máu còn: " + currentHealth);

        if (currentHealth <= 0f)
        {
            isDead = true;
            GameOver();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);

        UpdateHealthUI();

        Debug.Log("💚 Nhà chính được HỒI MÁU! Máu hiện tại: " + currentHealth);
    }

    private void UpdateHealthUI()
    {
        if (GameplayCanvasController.Instance != null)
        {
            GameplayCanvasController.Instance.UpdateBaseHealthDisplay(currentHealth, maxHealth);
        }
    }

    void GameOver()
    {
        Debug.Log("💀 GAME OVER! Nhà chính đã nổ!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameOver();
        }
    }
}