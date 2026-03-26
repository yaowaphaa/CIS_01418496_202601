using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMovement : MonoBehaviour
{
    public float speed = 0.25f;  // ความเร็วเดิน
    private Transform player;
    private Rigidbody rb;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;     // มอนไม่ตกแมพ
        rb.freezeRotation = true;  // ป้องกันหมุนเองเมื่อชน collider
        rb.isKinematic = false;    // ให้ Physics จัดการการชน
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            // เดินตรงไปหา player
            Vector3 dir = (player.position - rb.position).normalized;
            dir.y = 0;

            Vector3 newPos = rb.position + dir * speed * Time.fixedDeltaTime;

            // MovePosition → ใช้ Physics ให้ชน collider จริง
            rb.MovePosition(newPos);

            // หมุนหน้าไปทาง player
            if (dir != Vector3.zero)
                rb.rotation = Quaternion.LookRotation(dir);
        }
    }
}