using UnityEngine;
using System;
using System.Collections;

public class DashSkill : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 25f;
    public float dashStopDistance = 1.5f;
    public int dashDamage = 30;
    public GameObject dashEffectPrefab;

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

        // ⚡ เปิดอมตะตอน Dash
        if (playerHealth != null)
        {
            playerHealth.isInvincible = true;
            Debug.Log("🟡 เริ่ม Dash → อมตะ");
        }

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
                break;
            }

            yield return null;
        }

        if (effect != null)
            Destroy(effect, 0.5f);

        // ⚡ ปิดอมตะหลัง Dash + เพิ่มช่วงสั้น ๆ (0.3s)
        if (playerHealth != null)
        {
            yield return new WaitForSeconds(0.3f);
            playerHealth.isInvincible = false;
            Debug.Log("🔴 ปิดอมตะหลัง Dash");
        }

        isDashing = false;
        onComplete?.Invoke();
    }

    private void DealDamage(Transform target)
    {
        if (target == null) return;
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(dashDamage);
            Debug.Log("💥 Dash โดน! ดาเมจ: " + dashDamage);
        }
    }
}