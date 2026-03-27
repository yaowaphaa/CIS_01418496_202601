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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        UpdateUI();
        if (portalScript != null) portalScript.gameObject.SetActive(false);
    }

    void Update()
    {
        if (healthSlider.value != currentHealth)
            healthSlider.value = Mathf.Lerp(healthSlider.value, currentHealth, Time.deltaTime * 5f);
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;
        currentHealth -= damage;
        UpdateUI();
        if (anim != null) anim.SetTrigger("GetHit");
        if (currentHealth <= maxHealth * 0.75f && !hasEscaped)
        {
            hasEscaped = true;
            StartCoroutine(EscapeSequence());
        }
    }

    IEnumerator EscapeSequence()
    {
        yield return new WaitForSeconds(0.5f); 
        if (portalScript != null)
        {
            Transform cam = Camera.main.transform;
            Vector3 spawnPos = cam.position + (cam.forward * distanceFromCam);
            Vector3 dirToBoss = (transform.position - spawnPos).normalized;
            portalScript.ActivatePortal(spawnPos, Quaternion.LookRotation(dirToBoss) * Quaternion.Euler(0, -90, 0));
        }
        yield return new WaitForSeconds(1.0f); 
        if (portalScript != null)
        {
            Vector3 dirToPortal = (portalScript.transform.position - transform.position).normalized;
            dirToPortal.y = 0; // ล็อกแกน Y ไว้ไม่ให้บอสเงยหน้า
            Quaternion targetRotation = Quaternion.LookRotation(dirToPortal);
            
            float time = 0;
            while (time < 1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, time * rotationSpeed);
                time += Time.deltaTime;
                yield return null;
            }
            transform.rotation = targetRotation;
        }

        if (anim != null) 
        { 
            anim.SetTrigger("JumpAway");
            yield return new WaitForSeconds(0.4f);
            Jump(); 
            yield return new WaitForSeconds(0.7f);
            if(BossModel != null) BossModel.SetActive(false); 
            yield return new WaitForSeconds(2.0f);
            if(portalScript != null) portalScript.gameObject.SetActive(false);
        }
    }

    public void Jump()
    {
        if (rb == null) return;
        rb.linearVelocity = Vector3.zero;
        Vector3 jumpDirection = (Vector3.up * jumpForce) + (transform.forward * (jumpForce * 2.5f));
        rb.AddForce(jumpDirection, ForceMode.Impulse);
    }

    void UpdateUI() { healthText.text = Mathf.Round(currentHealth) + " / " + maxHealth; }
}