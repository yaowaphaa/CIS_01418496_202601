using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    // เพิ่มตัวแปร Static เพื่อเก็บค่าข้ามซีน
    public static float savedHealth = -1f; 
    public GameObject victoryScreen;
    public float maxHealth = 1000f;
    public float currentHealth;
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public Animator anim;
    public GameObject BossModel;
    private bool isDead = false;
    public PlayerBossMovement playerMove; 

    public float rotationSpeed = 8f; 
    public float jumpForce = 22f; 
    private Rigidbody rb;
    private bool hasEscaped = false;
    
    public float triggerPercent = 0.75f;
    public System.Action OnTriggerHP;

    void Start()
    {
        victoryScreen.SetActive(false);
        rb = GetComponent<Rigidbody>();

        if (savedHealth < 0) {
            currentHealth = maxHealth;
        } else {
            currentHealth = savedHealth;
        }

        if (healthSlider != null) {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        if (playerMove == null) {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerMove = p.GetComponent<PlayerBossMovement>();
        }

        UpdateUI();
    }

    void Update()
    {
        if (isDead) return;
        if (healthSlider != null && healthSlider.value != currentHealth)
            healthSlider.value = Mathf.Lerp(healthSlider.value, currentHealth, Time.deltaTime * 5f);
    }

    public void TakeDamage(float damage)
    {
        // เช็กอมตะช่วง Intro
        if (playerMove != null && playerMove.isIntroPlaying) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); 
        savedHealth = currentHealth; 
        
        UpdateUI();
        
        if (anim != null) anim.SetTrigger("GetHit");

        // เช็กบอสหนี
        if (currentHealth <= maxHealth * triggerPercent && !hasEscaped)
        {
            hasEscaped = true;
            OnTriggerHP?.Invoke(); 
            StartCoroutine(EscapeSequence());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator EscapeSequence()
    {
        if (isDead) yield break;
        yield return new WaitForSeconds(1.5f);
        if (isDead) yield break;
        if (anim != null) 
        { 
            anim.SetTrigger("JumpAway");
            yield return StartCoroutine(RotateBackwards());
            yield return new WaitForSeconds(0.1f);
            if (!isDead) Jump();
            yield return new WaitForSeconds(0.6f);
            if(BossModel != null) BossModel.SetActive(false); 
        }
    }

    IEnumerator RotateBackwards()
    {
        float targetAngle = transform.eulerAngles.y + 180f;
        float currentAngle = transform.eulerAngles.y;
        float elapsed = 0f;
        float duration = 0.5f; 

        while (elapsed < duration)
        {   
            if (isDead) yield break;
            elapsed += Time.deltaTime;
            float newY = Mathf.LerpAngle(currentAngle, targetAngle, elapsed / duration);
            transform.rotation = Quaternion.Euler(0f, newY, 0f);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
    }

    public void Jump()
    {
        if (rb == null) return;
        rb.linearVelocity = Vector3.zero;
        Vector3 jumpDirection = (Vector3.up * jumpForce) + (transform.forward * (jumpForce * 1.5f));
        rb.AddForce(jumpDirection, ForceMode.Impulse);
    }

    void UpdateUI() 
    { 
        if (healthText != null)
            healthText.text = Mathf.Round(currentHealth) + " / " + maxHealth; 
    }

    void Die()
    {   
        if (isDead) return;
        isDead = true;
        savedHealth = -1f;
        if (rb != null) {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        if (anim != null) anim.SetTrigger("Die");
        if (healthSlider != null) healthSlider.gameObject.SetActive(false);
        if (victoryScreen != null) victoryScreen.SetActive(true);
        if (playerMove != null) {
            playerMove.StartCoroutine(playerMove.SlowStop());
        }
    }
}