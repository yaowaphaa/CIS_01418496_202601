using UnityEngine;
using System.Collections;

public class PlayerCollisionHandler : MonoBehaviour
{
    [Header("Settings")]
    public float invulnerableDuration = 1f;
    public int manaLossOnHit = 10;
    public string obstacleLayerName = "Obstacle"; // ชื่อ Layer ของอุปสรรค

    private bool isInvulnerable = false;
    private PlayerAttack playerAttack; // ไว้อ้างอิงไปที่ Script เดิม
    private Renderer[] renderers;      // เก็บ Renderer ทั้งหมด (เผื่อตัวละครมีหลายส่วน)

    void Start()
    {
        // ดึง Script PlayerAttack มาเก็บไว้เพื่อลด Mana
        playerAttack = GetComponent<PlayerAttack>();
        // ดึง Renderer ทั้งหมดในตัวละครมาเตรียมทำตัวกระพริบ
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && !isInvulnerable)
        {
            StartCoroutine(HandleObstacleHit());
        }
    }

    IEnumerator HandleObstacleHit()
    {
        isInvulnerable = true;

        // 1. ลด Mana ผ่าน Script PlayerAttack
        if (playerAttack != null)
        {
            playerAttack.battleMana -= manaLossOnHit;
            if (playerAttack.battleMana < 0) playerAttack.battleMana = 0;
            Debug.Log("💥 ชน! Mana เหลือ: " + playerAttack.battleMana);
        }

        // 2. ตั้งค่าให้ทะลุ Layer (Player ทะลุกับ Obstacle)
        int playerLayer = gameObject.layer;
        int obstacleLayer = LayerMask.NameToLayer(obstacleLayerName);
        Physics.IgnoreLayerCollision(playerLayer, obstacleLayer, true);

        // 3. ตัวกระพริบสีแดง (วนลูปสลับสี)
        float elapsed = 0;
        while (elapsed < invulnerableDuration)
        {
            SetColor(Color.red);
            yield return new WaitForSeconds(0.1f);
            SetColor(Color.white); // หรือสีเดิมที่คุณต้องการ
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.2f;
        }

        // 4. กลับสู่สภาวะปกติ
        SetColor(Color.white);
        Physics.IgnoreLayerCollision(playerLayer, obstacleLayer, false);
        isInvulnerable = false;
    }

    // ฟังก์ชันช่วยเปลี่ยนสีทุกส่วนของร่างกาย
    void SetColor(Color color)
    {
        foreach (var r in renderers)
        {
            if (r != null) r.material.color = color;
        }
    }
}
