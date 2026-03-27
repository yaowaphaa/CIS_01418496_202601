using UnityEngine;

public class BossRunnerX : MonoBehaviour
{
    
    public Transform player; 

    
    public float forwardDistance = 12f; 
    public float smoothTime = 0.2f;     
    
    
    public float activationRange = 15f; // ระยะที่ถ้า Player วิ่งมาถึง บอสถึงจะเริ่มรักษาระยะห่าง

    private Vector3 currentVelocity = Vector3.zero;
    private bool hasSeenPlayer = false; // ตัวเช็คว่าเริ่มการทำงานหรือยัง

    void LateUpdate()
    {
        if (player == null) return;

        // 1. คำนวณระยะห่างปัจจุบัน (แกน X เท่านั้น)
        float distanceToPlayer = transform.position.x - player.position.x;

        // 2. เช็คว่าผู้เล่นวิ่งเข้ามาใกล้พอที่จะ "ปลุก" บอสหรือยัง
        if (!hasSeenPlayer && distanceToPlayer <= activationRange)
        {
            hasSeenPlayer = true; // บอสโดนปลุกแล้ว!
            Debug.Log("Boss Activated!");
        }

        // 3. ถ้าบอสโดนปลุกแล้ว ถึงจะเริ่มรักษาระยะห่างนำหน้าผู้เล่น
        if (hasSeenPlayer)
        {
            Vector3 targetPosition = new Vector3(player.position.x + forwardDistance, transform.position.y, transform.position.z);

            transform.position = Vector3.SmoothDamp(
                transform.position, 
                targetPosition, 
                ref currentVelocity, 
                smoothTime
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        // วาดเส้นรัศมีปลุกบอส (สีแดง)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activationRange);
    }
}