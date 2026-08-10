using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Chỉ số di chuyển")]
    public float baseSpeed = 3f;      // Tốc độ gốc
    private float currentSpeed;       // Tốc độ thực tế lúc đang chạy
    private float slowTimer = 0f;     // Đồng hồ đếm ngược thời gian bị làm chậm

    private Transform[] waypoints; 
    private int targetIndex = 0;  

    void Start()
    {
        currentSpeed = baseSpeed; // Mới đẻ ra thì chạy với tốc độ gốc
        Transform pathFolder = GameObject.Find("Path").transform;
        waypoints = new Transform[pathFolder.childCount];
        for (int i = 0; i < pathFolder.childCount; i++)
        {
            waypoints[i] = pathFolder.GetChild(i);
        }
    }

    void Update()
    {
        // 1. Đồng hồ đếm ngược hiệu ứng bùn lầy
        if (slowTimer > 0)
        {
            slowTimer -= Time.deltaTime; // Trừ dần thời gian
            if (slowTimer <= 0)
            {
                currentSpeed = baseSpeed; // Hết bùn thì trả lại tốc độ cũ
            }
        }

        // 2. Di chuyển (Dùng currentSpeed thay vì tốc độ gốc)
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
            Destroy(gameObject); 
        }
    }

    // Hàm này để viên đạn gọi vào khi trúng mục tiêu
    public void ApplySlow(float slowPercentage, float duration)
    {
        // Ví dụ slowPercentage = 0.5 (giảm 50% tốc)
        currentSpeed = baseSpeed * (1f - slowPercentage); 
        
        // Reset lại đồng hồ đếm ngược
        slowTimer = duration; 
    }
}