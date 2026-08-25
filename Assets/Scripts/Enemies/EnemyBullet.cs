using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 7f; 
    
    private Transform targetTower;
    private float bulletDamage;

    public void Seek(Transform target, float damage)
    {
        targetTower = target;
        bulletDamage = damage;
    }

    void Update()
    {
        if (targetTower == null)
        {
            Destroy(gameObject);
            return;
        }

        // 1. Tính toán hướng đi
        Vector3 dir = targetTower.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        // 2. [CODE MỚI] Xoay đầu mũi tên hướng về mục tiêu
        if (dir != Vector3.zero)
        {
            // Tính góc xoay bằng toán học (hàm Atan2)
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            // Xoay GameObject theo trục Z
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // 3. Di chuyển mũi tên tới mục tiêu
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    void HitTarget()
    {
        if (targetTower != null)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayEnemyBulletHit();
            // 1. Thử tìm Component Máu của Tháp
            TowerHealth tHealth = targetTower.GetComponent<TowerHealth>();
            if (tHealth != null)
            {
                tHealth.TakeDamage(bulletDamage);
                Debug.Log("💣 Đạn của quái trúng tháp, gây " + bulletDamage + " sát thương!");
            }
            else
            {
                // 2. [MỚI FIX] Nếu không phải tháp, thử tìm Component Máu của Nhà Chính
                BaseHealth bHealth = targetTower.GetComponent<BaseHealth>();
                if (bHealth != null)
                {
                    bHealth.TakeDamage(bulletDamage);
                    Debug.Log("🏰 Đạn của quái trúng NHÀ CHÍNH, gây " + bulletDamage + " sát thương!");
                }
            }
        }
        
        Destroy(gameObject); // Chạm mục tiêu thì tự hủy đạn
    }
}