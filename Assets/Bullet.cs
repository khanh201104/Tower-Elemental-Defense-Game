using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    public float speed = 10f;
    public int damage = 1;

    // Tháp sẽ gọi hàm này để truyền mục tiêu cho viên đạn
    public void Seek(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        // Nếu quái đã chết trước khi đạn bay tới -> Xóa viên đạn
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Đạn bay đuổi theo quái
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Chạm mục tiêu -> Trừ máu quái và tự hủy đạn
        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            target.GetComponent<EnemyHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}