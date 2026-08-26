using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Chỉ số di chuyển")]
    public float baseSpeed = 3f;
    private float currentSpeed;
    private float slowTimer = 0f;
    private float currentSlowPercent = 0f;

    public bool IsSlowed => slowTimer > 0;

    [Header("Chỉ số chiến đấu")]
    public float attackRange = 1f;
    public float damage = 10f;
    public float attackCooldown = 1f; 
    private float attackTimer = 0f;

    [Header("Tấn công xa (Để trống nếu là quái Cận chiến)")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    private Transform[] waypoints;
    private int targetIndex = 0;
    private Transform currentTargetTower;

    private SpriteRenderer spriteRenderer;
    private EnemyAnimation enemyAnimation; 

    // --- MẢNG LƯU TRỮ ĐƯỜNG ĐI RIÊNG CỦA TỪNG CON QUÁI ---
    private Vector3[] myPath; 

    void Awake()
    {
        GameObject pathGO = GameObject.Find("Path");
        if (pathGO != null)
        {
            Transform pathFolder = pathGO.transform;
            waypoints = new Transform[pathFolder.childCount];
            for (int i = 0; i < pathFolder.childCount; i++)
            {
                waypoints[i] = pathFolder.GetChild(i);
            }
        }
    }

    void Start()
    {
        currentSpeed = baseSpeed;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        enemyAnimation = GetComponent<EnemyAnimation>();
    }

    // --- HÀM TẠO SẴN ĐƯỜNG ĐI SONG SONG (LANE) ---
    public void InitializePathOffset(float offset, Vector3 spawnPos)
    {
        if (waypoints == null || waypoints.Length == 0) return;

        myPath = new Vector3[waypoints.Length];

        for (int i = 0; i < waypoints.Length; i++)
        {
            Vector3 currentPoint = waypoints[i].position;
            Vector3 dirIn = Vector3.zero;
            Vector3 dirOut = Vector3.zero;

            if (i > 0) dirIn = (currentPoint - waypoints[i - 1].position).normalized;
            if (i < waypoints.Length - 1) dirOut = (waypoints[i + 1].position - currentPoint).normalized;

            if (i == 0) 
            {
                Vector3 perp = new Vector3(-dirOut.y, dirOut.x, 0);
                myPath[i] = currentPoint + perp * offset;
            }
            else if (i == waypoints.Length - 1) 
            {
                Vector3 perp = new Vector3(-dirIn.y, dirIn.x, 0);
                myPath[i] = currentPoint + perp * offset;
            }
            else
            {
                if (Vector3.Dot(dirIn, dirOut) > 0.99f) 
                {
                    Vector3 perp = new Vector3(-dirIn.y, dirIn.x, 0);
                    myPath[i] = currentPoint + perp * offset;
                }
                else 
                {
                    Vector3 perpIn = new Vector3(-dirIn.y, dirIn.x, 0);
                    Vector3 perpOut = new Vector3(-dirOut.y, dirOut.x, 0);
                    myPath[i] = currentPoint + (perpIn + perpOut) * offset;
                }
            }
        }

        Vector3 startDir = (waypoints.Length > 1) ? (waypoints[1].position - waypoints[0].position).normalized : Vector3.right;
        Vector3 startPerp = new Vector3(-startDir.y, startDir.x, 0);
        transform.position = spawnPos + startPerp * offset;
    }

    void Update()
    {
        if (spriteRenderer != null)
        {
            // Nhân với -100 để đảm bảo số Y nhỏ (đứng thấp) sẽ tạo ra Order lớn (nổi lên trên)
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100f);
        }
        if (slowTimer > 0)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0)
            {
                currentSpeed = baseSpeed;
                currentSlowPercent = 0f;
            }
        }

        if (attackTimer > 0) attackTimer -= Time.deltaTime;

        FindTarget();

        if (currentTargetTower != null)
        {
            if (enemyAnimation != null) enemyAnimation.SetAttacking(true);

            if (attackTimer <= 0)
            {
                // [THAY ĐỔI]: Phân loại cận chiến và đánh xa
                // Nếu là quái cận chiến (không có bulletPrefab) hoặc mất file Animation, chém ngay lập tức
                if (bulletPrefab == null || enemyAnimation == null) 
                {
                    AttackTower();
                }
                // Nếu là quái bắn cung (có bulletPrefab), nó sẽ CHỜ Animation Event gọi hàm SpawnArrowEvent()
                
                attackTimer = attackCooldown;
            }
        }
        else if (myPath != null && targetIndex < myPath.Length) 
        {
            if (enemyAnimation != null) enemyAnimation.SetAttacking(false);
            MoveAlongPath();
        }
        else
        {
            if (enemyAnimation != null) enemyAnimation.SetAttacking(true);

            if (attackTimer <= 0)
            {
                if (bulletPrefab == null || enemyAnimation == null) 
                {
                    AttackBase();
                }
                attackTimer = attackCooldown;
            }
        }
    }

    void FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        float shortestDistance = Mathf.Infinity;
        Transform nearestTower = null;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Tower"))
            {
                TowerController towerCtrl = hit.GetComponent<TowerController>();
                if (towerCtrl != null && !towerCtrl.isOperational) continue; 

                float distanceToTower = Vector2.Distance(transform.position, hit.transform.position);
                if (distanceToTower < shortestDistance)
                {
                    shortestDistance = distanceToTower;
                    nearestTower = hit.transform;
                }
            }
        }
        currentTargetTower = nearestTower;
    }

    // --- HÀM NÀY ĐƯỢC GỌI BỞI LÁ CỜ ANIMATION EVENT (DÀNH CHO QUÁI BẮN CUNG) ---
    public void SpawnArrowEvent()
    {
        if (currentTargetTower != null)
        {
            AttackTower();
        }
        else if (BaseHealth.Instance != null)
        {
            AttackBase();
        }
    }

    void AttackTower()
    {
        if (currentTargetTower == null) return;

        if (bulletPrefab != null)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayEnemyShoot();
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            GameObject bulletGO = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            EnemyBullet bulletScript = bulletGO.GetComponent<EnemyBullet>();
            if (bulletScript != null) bulletScript.Seek(currentTargetTower, damage);
        }
        else
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayEnemyMelee();
            TowerHealth tHealth = currentTargetTower.GetComponent<TowerHealth>();
            if (tHealth != null) tHealth.TakeDamage(damage);
        }
    }

    void AttackBase()
    {
        if (BaseHealth.Instance == null) return;

        if (bulletPrefab != null)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayEnemyShoot();
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            GameObject bulletGO = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            EnemyBullet bulletScript = bulletGO.GetComponent<EnemyBullet>();
            if (bulletScript != null) bulletScript.Seek(BaseHealth.Instance.transform, damage);
        }
        else
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayEnemyMelee();
            BaseHealth.Instance.TakeDamage(damage);
        }
    }

    void MoveAlongPath()
    {
        Vector3 targetPos = myPath[targetIndex];

        if (spriteRenderer != null)
        {
            float directionX = targetPos.x - transform.position.x;
            if (directionX > 0.1f)
            {
                if (enemyAnimation != null) enemyAnimation.FlipSprite(true);
                else spriteRenderer.flipX = true;
            }
            else if (directionX < -0.1f)
            {
                if (enemyAnimation != null) enemyAnimation.FlipSprite(false);
                else spriteRenderer.flipX = false;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, currentSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            targetIndex++;
        }
    }

    public void ApplySlow(float slowPercentage, float duration)
    {
        if (slowPercentage >= currentSlowPercent)
        {
            currentSlowPercent = slowPercentage;
            currentSpeed = baseSpeed * (1f - slowPercentage);
        }
        slowTimer = duration;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}