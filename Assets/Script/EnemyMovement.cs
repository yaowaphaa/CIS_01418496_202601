using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMovement : MonoBehaviour
{
    public float speed = 3f;
    public float rotationSpeed = 200f;
    public float obstacleCheckDistance = 1f;

    private Transform player;
    private Rigidbody rb;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector3 dir = (player.position - rb.position).normalized;
        dir.y = 0;

        // ตรวจสอบสิ่งกีดขวางข้างหน้า
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, out hit, obstacleCheckDistance))
        {
            // ถ้ามี obstacle → เลี้ยวซ้ายหรือขวาสุ่ม
            Vector3 newDir = Vector3.Cross(Vector3.up, dir); // เลี้ยว 90 องศา
            if (Random.value > 0.5f)
                newDir = -newDir;

            dir = newDir.normalized;
        }

        // MovePosition
        Vector3 newPos = rb.position + dir * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);

        // หมุนหน้าไปทาง dir
        if (dir != Vector3.zero)
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(dir), rotationSpeed * Time.fixedDeltaTime);
    }
}