using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    [Header("Chỉ số tấn công")]
    public float range = 3f;      // Tầm bắn
    public float fireRate = 1f;   // Tốc độ bắn (1 viên/giây)
    private float fireCountdown = 0f;

    public GameObject bulletPrefab; // Nhét Prefab viên đạn vào đây

    void Update()
    {
        fireCountdown -= Time.deltaTime;
        
        // Đã hồi đạn xong -> Bắn quái gần nhất
        if (fireCountdown <= 0f)
        {
            ShootNearestEnemy();
        }
    }

    void ShootNearestEnemy()
    {
        // Tìm TẤT CẢ những thằng đang mang Tag "Enemy" trên bản đồ
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        // So sánh xem thằng nào gần nhất
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            // Nếu nằm trong tầm bắn và là thằng gần nhất nãy giờ tìm thấy
            if (distanceToEnemy < shortestDistance && distanceToEnemy <= range)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        // Lấy con gần nhất ra chém... à nhầm, bắn
        if (nearestEnemy != null)
        {
            // Sinh ra viên đạn tại vị trí của tháp
            GameObject bulletGO = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            
            // Truyền tọa độ con quái cho viên đạn đuổi theo
            bulletGO.GetComponent<Bullet>().Seek(nearestEnemy.transform);
            
            // Reset thời gian chờ
            fireCountdown = 1f / fireRate;
        }
    }
}