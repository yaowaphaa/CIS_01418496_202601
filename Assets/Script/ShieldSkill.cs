using UnityEngine;

public class ShieldSkill : MonoBehaviour
{
    [Header("Shield Settings")]
    public float shieldDuration = 3f;           // เวลาที่โล่คงอยู่
    public GameObject shieldVisualPrefab;       // Prefab โล่

    private HealthSystem playerHealth;
    private bool shieldActive = false;
    private GameObject activeShield;

    void Start()
    {
        playerHealth = GetComponent<HealthSystem>();
    }

    public void ActivateShield()
    {
        if (shieldActive)
        {
            Debug.Log("🛡 โล่ยังเปิดอยู่ ไม่สามารถเปิดซ้ำได้!");
            return;
        }

        shieldActive = true;

        if (playerHealth != null)
        {
            playerHealth.isInvincible = true;
            Debug.Log("🛡 ผู้เล่นกำลังใช้สกิล: Shield → อมตะเปิดแล้ว!");
        }

        if (shieldVisualPrefab != null)
        {
            // สร้างโล่และวางตรงตัวผู้เล่น
            activeShield = Instantiate(shieldVisualPrefab, transform.position, transform.rotation);
            activeShield.transform.SetParent(transform); // ทำให้โล่ติดผู้เล่น
        }

        Debug.Log("🛡 โล่เปิดใช้งาน! เวลาคงอยู่: " + shieldDuration + " วินาที");

        // เริ่ม Coroutine ปิดโล่หลังเวลาหมด
        StartCoroutine(ShieldRoutine());
    }

    private System.Collections.IEnumerator ShieldRoutine()
    {
        float elapsed = 0f;

        while (elapsed < shieldDuration)
        {
            // โล่ติดตัวผู้เล่นแล้ว ไม่ต้องอัปเดตตำแหน่งเอง
            elapsed += Time.deltaTime;
            yield return null;
        }

        DeactivateShield();
    }

    private void DeactivateShield()
    {
        shieldActive = false;

        if (playerHealth != null)
        {
            playerHealth.isInvincible = false;
            Debug.Log("🛡 ผู้เล่นเลิกใช้สกิล: Shield → อมตะปิดแล้ว!");
        }

        if (activeShield != null)
        {
            Destroy(activeShield);
            activeShield = null;
        }

        Debug.Log("🛡 โล่ปิดแล้ว!");
    }

    public bool IsShieldActive()
    {
        return shieldActive;
    }
}