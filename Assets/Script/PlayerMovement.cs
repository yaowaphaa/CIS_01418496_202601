using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 1f;
    public float horizontalSpeed = 3f;
    public float jumpForce = 5f;
    public float fallMultiplier = 2.5f;

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;

    private PlayerRise rise;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        rise = GetComponent<PlayerRise>();
    }

    void Update()
    {
        if (rise.IsRising) return;

        // กระโดด
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (rise.IsRising) return;

        // วิ่งไปข้างหน้าอัตโนมัติ
        Vector3 move = Vector3.right * speed;

        // ซ้ายขวา
        if (Input.GetKey(KeyCode.D))
            move += Vector3.back * horizontalSpeed;

        if (Input.GetKey(KeyCode.A))
            move += Vector3.forward * horizontalSpeed;

        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        // เพิ่มแรงตก
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y *
                                 (fallMultiplier - 1) *
                                 Time.fixedDeltaTime;
        }
    }

    public void Jump()
    {
        if (!isGrounded) return;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animator.SetBool("isJumping", true);
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