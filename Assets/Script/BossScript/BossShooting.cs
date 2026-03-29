using UnityEngine;
using System.Collections; // ต้องมีบรรทัดนี้เพื่อใช้ IEnumerator

public class BossShooting : MonoBehaviour
{
    public GameObject crystalPrefab; 
    public Transform firePoint;      
    public float minShootDelay = 2f; 
    public float maxShootDelay = 5f;
    public float startDelay = 10f;
    
    [Header("Animation Settings")]
    public Animator anim;
    public string attackTriggerName = "Attack"; // ชื่อ Trigger ใน Animator
    public float delayBeforeSpawn = 0.5f;      // ระยะเวลาที่รอให้ท่าทางอนิเมชันถึงจุดยิง

    public PlayerBossMovement player; 
    private float nextShootTime;
    private bool hasStarted = false;
    private bool isAttacking = false; // กันไม่ให้สั่งยิงซ้ำซ้อนขณะรออนิเมชัน

    void Start()
    {   
        SetNextShootTime();
    }

    void Update()
    {
        if (player != null && player.isIntroPlaying) return;
        if (isAttacking) return; // ถ้ากำลังอยู่ในคิวโจมตี ให้รอตัวแปรนี้ปลดล็อค

        if (!hasStarted)
        {
            nextShootTime = Time.time + startDelay;
            hasStarted = true;
        }

        if (Time.time >= nextShootTime)
        {   
            // เปลี่ยนจากเรียก Shoot() ตรงๆ เป็นเรียก Coroutine แทน
            StartCoroutine(AttackSequence());
            SetNextShootTime();
        }
    }

    IEnumerator AttackSequence()
    {
        isAttacking = true;

        // 1. สั่งเล่น Animation
        if (anim != null)
        {
            anim.SetTrigger(attackTriggerName);
        }

        // 2. รอเวลาให้ท่าทางอนิเมชันขยับไปถึงจังหวะที่จะปล่อยพลัง
        yield return new WaitForSeconds(delayBeforeSpawn);

        // 3. ปล่อยกระสุน
        PerformShoot();

        isAttacking = false;
    }

    void SetNextShootTime()
    {
        nextShootTime = Time.time + Random.Range(minShootDelay, maxShootDelay);
    }

    // แยก Logic การสร้างกระสุนออกมา
    void PerformShoot()
    {
        if (crystalPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(crystalPrefab, firePoint.position, firePoint.rotation);

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                bullet.transform.LookAt(playerObj.transform.position);
            }
        }
    }
}