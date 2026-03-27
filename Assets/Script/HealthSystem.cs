using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int health = 3;
    public int maxHealth = 3;

    [HideInInspector] public bool isInvincible = false; // สำหรับ Dash หรือสกิลอื่น

    [Header("UI Elements")]
    public Image[] hearts;
    public GameObject gameOverPanel;

    private float lastDamageTime;
    public float damageCooldown = 0.2f;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f;
        UpdateHeartsUI();
    }

    public void TakeDamage(int damage)
    {
        // กันโดนเร็วเกิน
        if (Time.time < lastDamageTime + damageCooldown) return;

        // ถ้าอมตะไม่โดน
        if (isInvincible) return;

        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHeartsUI();

        Debug.Log("❤️ ลดเลือดผู้เล่น: " + damage + " เหลือ " + health);

        if (health <= 0) GameOver();

        lastDamageTime = Time.time;
    }

    void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;
            hearts[i].color = (i < health) ? Color.white : Color.black;
        }
    }

    void GameOver()
    {
        Debug.Log("💀 Game Over");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Main()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Lobby");
    }
}