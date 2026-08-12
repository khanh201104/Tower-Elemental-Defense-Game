using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    private Vector3 lastTargetPosition; // Lưu vị trí cuối cùng phòng trường hợp quái chết giữa đường
    private GameObject shooterTower;

    public float speed = 15f;
    public int damage = 1;

    [Header("Hiệu ứng nổ (AOE)")]
    public float aoeRadius = 0f;
    public GameObject explosionVFX;

    [Header("Chí mạng (Crit)")]
    public bool isCritBullet = false;
    [Range(0f, 1f)] public float critChance = 0.2f;
    public int critMultiplier = 2;

    [Header("Hiệu ứng Làm chậm (CC)")]
    public bool isSlowBullet = false;
    [Range(0f, 1f)] public float slowPercentage = 0.5f;
    public float slowDuration = 2f;

    [Header("Hiệu ứng Đốt máu (Burn)")]
    public bool isBurnBullet = false;
    public int burnDamagePerTick = 1;
    public float burnDuration = 3f;
    public float burnTickRate = 1f;

    [Header("Hiệu ứng Hút máu")]
    public bool isLifestealBullet = false;
    [Range(0f, 1f)] public float lifestealPercent = 0.5f;

    public void Seek(Transform _target, GameObject _shooter)
    {
        target = _target;
        shooterTower = _shooter;
        if (target != null) lastTargetPosition = target.position;
    }

    void Update()
    {
        // Nếu quái còn sống thì cập nhật vị trí mới nhất
        if (target != null)
        {
            lastTargetPosition = target.position;
        }

        Vector3 targetPos = (target != null) ? target.position : lastTargetPosition;

        // Xoay đầu viên đạn hướng về mục tiêu
        Vector3 dir = targetPos - transform.position;
        if (dir != Vector3.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Chạm đích
        if (Vector3.Distance(transform.position, targetPos) < 0.2f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        // 1. SINH VFX VỤ NỔ
        if (explosionVFX != null)
{
    GameObject effectIns = Instantiate(explosionVFX, transform.position, transform.rotation);
    
    // Nếu là đạn nổ AOE -> Tự phóng to hình ảnh nổ theo bán kính aoeRadius
    if (aoeRadius > 0f)
    {
        // Nhân với 2 vì aoeRadius là BÁN KÍNH, còn Scale đại diện cho ĐƯỜNG KÍNH
        effectIns.transform.localScale = Vector3.one * (aoeRadius * 2f); 
    }

    Destroy(effectIns, 2f); 
}

        // 2. TÍNH CRIT 1 LẦN DUY NHẤT CHO VIÊN ĐẠN NÀY
        int calculatedDamage = damage;
        if (isCritBullet && Random.value <= critChance)
        {
            calculatedDamage *= critMultiplier;
            Debug.Log($"[CRIT HIT] Dội {calculatedDamage} sát thương!");
        }

        // 3. GÂY SÁT THƯƠNG (AOE HOẶC ĐƠN MỤC TIÊU)
        if (aoeRadius > 0f)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, aoeRadius);
            
            // Dùng HashSet để tránh 1 quái có nhiều Collider bị dính sát thương nhiều lần
            HashSet<EnemyHealth> processedEnemies = new HashSet<EnemyHealth>();

            foreach (Collider2D col in colliders)
            {
                if (col.CompareTag("Enemy"))
                {
                    EnemyHealth eHealth = col.GetComponent<EnemyHealth>();
                    if (eHealth != null && !processedEnemies.Contains(eHealth))
                    {
                        processedEnemies.Add(eHealth);
                        ApplyEffectsToEnemy(eHealth, col.gameObject, calculatedDamage);
                    }
                }
            }
        }
        else if (target != null)
        {
            EnemyHealth eHealth = target.GetComponent<EnemyHealth>();
            if (eHealth != null)
            {
                ApplyEffectsToEnemy(eHealth, target.gameObject, calculatedDamage);
            }
        }

        // 4. XÓA VIÊN ĐẠN
        Destroy(gameObject);
    }

    void ApplyEffectsToEnemy(EnemyHealth eHealth, GameObject enemyGo, int finalDamage)
    {
        EnemyMovement eMove = enemyGo.GetComponent<EnemyMovement>();

        // 1. Trừ máu quái
        eHealth.TakeDamage(finalDamage);

        // 2. Hút máu cho tháp
        if (isLifestealBullet && shooterTower != null)
        {
            float healAmount = Mathf.Max(1f, finalDamage * lifestealPercent);
            TowerHealth tHealth = shooterTower.GetComponent<TowerHealth>();
            if (tHealth != null)
            {
                tHealth.Heal(healAmount);
            }
        }

        // 3. Làm chậm
        if (isSlowBullet && eMove != null)
        {
            eMove.ApplySlow(slowPercentage, slowDuration);
        }

        // 4. Đốt máu
        if (isBurnBullet)
        {
            eHealth.ApplyBurn(burnDamagePerTick, burnDuration, burnTickRate);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}