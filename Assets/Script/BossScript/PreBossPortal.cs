using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PreBossPortal : MonoBehaviour
{
    [Header("--- Settings ---")]
    public float timeToWait = 10f;
    public float distanceInFront = 15f;
    public float moveDuration = 3f;
    public float moveSpeed = 5f;
    public float scaleSpeed = 5f;

    [Header("--- References ---")]
    public GameObject door;
    public string nextScene;

    private Vector3 savedTargetScale;
    private Quaternion savedRotation;
    private bool isSpawned = false;

    void Awake() // เก็บค่าให้ไวที่สุดก่อนโดน Reset
    {
        if (door != null)
        {
            // 1. จำค่าที่ปั้นไว้ใน Editor จริงๆ
            savedTargetScale = door.transform.localScale;
            savedRotation = door.transform.rotation;

            // ปิดและซ่อน
            door.transform.localScale = Vector3.zero;
            door.SetActive(false);
        }
    }

    void Start()
    {
        StartCoroutine(SpawnPortalRoutine());
    }

    IEnumerator SpawnPortalRoutine()
    {
        yield return new WaitForSeconds(timeToWait);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null && door != null)
        {
            // 2. คำนวณจุดเกิด "แค่ครั้งเดียว" ตอนเริ่ม (ดักหน้าผู้เล่น ณ วินาทีนั้น)
            Vector3 spawnPos = player.transform.position + (player.transform.forward * distanceInFront);
            
            // ล็อกแกน Y ให้เท่ากับระดับพื้นเดิมหรือระดับบอส (เลือกเอาตามต้องการ)
            // ในที่นี้ล็อกตามจุดที่คำนวณได้ครั้งแรก
            float lockY = spawnPos.y;
            float lockZ = spawnPos.z;
            float startX = spawnPos.x;

            // เซตตำแหน่งและองศาให้เป๊ะตาม Editor
            door.transform.position = spawnPos;
            door.transform.rotation = savedRotation; 

            door.SetActive(true);
            isSpawned = true;

            float elapsed = 0;
            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                
                // 3. เลื่อนเฉพาะแกน X จากจุดเริ่มต้นที่คำนวณไว้
                float currentX = startX + (elapsed * moveSpeed);
                
                // ใช้การสร้างพิกัดใหม่ทับลงไปที่ตัวประตูโดยตรง
                door.transform.position = new Vector3(currentX, lockY, lockZ);

                // 4. ค่อยๆ ขยายร่าง
                door.transform.localScale = Vector3.MoveTowards(door.transform.localScale, savedTargetScale, scaleSpeed * Time.deltaTime);

                yield return null;
            }

            // ปิดงาน: บังคับค่าสุดท้ายให้เป๊ะ
            door.transform.localScale = savedTargetScale;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isSpawned && other.CompareTag("Player"))
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}