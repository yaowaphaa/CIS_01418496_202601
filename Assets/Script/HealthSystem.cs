using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int health = 3; 
    public int maxHealth = 3;
    [HideInInspector] public bool isInvincible = false; // ⚡ เพิ่ม flag อมตะชั่วครู่

    [Header("UI Elements")]
    public Image[] hearts;        
    public GameObject gameOverPanel; 
    private float lastDamageTime;

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        Time.timeScale = 1f; 
        UpdateHeartsUI();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            Debug.Log("💨 ผู้เล่นอมตะ ไม่ลดเลือด");
            return; // ไม่ลดเลือดถ้าอมตะ
        }

        if (Time.time < lastDamageTime + 0.2f) return;

        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        Debug.Log("Current Health: " + health);
        UpdateHeartsUI();

        if (health <= 0)
            GameOver();

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
        Debug.Log("Game Over!");
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