using UnityEngine;

public class BaseHealth : MonoBehaviour
{
    public static BaseHealth Instance;

    [Header("Chỉ số Máu Nhà Chính")]
    public float maxHealth = 100f;
    private float currentHealth;

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
        currentHealth = maxHealth;
        UpdateHealthUI();
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

        UpdateHealthUI();

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

        UpdateHealthUI();

        Debug.Log("💚 Nhà chính được HỒI MÁU! Máu hiện tại: " + currentHealth);
    }

    private void UpdateHealthUI()
    {
        // Tự động gửi thông số máu sang Canvas Prefab để cập nhật Slider & Text
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
            GameManager.Instance.SetState(GameState.GameOver);
        }
    }
}