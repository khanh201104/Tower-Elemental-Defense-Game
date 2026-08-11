using UnityEngine;

public class TowerHealth : MonoBehaviour
{
    public float maxHealth = 50f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Hàm nhận sát thương từ quái
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log(gameObject.name + " bị vã! Máu còn: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Hàm nhận máu từ đạn Tháp Đất mang về
    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) 
        {
            currentHealth = maxHealth; // Chặn không cho hồi lố máu tối đa
        }
        Debug.Log("💚 " + gameObject.name + " HÚT MÁU! Máu hiện tại: " + currentHealth);
    }

    void Die()
    {
        Debug.Log("💥 " + gameObject.name + " đã sập!");
        
        // TODO: Chỗ này m sẽ cần clear ô gạch để người chơi được xây tháp khác vào (làm sau)
        
        Destroy(gameObject);
    }
}