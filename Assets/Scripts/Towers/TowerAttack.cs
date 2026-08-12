using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    [Header("Chỉ số tấn công")]
    public float range = 3f;
    public float fireRate = 1f;
    private float fireCountdown = 0f;

    public GameObject bulletPrefab;

    private TowerController towerController;

    void Start()
    {
        // Tự động tìm component TowerController trên cùng Tháp
        towerController = GetComponent<TowerController>();
    }

    void Update()
    {
        // BẮT BUỘC: Nếu tháp chưa được đặt hợp lệ -> Bỏ qua không cho bắn
        if (towerController != null && !towerController.isOperational) return;

        fireCountdown -= Time.deltaTime;

        if (fireCountdown <= 0f)
        {
            ShootNearestEnemy();
        }
    }

    void ShootNearestEnemy()
    {
        // Tối ưu: Chỉ quét các Collider nằm trong tầm bắn
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, range);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (Collider2D col in enemiesInRange)
        {
            if (col.CompareTag("Enemy"))
            {
                float distanceToEnemy = Vector3.Distance(transform.position, col.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = col.gameObject;
                }
            }
        }

        if (nearestEnemy != null)
        {
            GameObject bulletGO = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Bullet bulletScript = bulletGO.GetComponent<Bullet>();

            if (bulletScript != null)
            {
                bulletScript.Seek(nearestEnemy.transform, gameObject);
            }

            fireCountdown = 1f / fireRate;
        }
    }

    // Hiển thị vòng tròn tầm bắn của tháp trong tab Scene để dễ căn chỉnh
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}