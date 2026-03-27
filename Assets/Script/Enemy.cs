using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int maxHealth = 2;
    private int currentHealth;

    public int attackDamage = 1;
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
        Debug.Log("🩸 เลือดมอนเหลือ: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("💀 มอนตาย!");

        if (dropSystem != null)
            dropSystem.Drop();

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player") || hasAttacked)
            return;

        HealthSystem playerHealth = collision.gameObject.GetComponent<HealthSystem>();
        DashSkill dash = collision.gameObject.GetComponent<DashSkill>();

        if (playerHealth == null) return;

        // ❌ ถ้าผู้เล่นกำลัง Dash → ไม่โดนตี
        if (dash != null && dash.IsDashing()) return;

        // ❌ ถ้าผู้เล่นอมตะ → ไม่โดนตี
        if (playerHealth.isInvincible) return;

        hasAttacked = true;
        playerHealth.TakeDamage(attackDamage);

        Die();
    }
}