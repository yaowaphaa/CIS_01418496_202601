using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int health = 3; 
    public int maxHealth = 3;

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
    void Update()
    {
        /* ทดสอบ: กด Spacebar แล้วเลือดลด 1 (เอาไว้เช็ก UI หัวใจ)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(1);
            Debug.Log("ทดสอบลดเลือด: เหลือ " + health);
        }*/
    }

    // ฟังก์ชันรับดาเมจ (เรียกใช้จากสคริปต์มอนสเตอร์)
    public void TakeDamage(int damage)
    {
        if (Time.time < lastDamageTime + 0.2f) return;
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        Debug.Log("Current Health in System: " + health);
        UpdateHeartsUI();

        if (health <= 0)
        {
            GameOver();
        }
    }


    void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;

            if (i < health){
                hearts[i].color = Color.white; // เลือดเหลือ = สีปกติ
            }else{
                Debug.Log("สั่งให้หัวใจดวงที่ " + i + " กลายเป็นสีดำ");
                hearts[i].color = Color.black; // เลือดหมด = สีดำ
            }
            
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over! Showing Panel...");
        
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