using UnityEngine;
using System;
using System.Collections;

public class DashSkill : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public float dashDuration = 3f; // ⏱ เวลา Dash สูงสุด
    public float dashStopDistance = 1.5f;
    public int dashDamage = 30;
    public GameObject dashEffectPrefab;
    public float postDashInvincibleDuration = 0.3f; // ช่วงสั้นๆ หลัง Dash

    private bool isDashing = false;
    private HealthSystem playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<HealthSystem>();
    }

    public void StartDash(Transform target, Action onComplete = null)
    {
        if (isDashing || target == null) return;
        StartCoroutine(DashRoutine(target, onComplete));
    }

    public bool IsDashing() => isDashing;

    private IEnumerator DashRoutine(Transform target, Action onComplete)
    {
        isDashing = true;

        // 1️⃣ ระหว่าง Dash → ผู้เล่นอมตะและไม่โดนดาเมจ
        if (playerHealth != null)
        {
            playerHealth.isInvincible = true;
            Debug.Log("🟡 เริ่ม Dash → ผู้เล่นอมตะและไม่โดนดาเมจมอน");
        }

        GameObject effect = null;
        if (dashEffectPrefab != null)
        {
            effect = Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.SetParent(transform);
        }

        float timer = 0f;

        // --- Dash ไปยังเป้าหมาย ตามเวลาที่กำหนด ---
        while (timer < dashDuration && target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, target.position);

            transform.position += direction * dashSpeed * Time.deltaTime;

            if (distance <= dashStopDistance)
            {
                // ไม่ทำดาเมจตอน Dash
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (effect != null)
            Destroy(effect, 0.5f);

        // 2️⃣ หลัง Dash → ผู้เล่นยังอมตะ, มอนที่ชนตายทันที
        timer = 0f;
        while (timer < postDashInvincibleDuration)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 1f); // ปรับ radius ตามต้องการ
            foreach (var hit in hits)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(9999); // ตายทันที
                    Debug.Log("💀 มอน " + enemy.name + " ตายจากการชนผู้เล่น");
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // 3️⃣ หลังช่วงสั้นๆ → กลับเป็นปกติ
        if (playerHealth != null)
        {
            playerHealth.isInvincible = false;
            Debug.Log("🔴 ช่วงสั้นๆ จบ → ผู้เล่นกลับเป็นปกติ");
        }

        isDashing = false;
        onComplete?.Invoke();
    }
}