using UnityEngine;
using System.Collections;
public class CrystalProjectile : MonoBehaviour {
    public float speed = 15f;
    public float autoDestroyTime = 5f;
    void Start()
    {
        Destroy(gameObject, autoDestroyTime); // กันรกแมพ
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ใส่ Logic ลดเลือดผู้เล่นที่นี่ เช่น:
            other.GetComponent<HealthSystem>()?.TakeDamage(1);
            Destroy(gameObject); 
        }
    }
}