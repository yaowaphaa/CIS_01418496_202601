using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMovement : MonoBehaviour
{
    public float speed = 3f;
    public float rotationSpeed = 200f;
    public float obstacleCheckDistance = 1f;
    public float detectionRange = 5f; // ระยะเริ่มตามผู้เล่น
    public float stopFollowRange = 10f; // ถ้าผู้เล่นไกลเกินนี้ → ไม่ตาม

    private Transform player;
    private Rigidbody rb;
    private bool playerPassed = false; // ถ้าผู้เล่นวิ่งผ่าน

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
        if (player == null || playerPassed) return;

        // ตรวจสอบระยะห่างของผู้เล่น
        float distance = Vector3.Distance(rb.position, player.position);

        if (distance > stopFollowRange)
        {
            // ถ้าผู้เล่นไกลเกินไป → ไม่เดินตาม
            return;
        }

        if (distance > detectionRange)
        {
            // ถ้าผู้เล่นอยู่ไกลกว่า detectionRange แต่ไม่เกิน stopFollowRange → สามารถใส่พฤติกรรมอื่นได้ (เช่น เดินเฉยๆ)
            return;
        }

        // ตรวจสอบว่าผู้เล่นวิ่งผ่านศัตรู (แกน X ของผู้เล่นมากกว่าศัตรู)
        if (player.position.x > rb.position.x)
        {
            playerPassed = true;
            return;
        }

        Vector3 dir = (player.position - rb.position).normalized;
        dir.y = 0;

        // ตรวจสอบ obstacle
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, out hit, obstacleCheckDistance))
        {
            Vector3 newDir = Vector3.Cross(Vector3.up, dir);
            if (Random.value > 0.5f)
                newDir = -newDir;

            dir = newDir.normalized;
        }

        // MovePosition
        Vector3 newPos = rb.position + dir * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);

        if (dir != Vector3.zero)
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(dir), rotationSpeed * Time.fixedDeltaTime);
    }
}