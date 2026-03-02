using UnityEngine;

public class Projectiledamage : MonoBehaviour
{
    public int damage = 3;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
            ฆ    enemy.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}