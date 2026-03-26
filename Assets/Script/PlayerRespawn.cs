using UnityEngine;

public class ObstacleDodge : MonoBehaviour
{
    [Header("Dodge Settings")]
    public float dodgeDistance = 1.0f; // ระยะที่จะขยับออกข้าง
    public float scanDistance = 2.0f;  // ระยะสแกนหาที่ว่าง

    private PlayerStats playerStats;

    void Start()
    {
        // เชื่อมต่อกับสคริปต์ PlayerStats
        playerStats = GetComponent<PlayerStats>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // เช็ค Tag "Obstacle"
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // ลดแต้ม hpoint
            if (playerStats != null)
            {
                playerStats.DecreaseHPoint(1);
            }

            // สั่งให้ขยับหลบไปทางที่ว่าง
            DodgeToSpace();
        }
    }

    void DodgeToSpace()
    {
        Vector3 dodgeDirection = Vector3.zero;

        // ใช้ Raycast สแกนหาที่ว่าง
        bool hitRight = Physics.Raycast(transform.position, transform.right, scanDistance);
        bool hitLeft = Physics.Raycast(transform.position, -transform.right, scanDistance);

        if (!hitRight)
        {
            dodgeDirection = transform.right; // ไปขวาถ้าว่าง
        }
        else if (!hitLeft)
        {
            dodgeDirection = -transform.right; // ไปซ้ายถ้าขวาไม่ว่างแต่ซ้ายว่าง
        }
        else
        {
            dodgeDirection = -transform.forward; // ถอยหลังถ้าติดทั้งคู่
        }

        MovePosition(dodgeDirection * dodgeDistance);
    }

    void MovePosition(Vector3 offset)
    {
        CharacterController cc = GetComponent<CharacterController>();

        // ถ้ามี Character Controller ต้องปิดก่อนย้ายตำแหน่งแป๊บนึง
        if (cc != null) cc.enabled = false;

        transform.position += offset;

        if (cc != null) cc.enabled = true;

        Debug.Log("ขยับหลบพร้อมลด hpoint เรียบร้อย!");
    }
}
