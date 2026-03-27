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
    private bool hasEscaped = false;
    public float rotationSpeed = 5f; 

    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        UpdateUI();
    }

    void UpdateUI()
    {
        healthText.text = Mathf.Round(currentHealth) + " / " + maxHealth;
    }

    void Update()
    {
        if (healthSlider.value != currentHealth)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, currentHealth, Time.deltaTime * 5f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        
        UpdateUI();

        if (anim != null) anim.SetTrigger("GetHit");

        if (currentHealth <= maxHealth * 0.5f && !hasEscaped)
        {
            hasEscaped = true;
            StartCoroutine(SmoothTurnAndEscape());
        }
    }

    IEnumerator SmoothTurnAndEscape()
    {
        yield return new WaitForSeconds(2f);

        Quaternion targetRotation = Quaternion.LookRotation(-transform.forward);
    
        float time = 0;
        while (time < 1f)
        {

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, time);
            time += Time.deltaTime * rotationSpeed;
            yield return null;
        }

        
        if (anim != null) 
        {
            anim.SetTrigger("JumpAway");
        }
    }
}