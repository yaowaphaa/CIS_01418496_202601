using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int health = 3;
    public int maxHealth = 3;
    [HideInInspector] public bool isInvincible = false;

    [Header("UI Elements")]
    public Image[] hearts;
    public GameObject gameOverPanel;

    [Header("Movement & Slow Effect")]
    public PlayerMovement movementScript; 
    public float slowMultiplier = 0.5f;   
    public float slowDuration = 1.5f;     
    private float originalSpeed;
    private bool isSlowed = false;

    [Header("Visual Feedback")]
    public Renderer playerRenderer; 
    private float lastDamageTime;
    public float damageCooldown = 0.5f;

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (movementScript == null) movementScript = GetComponent<PlayerMovement>();
        UpdateHeartsUI();
    }

    // แก้เป็น OnTriggerEnter เพื่อให้เดินทะลุได้จั้บ!
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            TakeDamage(1);
            if (!isSlowed && movementScript != null)
            {
                StartCoroutine(SlowEffect());
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (Time.time < lastDamageTime + damageCooldown || isInvincible) return;

        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHeartsUI();
        StartCoroutine(DamageFlash());

        if (health <= 0) GameOver();
        lastDamageTime = Time.time;
    }

    IEnumerator SlowEffect()
    {
        isSlowed = true;
        originalSpeed = movementScript.speed; // เช็คชื่อ moveSpeed ใน PlayerMovement ด้วยนะจั้บ
        movementScript.speed = originalSpeed * slowMultiplier;
        yield return new WaitForSeconds(slowDuration);
        movementScript.speed = originalSpeed;
        isSlowed = false;
    }

    IEnumerator DamageFlash()
    {
        if (playerRenderer == null) yield break;
        for (int i = 0; i < 3; i++)
        {
            playerRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            playerRenderer.material.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void UpdateHeartsUI()
    {
        if (hearts == null) return;
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
                hearts[i].color = (i < health) ? Color.white : Color.black;
        }
    }

    void GameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Retry() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void Main() { Time.timeScale = 1f; SceneManager.LoadScene("Lobby"); }
}