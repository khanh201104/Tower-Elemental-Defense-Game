using UnityEngine;

public class TowerHealth : MonoBehaviour
{
    public float maxHealth = 50f;
    private float currentHealth;

    [Header("Khả năng Tự Hồi Máu (Regen)")]
    public bool enableRegen = false;       // Tích chọn nếu tháp này có khả năng tự hồi máu
    public float regenAmount = 2f;         // Số máu được hồi mỗi đợt
    public float regenInterval = 1f;       // Cứ mỗi bao nhiêu giây thì hồi 1 lần
    private float regenTimer = 0f;

    [Header("UI Máu Tự Động")]
    public Vector3 hpBarOffset = new Vector3(0, 0.65f, 0); // Đã để chuẩn 0.65
    public bool hideWhenFull = true;
    
    [HideInInspector]
    public HealthBar healthBar;

    private static GameObject cachedHpBarPrefab;

    void Start()
    {
        currentHealth = maxHealth;

        if (cachedHpBarPrefab == null)
        {
            cachedHpBarPrefab = Resources.Load<GameObject>("HealthBarCanvas");
        }

        if (cachedHpBarPrefab != null)
        {
            GameObject hpBarInstance = Instantiate(cachedHpBarPrefab, transform);
            hpBarInstance.transform.localPosition = hpBarOffset;
            hpBarInstance.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

            healthBar = hpBarInstance.GetComponentInChildren<HealthBar>();
        }

        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
            if (hideWhenFull) healthBar.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        HandleRegeneration();
    }

    // Hàm đếm ngược và xử lý hồi máu thụ động
    void HandleRegeneration()
    {
        if (enableRegen && currentHealth < maxHealth)
        {
            regenTimer += Time.deltaTime;

            if (regenTimer >= regenInterval)
            {
                Heal(regenAmount);
                regenTimer = 0f; // Reset bộ đếm thời gian
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0f, currentHealth);

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }

        Debug.Log(gameObject.name + " bị vã! Máu còn: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth >= maxHealth) 
        {
            currentHealth = maxHealth;
            if (healthBar != null && hideWhenFull) 
            {
                healthBar.gameObject.SetActive(false); // Đầy máu -> Tự ẩn thanh máu
            }
        }

        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }

        Debug.Log("💚 " + gameObject.name + " được HỒI MÁU! Máu hiện tại: " + currentHealth);
    }

    public void HealToFull()
    {
        Heal(maxHealth); // Tận dụng lại hàm Heal() sẵn có để cập nhật UI & tự ẩn HealthBar
    }

    void Die()
{
    Debug.Log("💥 " + gameObject.name + " đã sập!");

    // Giải phóng ô Tilemap tại vị trí tháp đứng
    if (TowerPlacementManager.Instance != null)
    {
        TowerPlacementManager.Instance.ClearTile(transform.position);
    }

    Destroy(gameObject);
}
}