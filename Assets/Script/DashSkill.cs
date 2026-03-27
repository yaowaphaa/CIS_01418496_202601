using UnityEngine;
using System;
using System.Collections;

public class DashSkill : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 25f;
    public float dashStopDistance = 1.5f;
    public int dashDamage = 30;
    public GameObject dashEffectPrefab; // Optional: ใส่ Prefab Particle

    private bool isDashing = false;
    private HealthSystem playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<HealthSystem>();
    }

    /// <summary>
    /// เริ่ม Dash ไปยังเป้าหมาย
    /// </summary>
    public void StartDash(Transform target, Action onComplete = null)
    {
        if (isDashing || target == null) return;
        StartCoroutine(DashRoutine(target, onComplete));
    }

    /// <summary>
    /// ตรวจสอบว่าผู้เล่นกำลัง Dash อยู่หรือไม่ (ใช้โดยมอน)
    /// </summary>
    public bool IsDashing()
    {
        return isDashing;
    }

    private IEnumerator DashRoutine(Transform target, Action onComplete)
    {
        isDashing = true;

        // ⚡ ผู้เล่นอมตะชั่วครู่ตอน Dash
        if (playerHealth != null)
            playerHealth.isInvincible = true;

        // สร้าง Dash Effect ถ้ามี
        GameObject effect = null;
        if (dashEffectPrefab != null)
        {
            effect = Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.SetParent(transform);
        }

        while (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, target.position);
            transform.position += direction * dashSpeed * Time.deltaTime;

            if (distance <= dashStopDistance)
            {
                DealDamage(target);

                // ⚡ หลัง Dash เสร็จ ทำให้ผู้เล่นอมตะชั่วครู่สั้น ๆ
                StartCoroutine(DashInvincibleCooldown(1f));

                break;
            }

            yield return null;
        }

        if (effect != null)
            Destroy(effect, 0.5f); // ให้ Particle เล่นจนจบ

        isDashing = false;
        onComplete?.Invoke();
    }

    /// <summary>
    /// ฟังก์ชันให้ผู้เล่นอมตะชั่วครู่หลัง Dash
    /// </summary>
    private IEnumerator DashInvincibleCooldown(float duration)
    {
        if (playerHealth == null) yield break;

        playerHealth.isInvincible = true;
        yield return new WaitForSeconds(duration);
        playerHealth.isInvincible = false;
    }

    /// <summary>
    /// ฟังก์ชันโจมตีเป้าหมาย
    /// </summary>
    private void DealDamage(Transform target)
    {
        if (target == null) return;

        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
            enemy.TakeDamage(dashDamage);

        Debug.Log("💥 Dash Punch ลดเลือด " + dashDamage);
    }
}