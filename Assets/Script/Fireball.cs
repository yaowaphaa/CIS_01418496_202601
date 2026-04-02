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
    public float speed = 100f;

    private Rigidbody rb;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.freezeRotation = true;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
        rb.linearVelocity = transform.forward * speed;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = transform.forward * speed;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void OnTriggerEnter(Collider other)
    {
        // 🔥 เช็คทิศทางก่อน
        Vector3 directionToTarget = (other.transform.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, directionToTarget);

        // ถ้าอยู่ด้านหลัง ไม่ต้องทำอะไร
        if (dot < 0.3f) return;

        Debug.Log($"Fireball ชนโดน: {other.name} Tag: {other.tag}");

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

        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}