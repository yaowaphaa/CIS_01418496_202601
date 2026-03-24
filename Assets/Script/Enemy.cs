using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 2;
    private int currentHealth;

    public GameObject itemPrefab;
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
}