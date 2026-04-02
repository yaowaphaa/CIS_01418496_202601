using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(TrailRenderer))]
public class SpreadProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float lifeTime = 10f;
    public float damage = 10f;
    public float speed = 1000f; // ต้องมากกว่า player speed

    private Rigidbody rb;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>(); // ป้องกันชนซ้ำ

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // ตั้งค่า Rigidbody ให้เหมาะกับ projectile
        rb.isKinematic = false; // ใช้ฟิสิกส์
        rb.interpolation = RigidbodyInterpolation.Interpolate; // ทำให้ smooth
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // ป้องกันทะลุ
    }

    void Start()
    {
        // เคลื่อนที่ไปตามหน้าออบเจกต์
        rb.linearVelocity = transform.forward * speed;

        // ลบ projectile หลังหมดอายุ
        Destroy(gameObject, lifeTime);
    }

     void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Fireball ชนโดน: {other.name} Tag: {other.tag}");

        // ตรวจสอบชน Enemy หรือ Boss และยังไม่ชนซ้ำ
        if (other.CompareTag("Enemy") && !hitTargets.Contains(other.gameObject))
        {
            hitTargets.Add(other.gameObject);

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage((int)damage);

            BossHealth boss = other.GetComponent<BossHealth>();
            if (boss != null)
                boss.TakeDamage(damage);
        }

        // ถ้าเจอ Wall ทำลายตัวเอง
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}