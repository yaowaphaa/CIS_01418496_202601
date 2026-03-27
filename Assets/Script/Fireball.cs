using UnityEngine;
using System.Collections.Generic;

public class Fireball : MonoBehaviour
{
    public float lifeTime = 3f;
    public float damage = 20f;

    private List<GameObject> hitEnemies = new List<GameObject>();

    void Start()
    {
        
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Fireball ชนโดน: " + other.name + " Tag: " + other.tag);
        if (other.CompareTag("Enemy") && !hitEnemies.Contains(other.gameObject))
        {
            hitEnemies.Add(other.gameObject);

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage((int)damage);

            BossHealth boss = other.GetComponent<BossHealth>();
            if (boss != null)
            {
                boss.TakeDamage(damage); // ส่งดาเมจไปที่ BossHealth
            }
        }

        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}