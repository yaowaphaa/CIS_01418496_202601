using UnityEngine;
using System.Collections;

public class PlayerCollisionHandler : MonoBehaviour
{
    [Header("Settings")]
    public float invulnerableDuration = 1.5f; // เวลาอมตะ
    public int manaLossOnHit = 10;           // จำนวนที่ลด
    public string obstacleLayerName = "Obstacle";

    private bool isInvulnerable = false;
    private PlayerAttack playerAttack;
    private Renderer[] renderers;

    void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();
        // ดึง Renderer ทั้งหมดในโมเดล (รวมถึงลูกๆ) เพื่อทำ Effect กระพริบ
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ตรวจสอบ Tag และสถานะอมตะ
        if (collision.gameObject.CompareTag("Obstacle") && !isInvulnerable)
        {
            StartCoroutine(HandleObstacleHit());
        }
    }

    IEnumerator HandleObstacleHit()
    {
        isInvulnerable = true;

        // ✅ เรียกฟังก์ชันลด Mana ที่เราสร้างไว้ใน PlayerAttack
        if (playerAttack != null)
        {
            playerAttack.TakeDamage(manaLossOnHit);
        }

        // --- ระบบทะลุสิ่งกีดขวางชั่วคราว ---
        int playerLayer = gameObject.layer;
        int obstacleLayer = LayerMask.NameToLayer(obstacleLayerName);

        if (obstacleLayer != -1)
            Physics.IgnoreLayerCollision(playerLayer, obstacleLayer, true);

        // --- Effect ตัวกระพริบแดง-ขาว ---
        float elapsed = 0;
        while (elapsed < invulnerableDuration)
        {
            SetColor(Color.red);
            yield return new WaitForSeconds(0.1f);
            SetColor(Color.white);
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.2f;
        }

        // --- คืนค่าสถานะปกติ ---
        SetColor(Color.white);
        if (obstacleLayer != -1)
            Physics.IgnoreLayerCollision(playerLayer, obstacleLayer, false);

        isInvulnerable = false;
    }

    void SetColor(Color color)
    {
        foreach (var r in renderers)
        {
            if (r != null)
            {
                // เปลี่ยนสี Material (รองรับ Shader มาตรฐาน)
                r.material.color = color;
            }
        }
    }
}
