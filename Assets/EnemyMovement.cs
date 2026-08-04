using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 3f;      
    
    // Đã đổi thành private vì bây giờ quái sẽ tự tìm đường, không cần kéo thả tay nữa
    private Transform[] waypoints; 
    private int targetIndex = 0;  

    void Start()
    {
        // Khi vừa đẻ ra, tự động tìm thư mục tên "Path" trên màn hình
        Transform pathFolder = GameObject.Find("Path").transform;
        
        // Lấy tất cả các điểm mốc (Point_0, Point_1...) nhét vào trí nhớ
        waypoints = new Transform[pathFolder.childCount];
        for (int i = 0; i < pathFolder.childCount; i++)
        {
            waypoints[i] = pathFolder.GetChild(i);
        }
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return; // Đề phòng lỗi chưa có đường

        if (targetIndex < waypoints.Length)
        {
            Transform targetPoint = waypoints[targetIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                targetIndex++;
            }
        }
        else
        {
            // Tạm thời đến đích thì tự hủy (sau này thêm code trừ máu nhà chính ở đây)
            Destroy(gameObject); 
        }
    }
}