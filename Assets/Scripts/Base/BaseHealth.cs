using UnityEngine;
using UnityEngine.SceneManagement; // Cần dòng này để load lại màn chơi khi thua

public class BaseHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    // Biến static này giúp TẤT CẢ quái vật trên bản đồ gọi thẳng vào nhà chính được luôn
    public static BaseHealth Instance; 

    void Awake()
    {
        // Đảm bảo trên bản đồ chỉ có duy nhất 1 cái nhà chính
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("🚨 BÁO ĐỘNG! Nhà chính bị cắn! Máu còn: " + currentHealth);

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("💀 GAME OVER! Nhà chính đã nổ văng miểng!");
        
        // Tạm thời: Khi thua sẽ load lại chính cái Scene (màn chơi) hiện tại từ đầu
        // Sau này m có thể thay bằng lệnh hiện bảng UI Game Over
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
}