using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class BossHealth : MonoBehaviour
{
    public float maxHealth = 1000f;
    public float currentHealth;
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public Animator anim;
    public GameObject BossModel;
    public float rotationSpeed = 8f; 
    public float jumpForce = 22f; 
    private Rigidbody rb;
    private bool hasEscaped = false;
    public SmartPortal portalScript;
    public float distanceFromCam = 60f;
    public float triggerPercent = 0.75f;
    public System.Action OnTriggerHP;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        UpdateUI();
    }

    void Update()
    {
        if (healthSlider.value != currentHealth)
            healthSlider.value = Mathf.Lerp(healthSlider.value, currentHealth, Time.deltaTime * 5f);
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;
        currentHealth -= damage; // ✅ ต้องมีบรรทัดนี้
        UpdateUI();
        if (anim != null) anim.SetTrigger("GetHit");
        if (currentHealth <= maxHealth * triggerPercent && !hasEscaped)
        {
            hasEscaped = true;
            OnTriggerHP?.Invoke(); // ✅ ยิง event ไปให้ portal
            StartCoroutine(EscapeSequence());
        }
    }
    IEnumerator EscapeSequence()
    {
        yield return new WaitForSeconds(1.5f);

        if (anim != null) 
        { 
            anim.SetTrigger("JumpAway");
            yield return StartCoroutine(RotateBackwards());
            yield return new WaitForSeconds(0.1f);
            Jump(); 
            yield return new WaitForSeconds(0.6f);
            if(BossModel != null) BossModel.SetActive(false); 
            Debug.Log("บอสหนีไปแล้ว");
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
        Vector3 jumpDirection = (Vector3.up * jumpForce) + (transform.forward * (jumpForce * 5f));
        rb.AddForce(jumpDirection, ForceMode.Impulse);
    }

    void UpdateUI() 
    { 
        healthText.text = Mathf.Round(currentHealth) + " / " + maxHealth; 
    }
    public void JumpToPortal(Vector3 targetPos)
    {
        if (rb == null) return;
        rb.linearVelocity = Vector3.zero; 
        Vector3 direction = (targetPos - transform.position).normalized;
        Vector3 jumpDirection = (Vector3.up * jumpForce) + (direction * (jumpForce * 2.5f));
        rb.AddForce(jumpDirection, ForceMode.Impulse);
    }
}