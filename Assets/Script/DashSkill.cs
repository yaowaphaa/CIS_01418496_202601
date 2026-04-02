using UnityEngine;
using System;
using System.Collections;

public class DashSkill : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public float dashDuration = 3f; 
    public float dashStopDistance = 1.5f;
    public float dashDamage = 1f;
    public GameObject dashEffectPrefab;
    public float postDashInvincibleDuration = 0.3f; 

    private bool isDashing = false;
    private HealthSystem playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<HealthSystem>();
    }

    public void StartDash(Transform target, Animator animator, Action onComplete = null)
    {
        if (isDashing || target == null) return;
        StartCoroutine(DashRoutine(target, animator, onComplete));
    }

    public bool IsDashing() => isDashing;

    private IEnumerator DashRoutine(Transform target, Animator animator, Action onComplete)
    {
        isDashing = true;
        BossMovement boss = target.GetComponent<BossMovement>();
        BossHealth bossHealth = target.GetComponent<BossHealth>();
        if (boss != null) boss.FreezeMovement(true);
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
        while (timer < dashDuration && target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, target.position);

            transform.position += direction * dashSpeed * Time.deltaTime;

            if (distance <= dashStopDistance)
            {
                if (animator != null) animator.SetTrigger("Attack"); 
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (effect != null)
            Destroy(effect, 0.5f);

        timer = 0f;
        bool hasHitBoss = false;
        while (timer < postDashInvincibleDuration)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 1f);
            foreach (var hit in hits)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.TakeDamage(9999);

                if (!hasHitBoss)
                {
                    BossHealth bh = hit.GetComponent<BossHealth>();
                    if (bh != null)
                    {
                        bh.TakeDamage(1f);
                        hasHitBoss = true;
                    }
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }

        if (boss != null) boss.FreezeMovement(false);
        if (playerHealth != null)
        {
            playerHealth.isInvincible = false;
            Debug.Log("🔴 ช่วงสั้นๆ จบ → ผู้เล่นกลับเป็นปกติ");
        }

        isDashing = false;
        onComplete?.Invoke();
    }
}