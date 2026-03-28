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
        if (portalScript != null)
        {
            Transform cam = Camera.main.transform;
            Vector3 spawnPos = cam.position + (cam.forward * distanceFromCam);
            spawnPos.y = transform.position.y;
            portalScript.ActivatePortal(spawnPos);
        }
        yield return new WaitForSeconds(1.5f); 
        if (portalScript != null)
        {

            Vector3 dirToPortal = (portalScript.transform.position - transform.position).normalized;
            dirToPortal.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(dirToPortal);
            
            float time = 0;
            while (time < 0.5f) 
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
            yield return new WaitForSeconds(0.4f); // จังหวะย่อตัว

            if (portalScript != null)
            {
                JumpToPortal(portalScript.transform.position); 
            }
            yield return new WaitForSeconds(0.6f);
            if(BossModel != null) BossModel.SetActive(false); 
            Debug.Log("บอสหนีไปแล้ว ประตูยังเปิดอยู่ให้ผู้เล่นตามไป");
        }
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