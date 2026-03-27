using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealth : MonoBehaviour
{
    public float maxHealth = 1000f;
    public float currentHealth;
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

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
        healthSlider.value = currentHealth;
    }

    void Update()
    {
        // ทำให้แถบเลือดค่อยๆ ลดลงแบบสมูท (Lerp)
        if (healthSlider.value != currentHealth)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, currentHealth, Time.deltaTime * 5f);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        UpdateUI();
        // ถ้าต้องการให้เลือดลดทันที (ไม่ใช้ Lerp ใน Update) 
        // ให้ใช้บรรทัดนี้แทน: healthSlider.value = currentHealth;
    }
}