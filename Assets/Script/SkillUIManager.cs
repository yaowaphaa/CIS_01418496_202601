using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SkillUIManager : MonoBehaviour
{
    public PlayerAttack player; // ลากใส่ใน Inspector

    [Header("Mana & Coins")]
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI scoreText;

    [Header("Cooldown Images")]
    // ลาก Image ที่เป็นตัว Fill มาใส่0=Q, 1=W, 2=E, 3=R
    public Image[] cooldownFills; 
    public Image[] skillIcons;
    private bool isBossScene;

    void Start()
    {
        string scene = SceneManager.GetActiveScene().name;
        isBossScene = scene == "ProjectLevel1" || scene == "ProjectLevel2" || scene == "ProjectLevel3";

        // ✅ ถ้าเป็นซีนบอส ทำให้ icon Q (index 0) เป็นสีดำ
        if (isBossScene && skillIcons.Length > 0 && skillIcons[0] != null)
            skillIcons[0].color = new Color(64f, 61f, 61f, 0.6f);
    }

    void Update()
    {
        if (player == null) return;
        UpdateCooldowns();
    }

    void UpdateCooldowns()
    {
        for (int i = 0; i < cooldownFills.Length; i++)
        {
            if (isBossScene && i == 0) continue;
            float lastUsed = player.lastUsedTimes[i];
            float cooldown = player.skillCooldowns[i];
            
            float timePassed = Time.time - lastUsed;

            if (timePassed < cooldown)
            {
                cooldownFills[i].fillAmount = 1 - (timePassed / cooldown);
            }
            else
            {
                cooldownFills[i].fillAmount = 0;
            }
        }
    }
}