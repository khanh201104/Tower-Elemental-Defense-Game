using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int hp = 3;
    public int goldReward = 15;

    [Header("VFX Hiệu Ứng (Kéo Object con vào đây)")]
    public GameObject fireVFX; 
    public GameObject slowVFX;

    [Header("Màu sắc khi dính trạng thái")]
    public Color burnColor = new Color(1f, 0.4f, 0.2f);        
    public Color slowColor = new Color(0.4f, 0.8f, 1f);        
    public Color combinedColor = new Color(0.8f, 0.3f, 0.9f);  

    private SpriteRenderer spriteRenderer;
    private EnemyMovement enemyMovement;
    private EnemyAnimation enemyAnimation;
    private Collider2D enemyCollider;
    private bool isDead = false;

    [Header("Hiệu ứng Đốt Máu")]
    private float burnTimer = 0f;
    private float burnTickTimer = 0f;
    private int burnDamage = 0;
    private float burnTickInterval = 1f;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAnimation = GetComponent<EnemyAnimation>();
        enemyCollider = GetComponent<Collider2D>();

        if (fireVFX != null) fireVFX.SetActive(false);
        if (slowVFX != null) slowVFX.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

        if (burnTimer > 0)
        {
            burnTimer -= Time.deltaTime;
            burnTickTimer -= Time.deltaTime;

            if (burnTickTimer <= 0)
            {
                TakeDamage(burnDamage);
                burnTickTimer = burnTickInterval;
            }
        }

        UpdateStatusVisuals();
    }

    void UpdateStatusVisuals()
    {
        if (isDead) return;

        bool isBurned = burnTimer > 0;
        bool isSlowed = (enemyMovement != null && enemyMovement.IsSlowed);

        if (isBurned && isSlowed)
        {
            if (fireVFX != null) fireVFX.SetActive(true);
            if (slowVFX != null) slowVFX.SetActive(true);
            if (spriteRenderer != null) spriteRenderer.color = combinedColor;
        }
        else if (isBurned)
        {
            if (fireVFX != null) fireVFX.SetActive(true);
            if (slowVFX != null) slowVFX.SetActive(false);
            if (spriteRenderer != null) spriteRenderer.color = burnColor;
        }
        else if (isSlowed)
        {
            if (fireVFX != null) fireVFX.SetActive(false);
            if (slowVFX != null) slowVFX.SetActive(true);
            if (spriteRenderer != null) spriteRenderer.color = slowColor;
        }
        else
        {
            if (fireVFX != null) fireVFX.SetActive(false);
            if (slowVFX != null) slowVFX.SetActive(false);
            if (spriteRenderer != null) spriteRenderer.color = Color.white;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // 1. Cộng vàng ngay lập tức
        if (GameEconomy.Instance != null)
        {
            GameEconomy.Instance.AddGold(goldReward);
        }

        // 2. Tắt va chạm và dừng di chuyển
        if (enemyCollider != null) enemyCollider.enabled = false;
        if (enemyMovement != null) enemyMovement.enabled = false;
        if (fireVFX != null) fireVFX.SetActive(false);
        if (slowVFX != null) slowVFX.SetActive(false);
        if (spriteRenderer != null) spriteRenderer.color = Color.white;

        // 3. Đổi Tag để WaveSpawner biết quái này đã bị tiêu diệt
        gameObject.tag = "Untagged";

        // 4. Kích hoạt animation Chết
        if (enemyAnimation != null)
        {
            enemyAnimation.PlayDieAnimation();
        }

        // 5. Hủy GameObject sau 0.6 giây để chạy xong animation chết
        Destroy(gameObject, 0.6f);
    }

    public void ApplyBurn(int damagePerTick, float duration, float interval)
    {
        if (isDead) return;
        burnDamage = Mathf.Max(burnDamage, damagePerTick);
        burnTimer = duration; 
        burnTickInterval = interval;
        if (burnTickTimer <= 0) burnTickTimer = interval;
    }
}