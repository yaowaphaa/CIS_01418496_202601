using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillUIManager : MonoBehaviour
{
    public PlayerAttack player; // ลากใส่ใน Inspector

    [Header("Mana & Coins")]
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI scoreText;

    [Header("Cooldown Images")]
    // ลาก Image ที่เป็นตัว Fill (Radial) มาใส่ตามลำดับ 0=Q, 1=W, 2=E, 3=R
    public Image[] cooldownFills; 

    void Update()
    {
        if (player == null) return;

        // 1. อัปเดตตัวเลข Mana
        if (scoreText != null)
        {
            scoreText.text = Score.score.ToString();
        }
        if (manaText != null)
            manaText.text = player.battleMana.ToString();

        // 2. อัปเดต Cooldown Fill
        UpdateCooldowns();
    }

    void UpdateCooldowns()
    {
        for (int i = 0; i < cooldownFills.Length; i++)
        {
            float lastUsed = player.lastUsedTimes[i];
            float cooldown = player.skillCooldowns[i];
            
            float timePassed = Time.time - lastUsed;

            if (timePassed < cooldown)
            {
                // แสดงผล Cooldown (ค่า 0 ถึง 1)
                cooldownFills[i].fillAmount = 1 - (timePassed / cooldown);
            }
            else
            {
                cooldownFills[i].fillAmount = 0;
            }
        }
    }
}