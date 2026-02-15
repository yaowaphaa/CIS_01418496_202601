using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public float fallMultiplier = 2.5f;
    public float speed = 1f;
    public float horizontalSpeed = 3f;
    public float jumpForce = 5f;
    public int impactCount = 3;          // จำนวนครั้งที่กระแทก
    public float preImpactBounce = 0.2f; // ความแรงของการกระแทกใต้ดิน
    public float preImpactSpeed = 5f;   // ความเร็วการกระแทก
    public float impactBounce = 0.15f; // ความเด้งตอนกระแทก
    public float impactSpeed = 12f;    // ความแรงตอนพุ่ง
    public float undergroundY = -1f; // ความลึกใต้ดินเริ่มต้น
    private Vector3 startPosition;
    private bool isRising = true;
    private Animator animator;
    private bool isGrounded;
    private Rigidbody rb;

    
    void Start()
    {

        
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

         // 1. บันทึกตำแหน่งที่ควรจะเป็น (ตำแหน่งที่คุณวางไว้ใน Editor)
        startPosition = transform.position;

        // 2. ย้ายวัตถุไปไว้ใต้ดินทันทีเมื่อเริ่ม
        transform.position = new Vector3(startPosition.x, undergroundY, startPosition.z);

        // 3. เริ่มการทำงานให้ค่อยๆ ผุดขึ้นมา
        StartCoroutine(RiseUp());

    }
    IEnumerator RiseUp()
    {
            Vector3 basePos = transform.position;
            // กระแทกใต้ดิน 3 ครั้ง
            for (int i = 0; i < impactCount; i++)
            {
                // กระแทกขึ้น
                float t = 0f;
                while (t < 1f)
                {   
                    t += Time.deltaTime * preImpactSpeed;
                    transform.position = basePos + Vector3.up * Mathf.Sin(t * Mathf.PI) * preImpactBounce;
                    yield return null;
                }
             // กลับตำแหน่งเดิม
            transform.position = basePos;
            yield return new WaitForSeconds(0.05f);
            }
            // พุ่งจริง           
            while (transform.position.y < startPosition.y) 
            {
                transform.position += Vector3.up * impactSpeed * Time.deltaTime;
                yield return null; // รอเฟรมถัดไป
                
            }
            transform.position = new Vector3(startPosition.x,startPosition.y + impactBounce,startPosition.z);
            // กลับลงมากระแทก
            yield return new WaitForSeconds(0.05f);

            // ล็อกตำแหน่งให้เป๊ะเมื่อเสร็จสิ้น
            transform.position = startPosition;
            isRising = false; // ผุดเสร็จ
        }
        
    

    
    
    void Update()

    {
        Debug.Log("isRising = " + isRising);
        //bool isTurning = false;
        
        // ระหว่างผุด ห้ามขยับ

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool("isJumping", true);
        }
                

    }
    void FixedUpdate()
    {
        if (isRising) return;

        Vector3 move = Vector3.right * speed;

        if (Input.GetKey(KeyCode.D))
            move += Vector3.back * horizontalSpeed;

        if (Input.GetKey(KeyCode.A))
            move += Vector3.forward * horizontalSpeed;

        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }


    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("isJumping", false);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            
        }
    }
    
}

