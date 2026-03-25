using UnityEngine;

public class MagnetItem : MonoBehaviour
{
    public float speed = 15f;        
    public float acceleration = 25f; 
    public float pickupRange = 5f;   
    private Transform player;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    void FixedUpdate()
    {
        if (player == null || rb == null) return;
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= pickupRange)
        {
            rb.useGravity = false; 
            Vector3 dir = (player.position - transform.position).normalized;
            speed += acceleration * Time.fixedDeltaTime;
            rb.linearVelocity = dir * speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerAttack playerScript = other.GetComponent<PlayerAttack>();
            if (playerScript != null)
            {
                playerScript.AddMana(1);
            }

            Destroy(gameObject);
        }
    }
}