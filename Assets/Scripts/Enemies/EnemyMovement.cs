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

    void Start()
    {
        currentSpeed = baseSpeed;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

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

    void Update()
    {
        if (slowTimer > 0)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0)
            {
                currentSpeed = baseSpeed;
                currentSlowPercent = 0f;
            }
        }

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        FindTarget();

        if (currentTargetTower != null)
        {
            if (attackTimer <= 0)
            {
                AttackTower();
                attackTimer = attackCooldown;
            }
        }
        else if (waypoints != null && targetIndex < waypoints.Length)
        {
            MoveAlongPath();
        }
        else
        {
            if (attackTimer <= 0)
            {
                AttackBase();
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
                // [MỚI FIX] CHỈ ĐÁNH THÁP ĐANG ACTIVE TRÊN SÂN
                TowerController towerCtrl = hit.GetComponent<TowerController>();
                if (towerCtrl != null && !towerCtrl.isOperational)
                {
                    continue; // Bỏ qua tháp nằm ở hàng chờ (Inactive)
                }

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

    void AttackTower()
    {
        if (currentTargetTower == null) return;

        if (bulletPrefab != null)
        {
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            GameObject bulletGO = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            EnemyBullet bulletScript = bulletGO.GetComponent<EnemyBullet>();

            if (bulletScript != null)
            {
                bulletScript.Seek(currentTargetTower, damage);
            }
        }
        else
        {
            TowerHealth tHealth = currentTargetTower.GetComponent<TowerHealth>();
            if (tHealth != null)
            {
                tHealth.TakeDamage(damage);
            }
        }
    }

    void AttackBase()
    {
        if (BaseHealth.Instance == null) return;

        if (bulletPrefab != null)
        {
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            GameObject bulletGO = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            EnemyBullet bulletScript = bulletGO.GetComponent<EnemyBullet>();

            if (bulletScript != null)
            {
                bulletScript.Seek(BaseHealth.Instance.transform, damage);
            }
        }
        else
        {
            BaseHealth.Instance.TakeDamage(damage);
        }
    }

    void MoveAlongPath()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform targetPoint = waypoints[targetIndex];

        if (spriteRenderer != null)
        {
            float directionX = targetPoint.position.x - transform.position.x;
            if (directionX > 0.1f)
            {
                spriteRenderer.flipX = true;
            }
            else if (directionX < -0.1f)
            {
                spriteRenderer.flipX = false;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, currentSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
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