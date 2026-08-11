using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    private GameObject shooterTower; // Lưu lại tháp nào đã bắn viên đạn này để truyền máu về

    public float speed = 15f;
    public int damage = 1;
    
    [Header("Hiệu ứng nổ (AOE)")]
    public float aoeRadius = 0f; // Để bằng 0 thì nó là đạn đơn mục tiêu
    public GameObject explosionVFX; // Kéo Prefab vụ nổ vào đây

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

    [Header("Hiệu ứng Hút máu (Tháp Đất/Tank)")]
    public bool isLifestealBullet = false; 
    [Range(0f, 1f)] public float lifestealPercent = 0.5f; // Hút 50% lượng sát thương gây ra thành máu

    
    // Nâng cấp hàm Seek để tháp khi bắn có thể nhồi thông tin của nó vào viên đạn
    public void Seek(Transform _target, GameObject _shooter)
    {
        target = _target;
        shooterTower = _shooter; 
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Bổ sung: Xoay đầu viên đạn hướng về phía mục tiêu
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        // 1. SINH RA HÌNH ẢNH VỤ NỔ (VFX)
        if (explosionVFX != null)
        {
            // Đẻ cái hình ảnh nổ ra tại đúng vị trí viên đạn vừa chạm đích
            GameObject effectIns = Instantiate(explosionVFX, transform.position, transform.rotation);
            
            // Xóa hình ảnh vụ nổ sau 2 giây để không bị rác bộ nhớ game
            Destroy(effectIns, 2f); 
        }

        // 2. GÂY SÁT THƯƠNG
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

        // 3. XÓA VIÊN ĐẠN
        Destroy(gameObject);
    }

    void ApplyEffects(GameObject enemyGo)
    {
        // Lấy component ra trước và kiểm tra an toàn (tránh lỗi Null Reference)
        EnemyHealth eHealth = enemyGo.GetComponent<EnemyHealth>();
        EnemyMovement eMove = enemyGo.GetComponent<EnemyMovement>();

        if (eHealth == null) return; 

        // 1. TÍNH TOÁN SÁT THƯƠNG CHÍ MẠNG
        int finalDamage = damage;
        if (isCritBullet)
        {
            if (Random.value <= critChance)
            {
                finalDamage *= critMultiplier;
                Debug.Log("CHÍ MẠNG! Đấm phát chết luôn: " + finalDamage + " dame"); 
            }
        }

        // 2. Trừ máu gốc của quái
        eHealth.TakeDamage(finalDamage);

        // 3. HÚT MÁU (Gửi máu về cho tháp)
        if (isLifestealBullet && shooterTower != null)
        {
            // Tính số máu hút được
            float healAmount = Mathf.Max(1f, finalDamage * lifestealPercent); 
            
            // Tìm tháp gốc và bơm máu cho nó
            TowerHealth tHealth = shooterTower.GetComponent<TowerHealth>();
            if (tHealth != null)
            {
                tHealth.Heal(healAmount); 
            }
        }

        // 4. Trát bùn làm chậm 
        if (isSlowBullet && eMove != null)
        {
            eMove.ApplySlow(slowPercentage, slowDuration);
        }

        // 5. Châm lửa đốt máu
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