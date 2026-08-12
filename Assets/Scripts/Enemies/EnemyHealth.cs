using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int hp = 3;
    public int goldReward = 15;

    [Header("VFX Hiệu Ứng (Kéo Object con vào đây)")]
    public GameObject fireVFX; 
    public GameObject slowVFX;

    [Header("Màu sắc khi dính trạng thái")]
    public Color burnColor = new Color(1f, 0.4f, 0.2f);        // Màu Cam Đỏ (Đốt)
    public Color slowColor = new Color(0.4f, 0.8f, 1f);        // Màu Xanh Băng (Chậm)
    public Color combinedColor = new Color(0.8f, 0.3f, 0.9f);  // Màu Tím Ma Thuật (Cả 2)

    private SpriteRenderer spriteRenderer;
    private EnemyMovement enemyMovement;

    [Header("Hiệu ứng Đốt Máu")]
    private float burnTimer = 0f;
    private float burnTickTimer = 0f;
    private int burnDamage = 0;
    private float burnTickInterval = 1f;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        enemyMovement = GetComponent<EnemyMovement>();

        if (fireVFX != null) fireVFX.SetActive(false);
        if (slowVFX != null) slowVFX.SetActive(false);
    }

    void Update()
    {
        // 1. Đếm ngược hiệu ứng Đốt Máu
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

        // 2. Cập nhật hiển thị VFX và Màu sắc chính xác theo trạng thái
        UpdateStatusVisuals();
    }

    void UpdateStatusVisuals()
    {
        bool isBurned = burnTimer > 0;
        bool isSlowed = (enemyMovement != null && enemyMovement.IsSlowed);

        // TH1: Dính CẢ 2 HIỆU ỨNG (Bật cả 2 VFX + Nhuộm màu Tím)
        if (isBurned && isSlowed)
        {
            if (fireVFX != null) fireVFX.SetActive(true);
            if (slowVFX != null) slowVFX.SetActive(true);
            if (spriteRenderer != null) spriteRenderer.color = combinedColor;
        }
        // TH2: Chỉ bị ĐỐT (Bật Fire_VFX + Nhuộm màu Cam Đỏ)
        else if (isBurned)
        {
            if (fireVFX != null) fireVFX.SetActive(true);
            if (slowVFX != null) slowVFX.SetActive(false);
            if (spriteRenderer != null) spriteRenderer.color = burnColor;
        }
        // TH3: Chỉ bị LÀM CHẬM (Bật Slow_VFX + Nhuộm màu Xanh Băng)
        else if (isSlowed)
        {
            if (fireVFX != null) fireVFX.SetActive(false);
            if (slowVFX != null) slowVFX.SetActive(true);
            if (spriteRenderer != null) spriteRenderer.color = slowColor;
        }
        // TH4: BÌNH THƯỜNG (Tắt hết VFX + Trả về màu Trắng gốc)
        else
        {
            if (fireVFX != null) fireVFX.SetActive(false);
            if (slowVFX != null) slowVFX.SetActive(false);
            if (spriteRenderer != null) spriteRenderer.color = Color.white;
        }
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            GameEconomy.Instance.AddGold(goldReward);
            Destroy(gameObject);
        }
    }

    public void ApplyBurn(int damagePerTick, float duration, float interval)
    {
        burnDamage = Mathf.Max(burnDamage, damagePerTick);
        burnTimer = duration; 
        burnTickInterval = interval;
        if (burnTickTimer <= 0) burnTickTimer = interval;
    }
}