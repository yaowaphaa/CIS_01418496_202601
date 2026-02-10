using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 1f;
    public float horizontalSpeed = 3f;
    public int impactCount = 3;          // จำนวนครั้งที่กระแทก
    public float preImpactBounce = 0.2f; // ความแรงของการกระแทกใต้ดิน
    public float preImpactSpeed = 5f;   // ความเร็วการกระแทก
    public float impactBounce = 0.15f; // ความเด้งตอนกระแทก
    public float impactSpeed = 12f;    // ความแรงตอนพุ่ง
    public float undergroundY = -1f; // ความลึกใต้ดินเริ่มต้น
    private Vector3 startPosition;
    private bool isRising = true;

    void Start()
    {
         // 1. บันทึกตำแหน่งที่ควรจะเป็น (ตำแหน่งที่คุณวางไว้ใน Editor)
        startPosition = transform.position;

        // 2. ย้ายวัตถุไปไว้ใต้ดินทันทีเมื่อเริ่ม
        transform.position = new Vector3(startPosition.x, undergroundY, startPosition.z);

        // 3. เริ่มการทำงานให้ค่อยๆ ผุดขึ้นมา
        StartCoroutine(RiseUp());

    
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
    }

    
    
    void Update()

    {
        


        
        //bool isTurning = false;
        
        // ระหว่างผุด ห้ามขยับ
        if (isRising) return;

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D))
        {
            
            transform.Translate(Vector3.right * horizontalSpeed * Time.deltaTime);
           
        }
        if (Input.GetKey(KeyCode.A))
        {
           
            transform.Translate(Vector3.left * horizontalSpeed * Time.deltaTime  );
           
            }
     

    }
}
