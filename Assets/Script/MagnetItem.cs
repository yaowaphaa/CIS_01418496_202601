using UnityEngine;

public class MagnetItem : MonoBehaviour
{
    public float speed = 10f;
    public float pickupRange = 3f;
    private Transform player;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= pickupRange)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (player.position - transform.position).normalized;
                rb.linearVelocity = dir * speed;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ต้องตรง Tag
        if (other.CompareTag("Player"))
        {
            // เพิ่ม Mana/Coins
            PlayerAttack playerScript = other.GetComponent<PlayerAttack>();
            if (playerScript != null)
            {
                playerScript.AddMana(1);
            }

            // 🔥 ลบไอเท็มทันที
            Destroy(gameObject);
        }
    }
}