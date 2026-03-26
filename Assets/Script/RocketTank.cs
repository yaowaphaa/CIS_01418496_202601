using UnityEngine;

public class RocketGasTank : MonoBehaviour
{
    [Header("Detection")]
    public float detectRange = 10f;
    public string playerTag = "Player";

    [Header("Rocket Physics")]
    public float launchSpeed = 20f; // ความเร็วตอนพุ่ง
    public float rotateSpeed = 15f; // ความเร็วในการหันก้นไปหาผู้เล่น (ก่อนพุ่ง)

    private Transform playerTransform;
    private bool isLaunched = false;
    private Vector3 launchDirection;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null) playerTransform = player.transform;
    }

    void Update()
    {
        if (playerTransform == null || isLaunched) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // 1. ตรวจจับระยะ
        if (distance <= detectRange)
        {
            StartCoroutine(LaunchSequence());
        }
    }

    System.Collections.IEnumerator LaunchSequence()
    {
        isLaunched = true;

        // 2. คำนวณทิศทาง: หาทางที่จะให้ "ท้าย (-Y)" ชี้ไปหาผู้เล่น
        // นั่นแปลว่าเราต้องให้ "หัว (+Y)" ชี้ "หนี" จากผู้เล่น
        Vector3 dirAwayFromPlayer = (transform.position - playerTransform.position).normalized;

        // ล็อคเป้าหมายสุดท้ายที่จะพุ่งไป (ทิศที่ก้นชี้ไปหาผู้เล่นตอนนั้น)
        launchDirection = -dirAwayFromPlayer;

        // 3. หมุนตัวถังให้หัว (แกน Y) หันหนีผู้เล่น (เพื่อให้ท้ายพุ่งใส่)
        float elapsed = 0;
        Quaternion targetRotation = Quaternion.LookRotation(dirAwayFromPlayer, Vector3.up);
        // เนื่องจากปกติ LookRotation ใช้แกน Z เราต้องปรับให้ใช้แกน Y แทน
        targetRotation *= Quaternion.Euler(90, 0, 0);

        while (elapsed < 0.5f) // ใช้เวลา 0.5 วินาทีในการหันก้นมาหา
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, elapsed / 0.5f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 4. พุ่ง! (ใช้ Rigidbody เพื่อให้มีแรงกระแทก)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false; // พุ่งแบบจรวดไม่สนโลก
            rb.AddForce(launchDirection * launchSpeed, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            // ลดแต้ม hpoint
            PlayerStats stats = collision.gameObject.GetComponent<PlayerStats>();
            if (stats != null) stats.DecreaseHPoint(1);

            // ใส่เอฟเฟกต์ระเบิดตรงนี้ได้
            Debug.Log("ถังแก๊สอัดหน้าผู้เล่น!");
            Destroy(gameObject);
        }
    }
}
