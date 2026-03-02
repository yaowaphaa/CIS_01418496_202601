using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // =========================
    // 💰 BATTLE MANA (เหรียญในด่าน)
    // =========================

    [Header("Battle Mana")]
    public int battleMana = 0;       // เหรียญใช้ยิง
    public int[] manaCosts;          // ใส่ 3 ค่าใน Inspector

    [Header("Skill Cooldowns")]
    public float[] skillCooldowns;   // ใส่ 3 ค่าใน Inspector
    private float[] lastUsedTimes;   // เก็บเวลาใช้ล่าสุด

    // =========================
    // ⚔ SKILL SYSTEM
    // =========================

    public GameObject[] skillPrefabs;
    public Transform firePoint;
    public float projectileSpeed = 15f;

    [Header("Spread Skill")]
    public int spreadCount = 5;        // จำนวนกระสุน
    public float spreadAngle = 30f;    // มุมกระจายรวม


    public GameObject targetIndicatorPrefab;

    private int currentSkill = -1; // -1 = ยังไม่ได้เลือก
    private Transform currentTarget;
    private GameObject currentIndicator;

    private Renderer lastRenderer;
    private Color originalColor;

    void Start()
    {
        battleMana = 0;   // รีเซ็ตตอนเริ่มด่าน
        lastUsedTimes = new float[skillPrefabs.Length];
        currentIndicator = Instantiate(targetIndicatorPrefab);
        currentIndicator.SetActive(false);
    }

    void Update()
    {
        SelectSkill();
        DetectTarget();

        if (Input.GetMouseButtonDown(0) && currentTarget != null)
        {
            TryCastSkill();
        }
    }

    // =========================
    // 💰 MANA FUNCTIONS
    // =========================

    public void AddMana(int amount)
    {
        battleMana += amount;
        Debug.Log("💰 Mana ในด่าน: " + battleMana);
    }

    bool UseMana(int amount)
    {
        if (battleMana < amount)
        {
            Debug.Log("❌ เหรียญไม่พอ!");
            return false;
        }

        battleMana -= amount;
        return true;
    }

    void TryCastSkill()
    {
         Debug.Log("พยายามยิง");
         if (currentSkill == -1)
            {
                Debug.Log("⚠ ยังไม่ได้เลือกสกิล!");
                return;
            }

        if (currentSkill >= skillPrefabs.Length ||
            currentSkill >= manaCosts.Length)
        {
            Debug.LogWarning("Skill ยังไม่มี!");
            return;
        }
        // 🔥 เช็คคูลดาวน์
        if (Time.time < lastUsedTimes[currentSkill] + skillCooldowns[currentSkill])
        {
            float remain = 
                (lastUsedTimes[currentSkill] + skillCooldowns[currentSkill]) - Time.time;

            Debug.Log("⏳ คูลดาวน์เหลือ: " + remain.ToString("F1") + " วิ");
            return;
        }

        int cost = manaCosts[currentSkill];

        if (!UseMana(cost))
            return;

        // ยิงตามหมายเลขสกิล
        if (currentSkill == 0)
        {
            CastSkill();        // สกิล 1 ยิงเดี่ยว
        }
        else if (currentSkill == 1)
        {
            CastSpreadSkill();  // สกิล 2 ดาวกระจาย
        }
        
        lastUsedTimes[currentSkill] = Time.time;
    }
    
    // =========================
    // ⚔ SKILL FUNCTIONS
    // =========================

    void SelectSkill()
    {
        if (Input.GetKeyDown(KeyCode.Q) && skillPrefabs.Length > 0)
            {currentSkill = 0;
            Debug.Log("เลือกสกิล 1");}

        else  if (Input.GetKeyDown(KeyCode.W) && skillPrefabs.Length > 1)
            {currentSkill = 1;
            Debug.Log("เลือกสกิล 2");}

        else  if (Input.GetKeyDown(KeyCode.E) && skillPrefabs.Length > 2)
            {currentSkill = 2;
            Debug.Log("เลือกสกิล 3");}
    }

    void DetectTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                currentTarget = hit.collider.transform;

                currentIndicator.SetActive(true);
                Collider col = hit.collider;
                Vector3 pos = currentTarget.position;
                pos.y = col.bounds.min.y + 0.02f; // ต่ำสุดของตัวมอน + กันจมพื้น
                currentIndicator.transform.position = pos;

                Renderer rend = hit.collider.GetComponent<Renderer>();
                if (rend != null && rend != lastRenderer)
                {
                    ResetLastColor();
                    lastRenderer = rend;
                    originalColor = rend.material.color;
                    rend.material.color = Color.red;
                }

                return;
            }
        }

        HideIndicator();
    }

    void HideIndicator()
    {
        currentTarget = null;
        currentIndicator.SetActive(false);
        ResetLastColor();
    }

    void ResetLastColor()
    {
        if (lastRenderer != null)
        {
            lastRenderer.material.color = originalColor;
            lastRenderer = null;
        }
    }

    void CastSkill()
    {
        GameObject projectile = Instantiate(
            skillPrefabs[currentSkill],
            firePoint.position,
            Quaternion.identity
        );

        Vector3 direction = (currentTarget.position - firePoint.position).normalized;

        projectile.transform.rotation = Quaternion.LookRotation(direction);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        Debug.Log("💰 Mana เหลือ: " + battleMana);
    }
    void CastSpreadSkill()
    {
        Vector3 baseDirection = 
            (currentTarget.position - firePoint.position).normalized;

        float angleStep = spreadAngle / (spreadCount - 1);
        float startAngle = -spreadAngle / 2f;

        for (int i = 0; i < spreadCount; i++)
        {
            float angle = startAngle + (angleStep * i);

            Quaternion rotation = 
                Quaternion.AngleAxis(angle, Vector3.up);

            Vector3 direction = rotation * baseDirection;

            GameObject projectile = Instantiate(
                skillPrefabs[currentSkill],
                firePoint.position,
                Quaternion.LookRotation(direction)
            );

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = direction * projectileSpeed;
            }
        }
        Debug.Log("💰 Mana เหลือ: " + battleMana);
    }
    ฆ
}