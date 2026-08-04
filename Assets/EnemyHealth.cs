using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int hp = 3; 
    public int goldReward = 15; // Giết con này được 15 vàng

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            // Báo cho máy chủ cộng tiền (Script này mình sẽ tạo ở Bước 2)
            GameEconomy.Instance.AddGold(goldReward);
            
            Destroy(gameObject); // Bay màu
        }
    }
}