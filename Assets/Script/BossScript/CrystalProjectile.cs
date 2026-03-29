using UnityEngine;
using System.Collections;

public class CrystalProjectile : MonoBehaviour {
    public float baseSpeed = 15f;      // ความเร็วพื้นฐานของกระสุน
    public float extraSpeedRatio = 1.1f; // ตัวคูณความเร็ว (เช่น เร็วกว่าผู้เล่น 10%)
    public float autoDestroyTime = 5f;
        private float finalSpeed;

    void Start()
    {

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            PlayerBossMovement playerScript = playerObj.GetComponent<PlayerBossMovement>();
            if (playerScript != null)
            {
                finalSpeed = baseSpeed + (playerScript.speed * extraSpeedRatio);
            }
        }
        else
        {
            finalSpeed = baseSpeed;
        }

        Destroy(gameObject, autoDestroyTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * finalSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<HealthSystem>()?.TakeDamage(1);
            Destroy(gameObject); 
        }
        
        // ถ้าชนพื้นให้หายไป
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}