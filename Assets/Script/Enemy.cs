using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 2;
    private int currentHealth;
     public int attackDamage = 1; // จำนวนเลือดที่ลดต่อการโจมตี

    private EnemyDrop dropSystem; 

    void Start()
    {
        currentHealth = maxHealth;
         dropSystem = GetComponent<EnemyDrop>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log("เลือดมอนเหลือ: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("มอนตาย!");
        if (dropSystem != null)
        {
            dropSystem.Drop(); // 👈 เรียกดรอปตรงนี้
        }
        else
        {
            Debug.LogWarning("ไม่มี EnemyDrop!");
        }
        

        Destroy(gameObject);
    }

    // 🔹 ใช้ Trigger แทน Collision
   private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 💀 มอนตายทันที
            Die();

            // (จะเอาหรือไม่เอาก็ได้)
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }

            Debug.Log("ผู้เล่นชนมอน → มอนตาย!");
        }
    }
}