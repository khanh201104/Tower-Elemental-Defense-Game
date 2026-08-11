using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Chỉ số di chuyển")]
    public float baseSpeed = 3f;      // Tốc độ gốc
    private float currentSpeed;       // Tốc độ thực tế lúc đang chạy
    private float slowTimer = 0f;     // Đồng hồ đếm ngược thời gian bị làm chậm

    [Header("Chỉ số chiến đấu")]
    public float attackRange = 1f;    // Tầm đánh (Cận chiến thì nhỏ, Đánh xa thì to)
    public float damage = 10f;        // Lượng sát thương gây ra
    public float attackCooldown = 1f; // Tốc độ đánh (Ví dụ: 1 giây đánh 1 cái)
    private float attackTimer = 0f;   // Đồng hồ đếm ngược đòn đánh

    [Header("Tấn công xa (Để trống nếu là quái Cận chiến)")]
    public GameObject bulletPrefab; // Viên đạn
    public Transform firePoint;     // Nòng súng (điểm đạn bay ra)
    private Transform[] waypoints; 
    private int targetIndex = 0;  
    private Transform currentTargetTower; // Tháp đang bị con quái này nhắm tới

    [Header("VFX")]
    public GameObject iceVFX; // Kéo object hình cục băng vào đây
    void Start()
    {
        currentSpeed = baseSpeed; 
        Transform pathFolder = GameObject.Find("Path").transform;
        waypoints = new Transform[pathFolder.childCount];
        for (int i = 0; i < pathFolder.childCount; i++)
        {
            waypoints[i] = pathFolder.GetChild(i);
        }
    }

    void Update()
    {
        // 1. Đồng hồ đếm ngược hiệu ứng bùn lầy (Giữ nguyên của m)
    if (slowTimer > 0)
{
        if (iceVFX != null) iceVFX.SetActive(true); // Bật hiệu ứng băng
        slowTimer -= Time.deltaTime; 
        if (slowTimer <= 0)
        {
        currentSpeed = baseSpeed; 
        if (iceVFX != null) iceVFX.SetActive(false); // Tắt hiệu ứng băng
        }
}

        // 2. Đồng hồ chờ đòn đánh
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // 3. Tìm tháp để đánh
        FindTarget();

        // 4. Quyết định Hành động: Đánh hoặc Đi
        if (currentTargetTower != null)
        {
            // CÓ THÁP TRONG TẦM -> Đứng lại và vã nó
            if (attackTimer <= 0)
            {
                Attack();
                attackTimer = attackCooldown; // Reset thời gian chờ vung tay
            }
        }
        else
        {
            // KHÔNG CÓ THÁP TRONG TẦM -> Tiếp tục di chuyển
            MoveAlongPath();
        }
    }

    // --- HÀM TÌM THÁP ---
    void FindTarget()
    {
        // Quét một vòng tròn xung quanh con quái để tìm xem có cái Tháp nào không
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        float shortestDistance = Mathf.Infinity;
        Transform nearestTower = null;

        foreach (Collider2D hit in hits)
        {
            // Bắt buộc m phải gán Tag "Tower" cho các Prefab Tháp trong Unity nhé
            if (hit.CompareTag("Tower")) 
            {
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

    // --- HÀM TẤN CÔNG ---
    void Attack()
    {
        // ĐẢM BẢO QUÁI ĐÁNH XA: Nếu có gắn Prefab viên đạn
        if (bulletPrefab != null)
        {
            // Điểm xuất phát của viên đạn (Nếu không có nòng súng thì lấy ngay tâm con quái)
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            
            // Đẻ viên đạn ra
            GameObject bulletGO = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            EnemyBullet bulletScript = bulletGO.GetComponent<EnemyBullet>();
            
            if (bulletScript != null)
            {
                // Nhồi thông tin mục tiêu và lượng sát thương vào viên đạn
                bulletScript.Seek(currentTargetTower, damage);
            }
        }
        // QUÁI CẬN CHIẾN & BOSS: Không có viên đạn -> Gây sát thương trực tiếp
        else
        {
            Debug.Log("CHÉM! " + gameObject.name + " chém cận chiến vào tháp gây " + damage + " dame!");
            // TODO: Trừ máu trực tiếp ở đây
        }
    }

    // --- HÀM DI CHUYỂN CŨ CỦA M ---
    void MoveAlongPath()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (targetIndex < waypoints.Length)
        {
            Transform targetPoint = waypoints[targetIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, currentSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                targetIndex++;
            }
        }
        else
        {
            // Chạy đến cuối đường -> Đụng nhà chính
            Debug.Log("Quái vật đã lọt vào nhà chính!");
            Destroy(gameObject); 
        }
    }

    // Hàm gọi khi bị dính đạn làm chậm
    public void ApplySlow(float slowPercentage, float duration)
    {
        currentSpeed = baseSpeed * (1f - slowPercentage); 
        slowTimer = duration; 
    }

    // Vẽ vòng tròn tầm đánh ra màn hình Editor để m dễ căn chỉnh
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}