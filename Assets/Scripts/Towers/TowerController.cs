using UnityEngine;

public class TowerController : MonoBehaviour
{
    [Header("Trạng Thái Hoạt Động")]
    public bool isOperational = false; 

    private SpriteRenderer spriteRenderer;
    private Collider2D towerCollider;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        towerCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        if (!isOperational)
        {
            SetOperational(false);
        }
    }

    // Hàm bổ sung: Tùy chỉnh độ mờ linh hoạt
    public void SetAlpha(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }

    public void SetOperational(bool state)
    {
        isOperational = state;

        if (towerCollider != null)
        {
            towerCollider.enabled = true; 
        }

        // Active = 100% (1f), Inactive trên Hàng chờ = 80% (0.8f)
        SetAlpha(isOperational ? 1f : 0.8f);

        if (isOperational)
        {
            Debug.Log("✅ " + gameObject.name + " đã được đặt ĐÚNG VỊ TRÍ và bắt đầu HOẠT ĐỘNG!");
        }
    }

    void Update()
    {
        if (!isOperational) return;
    }
}