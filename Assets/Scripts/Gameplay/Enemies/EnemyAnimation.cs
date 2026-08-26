using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    // Chuyển đổi trạng thái Đi bộ <-> Tấn công
    public void SetAttacking(bool isAttacking)
    {
        if (animator != null)
        {
            animator.SetBool("isAttacking", isAttacking);
        }
    }

    // Kích hoạt khi quái gục ngã
    public void PlayDieAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
    }

    // Lật mặt quái theo hướng di chuyển
    public void FlipSprite(bool moveLeft)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = moveLeft;
        }
    }
}