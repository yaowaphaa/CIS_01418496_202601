using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthSystem : MonoBehaviour
{
    public int health = 3;     
    public Image[] hearts;        
    public Sprite heartFull;       
    public GameObject gameOverPanel;
    void Update()
    {
        
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].color = Color.white; 
            }
            else
            {
                hearts[i].color = Color.black;
            }
        }

         /*ทดสอบกดปุ่ม Space แล้วเลือดลด
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(1);
        }*/
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, 3);

        if (health <= 0)
        {
            Debug.Log("Game Over!");
            GameOver();
        }
    }
    void GameOver()
    {
        gameOverPanel.SetActive(true); 
        Time.timeScale = 0f;
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