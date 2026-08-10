using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int hp = 3; 
    public int goldReward = 15; 

    [Header("Hiệu ứng Đốt Máu")]
    private float burnTimer = 0f;        // Thời gian bị cháy còn lại
    private float burnTickTimer = 0f;    // Đồng hồ đếm ngược từng nhịp (tick)
    private int burnDamage = 0;          // Mỗi nhịp cháy mất bao nhiêu máu?
    private float burnTickInterval = 1f; // Tốc độ cháy (VD: 1 giây mất máu 1 lần)

    void Update()
    {
        // Nếu quái đang trong trạng thái bị đốt
        if (burnTimer > 0)
        {
            burnTimer -= Time.deltaTime;     // Trừ dần tổng thời gian cháy
            burnTickTimer -= Time.deltaTime; // Trừ dần thời gian chờ nhịp tiếp theo

            // Nếu đến nhịp cháy (tick)
            if (burnTickTimer <= 0)
            {
                TakeDamage(burnDamage);           // Trừ máu
                burnTickTimer = burnTickInterval; // Reset lại đồng hồ chờ cho nhịp sau
            }
        }
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            GameEconomy.Instance.AddGold(goldReward);
            Destroy(gameObject); 
        }
    }

    // Viên đạn Lửa/Dung Nham sẽ gọi hàm này
    public void ApplyBurn(int damagePerTick, float duration, float interval)
    {
        burnDamage = damagePerTick;
        burnTimer = duration;
        burnTickInterval = interval;
        burnTickTimer = interval; // Đợi hết 1 nhịp (VD: 1s) rồi mới đốt phát đầu tiên
    }
}