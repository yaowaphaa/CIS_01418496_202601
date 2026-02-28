using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Transform model;

    public bool CanMove { get; private set; }
    public bool MoveLeft { get; private set; }
    public bool MoveRight { get; private set; }

    private bool hasTurned = false;
    private PlayerMovement movement;
    private PlayerRise rise;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        rise = GetComponent<PlayerRise>();
    }

    void Update()
    {
        if (rise.IsRising)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!hasTurned)
            {
                model.localRotation = Quaternion.Euler(0f, 90f, 0f);
                hasTurned = true;
            }

            CanMove = true;
        }

        if (!CanMove) return;

        MoveLeft = Input.GetKey(KeyCode.A);
        MoveRight = Input.GetKey(KeyCode.D);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            movement.Jump();
        }
    }
}

//🎯 วิธีใช้ (สำคัญมาก)

//ใส่ 3 Script นี้ลง GameObject Player ตัวเดียวกัน

//ต้องมี

//Rigidbody

//Animator

//ตั้ง Tag พื้นเป็น "Ground"

//ลาก Model ไปใส่ในช่อง model ของ PlayerInput