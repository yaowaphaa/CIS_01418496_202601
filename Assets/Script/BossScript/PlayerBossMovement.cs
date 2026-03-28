using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody))]
public class PlayerBossMovement : MonoBehaviour
{
   
    public float speed = 5f;            
    public float horizontalSpeed = 4f;  
    public float jumpForce = 12f;       
    public float fallMultiplier = 2.5f; 

    
    public Animator childAnim;
    private Rigidbody rb;

   
    public bool isGrounded;
    public bool isLanded = true;   
    public bool isIntroPlaying = false; 

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (childAnim == null) childAnim = GetComponentInChildren<Animator>();

        // ล็อคไม่ให้ตัวละครล้ม
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (isIntroPlaying) return;

        // ระบบกระโดด
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("กด Space! สถานะพื้น: " + isGrounded);
            if (isGrounded)
            {
                Jump();
            }
        }
    }

    void FixedUpdate()
    {
        if (isIntroPlaying) return;
        Vector3 move = Vector3.right * speed;
        if (Input.GetKey(KeyCode.D)) move += Vector3.back * horizontalSpeed;
        if (Input.GetKey(KeyCode.A)) move += Vector3.forward * horizontalSpeed;

        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    public void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (childAnim != null)
        {
            childAnim.SetBool("IsJumpping", true);
        }
        isGrounded = false;
    }

    void OnCollisionEnter(Collision collision)
{

    if (collision.gameObject.CompareTag("Ground"))
    {
        isGrounded = true;
  
        if (!isLanded)
        {
            isLanded = true; 
            
            if (childAnim != null)
            {
                childAnim.SetBool("IsFallingIdle", false);
                childAnim.SetTrigger("Landing");
            }
        }

        if (childAnim != null) childAnim.SetBool("IsJumpping", false);
    }
}

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = false;
    }
}