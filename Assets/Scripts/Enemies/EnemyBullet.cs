using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 7f; // Tốc độ bay của đạn
    
    private Transform targetTower;
    private float bulletDamage;

    // Hàm này được gọi từ con quái để truyền mục tiêu và sát thương cho viên đạn
    public void Seek(Transform target, float damage)
    {
        targetTower = target;
        bulletDamage = damage;
    }

    void Update()
    {
        // Nếu trong lúc đạn đang bay mà tháp đã bị phá hủy (hoặc bị người chơi bán đi) -> Hủy viên đạn
        if (targetTower == null)
        {
            Destroy(gameObject);
            return;
        }

        // Tính toán khoảng cách di chuyển trong khung hình này
        Vector3 dir = targetTower.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        // Nếu đạn bay đủ gần mục tiêu -> Xử lý trúng đòn
        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        // Di chuyển viên đạn
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    void HitTarget()
    {
        Debug.Log("BÙM! Đạn của quái trúng tháp, gây " + bulletDamage + " sát thương!");
        
        // TODO: Lát nữa sang phần Hệ thống Máu Tháp, mình sẽ móc code trừ máu vào đây!

        // Trúng đích xong thì viên đạn phải biến mất
        Destroy(gameObject);
    }
}