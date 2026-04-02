using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(TrailRenderer))]
public class Fireball : MonoBehaviour
{
    [Header("Fireball Settings")]
    public float lifeTime = 10f;
    public float damage = 20f;
    public float speed = 20f; // ความเร็ว Fireball

    private Rigidbody rb;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>(); // ป้องกันชนซ้ำ

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.freezeRotation = true; // ป้องกันหมุนเอง
    }

    void Start()
    {
        Destroy(gameObject, lifeTime); // ทำลายตัวเองหลังหมดเวลา
        rb.linearVelocity = transform.forward * speed; // เริ่มเคลื่อนที่
    }

    void FixedUpdate()
    {
        rb.linearVelocity = transform.forward * speed; // รักษาความเร็วคงที่
    }

    /// <summary>
    /// ปรับความเร็ว Fireball แบบไดนามิก
    /// </summary>
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
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