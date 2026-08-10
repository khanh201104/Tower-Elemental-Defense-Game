using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    public float speed = 15f;
    public int damage = 1;
    
    [Header("Hiệu ứng nổ (AOE)")]
    public float aoeRadius = 0f; // Để bằng 0 thì nó là đạn đơn mục tiêu

    [Header("Chí mạng (Crit)")]
    public bool isCritBullet = false;
    [Range(0f, 1f)] public float critChance = 0.2f; // Tỉ lệ 20%
    public int critMultiplier = 2; // X2 sát thương khi nổ Crit

    [Header("Hiệu ứng Làm chậm (CC)")]
    public bool isSlowBullet = false; 
    [Range(0f, 1f)] public float slowPercentage = 0.5f; 
    public float slowDuration = 2f; 

    [Header("Hiệu ứng Đốt máu (Burn)")]
    public bool isBurnBullet = false;      
    public int burnDamagePerTick = 1;      
    public float burnDuration = 3f;        
    public float burnTickRate = 1f;        

    public void Seek(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        if (aoeRadius > 0f)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, aoeRadius);
            foreach (Collider2D col in colliders)
            {
                if (col.CompareTag("Enemy"))
                {
                    ApplyEffects(col.gameObject); 
                }
            }
        }
        else
        {
            ApplyEffects(target.gameObject);
        }

        Destroy(gameObject);
    }

    void ApplyEffects(GameObject enemyGo)
    {
        // 1. TÍNH TOÁN SÁT THƯƠNG CHÍ MẠNG (Dành cho Đất)
        int finalDamage = damage;
        if (isCritBullet)
        {
            // Random.value sẽ quay xổ số ra một con số từ 0.0 đến 1.0
            if (Random.value <= critChance)
            {
                finalDamage *= critMultiplier;
                Debug.Log("CHÍ MẠNG! Đấm phát chết luôn: " + finalDamage + " dame"); 
            }
        }

        // 2. Trừ máu gốc 
        enemyGo.GetComponent<EnemyHealth>().TakeDamage(finalDamage);

        // 3. Trát bùn làm chậm (Dành cho Nước)
        if (isSlowBullet)
        {
            enemyGo.GetComponent<EnemyMovement>().ApplySlow(slowPercentage, slowDuration);
        }

        // 4. Châm lửa đốt máu (Dành cho Lửa)
        if (isBurnBullet)
        {
            enemyGo.GetComponent<EnemyHealth>().ApplyBurn(burnDamagePerTick, burnDuration, burnTickRate);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}