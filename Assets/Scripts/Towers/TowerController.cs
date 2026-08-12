using UnityEngine;

public class TowerController : MonoBehaviour
{
    [Header("Trạng Thái Hoạt Động")]
    public bool isOperational = false; // Mặc định là false (Chưa hoạt động)

    private SpriteRenderer spriteRenderer;
    private Collider2D towerCollider;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        towerCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        // Khi vừa tạo ra, nếu chưa Operational thì làm mờ tháp + tắt Collider
        if (!isOperational)
        {
            SetOperational(false);
        }
    }

    // Hàm Bật/Tắt trạng thái hoạt động của Tháp
    public void SetOperational(bool state)
    {
        isOperational = state;

        // 1. Quản lý Collider (Quái chỉ vã tháp khi state = true)
        if (towerCollider != null)
        {
            towerCollider.enabled = isOperational;
        }

        // 2. Quản lý Đồ họa (Làm mờ khi chưa đặt, hiện rõ khi đã đặt)
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = isOperational ? 1f : 0.5f; // Alpha = 1 (Hiện rõ), Alpha = 0.5 (Mờ)
            spriteRenderer.color = color;
        }

        if (isOperational)
        {
            Debug.Log("✅ " + gameObject.name + " đã được đặt ĐÚNG VỊ TRÍ và bắt đầu HOẠT ĐỘNG!");
        }
    }

    void Update()
    {
        // BẮT BUỘC: Nếu tháp chưa hoạt động -> Ngưng mọi logic phía dưới
        if (!isOperational) return;
    }
}