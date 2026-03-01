using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float lifeTime = 3f;
    public float damage = 20f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // ถ้าโดนศัตรู
        if (other.CompareTag("Enemy"))
        {
            // ตัวอย่างเรียกเลือดศัตรู
            // other.GetComponent<EnemyHealth>().TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}