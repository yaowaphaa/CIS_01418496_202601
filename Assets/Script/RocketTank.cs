using UnityEngine;

public class RocketTank : MonoBehaviour
{
    [Header("Detection")]
    public float detectRange = 12f;
    public string playerTag = "Player";

    [Header("Rocket Physics")]
    public float launchSpeed = 35f;  // เพิ่มความเร็วให้สะใจ
    public Vector3 rocketAxis = Vector3.up; // ปกติถังแก๊สจะพุ่งตามแนวตั้ง (Y)

    private Transform player;
    private bool isLaunched = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameObject p = GameObject.FindGameObjectWithTag(playerTag);
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null || isLaunched) return;

        if (Vector3.Distance(transform.position, player.position) <= detectRange)
        {
            LaunchNow();
        }
    }

    void LaunchNow()
    {
        isLaunched = true;

        // 1. หันหัวไปหาผู้เล่น "ทันที" (ไม่รอหมุน)
        Vector3 dir = (player.position - transform.position).normalized;

        // ใช้ LookRotation เพื่อให้แกน 'rocketAxis' ชี้ไปหาผู้เล่น
        transform.rotation = Quaternion.FromToRotation(rocketAxis, dir) * transform.rotation;

        // 2. ให้ Rigidbody ทำงานทันที
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
            // ใส่แรงถีบมหาศาลแบบ Impulse (พุ่งพรวดเดียว!)
            rb.AddForce(dir * launchSpeed, ForceMode.VelocityChange);
        }

        // 3. ทำลายตัวเองถ้าพลาดเป้า (กันรกฉาก)
        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            HealthSystem health = collision.gameObject.GetComponent<HealthSystem>();
            if (health != null) health.TakeDamage(1);

            Debug.Log("🚀 ตู้มมม!");
            Destroy(gameObject);
        }
    }
}
