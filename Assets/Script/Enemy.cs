using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 2;
    private int currentHealth;
    public int attackDamage = 1; // ดาเมจที่จะลดเลือดผู้เล่น
    private bool hasAttacked = false;
    
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
            dropSystem.Drop(); 
        }
        
        Destroy(gameObject);
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Player") && !hasAttacked)
        {
           
            HealthSystem playerHealth = collision.gameObject.GetComponent<HealthSystem>();
            
            if (playerHealth != null)
            {
                hasAttacked = true;
                playerHealth.TakeDamage(attackDamage);
                Debug.Log("มอนชนผู้เล่น! ลดเลือด" + attackDamage);
                
                
                Die();
            }
        }
    }
}