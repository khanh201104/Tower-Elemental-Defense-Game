using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    [Header("Cài đặt Lạch bạch")]
    public float wobbleSpeed = 15f;   // Tốc độ lắc (Càng to lắc càng nhanh)
    public float wobbleAngle = 10f;   // Góc nghiêng tối đa (Nghiêng bao nhiêu độ)

    private float randomOffset;       // Biến tạo độ trễ ngẫu nhiên

    void Start()
    {
        // 💡 MẸO PRO: Tạo một con số ngẫu nhiên lúc quái mới sinh ra.
        // Việc này giúp 10 con Goblin tuôn ra cùng lúc sẽ con lắc trước, con lắc sau.
        // Tránh tình trạng cả bầy lắc đều tăm tắp như đang tập thể dục nhịp điệu!
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        // Dùng hàm Sin kết hợp thời gian thực để sinh ra góc nghiêng từ -10 độ đến 10 độ
        float currentAngle = Mathf.Sin((Time.time + randomOffset) * wobbleSpeed) * wobbleAngle;
        
        // Ép cái ảnh của con quái xoay theo góc vừa tính toán (Xoay trên trục Z của 2D)
        transform.localRotation = Quaternion.Euler(0f, 0f, currentAngle);
    }
}