using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // =========================
    // 💰 BATTLE MANA (เหรียญในด่าน)
    // =========================
    [Header("Battle Mana")]
    public int battleMana = 0;       // เหรียญใช้ยิงสกิลในด่าน
    public int[] manaCosts;          // ใส่ 3 ค่าใน Inspector

    [Header("Persistent Coins (ใช้ซื้อของ)")]
    public int totalCoins = 0;       // เหรียญจริงสะสมข้ามด่าน

    [Header("Skill Cooldowns")]
    public float[] skillCooldowns;   // ใส่ 3 ค่าใน Inspector
    private float[] lastUsedTimes;   // เก็บเวลาใช้ล่าสุด

    // =========================
    // ⚔ SKILL SYSTEM
    // =========================
    public RectTransform crosshair;
    public GameObject[] skillPrefabs;
    public Transform firePoint;
    public float projectileSpeed = 15f;

    [Header("Spread Skill")]
    public int spreadCount = 5;        
    public float spreadAngle = 30f;

    [Header("Dash Skill")]
    public DashSkill dashSkill;

    public GameObject targetIndicatorPrefab;

    private int currentSkill = -1; 
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
        UpdateCrosshair();

        if (Input.GetMouseButtonDown(0) && currentTarget != null)
        {
            TryCastSkill();
        }
    }

    void UpdateCrosshair()
    {
        if (currentSkill != -1 && crosshair != null)
        {
            crosshair.gameObject.SetActive(true);
            crosshair.position = Input.mousePosition;
        }
        else if (crosshair != null)
        {
            crosshair.gameObject.SetActive(false);
        }
    }

    // =========================
    // 💰 MANA / COINS FUNCTIONS
    // =========================
    public void AddMana(int amount)
    {
        if (amount <= 0) return;

        battleMana += amount;       // เพิ่ม mana ในด่าน
        totalCoins += amount;       // เพิ่มเหรียญสะสมจริง

        Debug.Log("💰 เก็บเหรียญ: " + amount + 
                  " | เหรียญในด่าน: " + battleMana + 
                  " | เหรียญสะสมจริง: " + totalCoins);
    }

    bool UseMana(int amount)
    {
        if (battleMana < amount)
        {
            Debug.Log("❌ เหรียญไม่พอ! ต้องใช้ " + amount + 
                      " | เหรียญในด่าน: " + battleMana);
            return false;
        }

        battleMana -= amount;
        Debug.Log("🎯 ใช้เหรียญในด่าน: " + amount + 
                  " | เหรียญในด่านคงเหลือ: " + battleMana + 
                  " | เหรียญสะสมจริง: " + totalCoins);
        return true;
    }

    // =========================
    // ⚔ SKILL CASTING
    // =========================
    void TryCastSkill()
    {
        if (currentSkill == -1)
        {
            Debug.Log("⚠ ยังไม่ได้เลือกสกิล!");
            return;
        }

        if (currentSkill >= skillPrefabs.Length || currentSkill >= manaCosts.Length)
        {
            Debug.LogWarning("Skill ยังไม่มี!");
            return;
        }

        if (Time.time < lastUsedTimes[currentSkill] + skillCooldowns[currentSkill])
        {
            float remain = (lastUsedTimes[currentSkill] + skillCooldowns[currentSkill]) - Time.time;
            Debug.Log("⏳ คูลดาวน์เหลือ: " + remain.ToString("F1") + " วิ");
            return;
        }

        int cost = manaCosts[currentSkill];
        if (!UseMana(cost)) return;

        // ยิงสกิลตามประเภท
        if (currentSkill == 0) CastSkill();
        else if (currentSkill == 1) CastSpreadSkill();
        else if (currentSkill == 2 && currentTarget != null && dashSkill != null)
        {
            dashSkill.StartDash(currentTarget, () => { Debug.Log("Dash จบแล้ว"); });
        }

        lastUsedTimes[currentSkill] = Time.time;

        if (crosshair != null) crosshair.gameObject.SetActive(false);
        currentSkill = -1;
        Debug.Log("✅ ต้องเลือกสกิลใหม่ก่อนยิงอีกครั้ง");
    }

    void SelectSkill()
    {
        if (Input.GetKeyDown(KeyCode.Q) && skillPrefabs.Length > 0) currentSkill = 0;
        else if (Input.GetKeyDown(KeyCode.W) && skillPrefabs.Length > 1) currentSkill = 1;
        else if (Input.GetKeyDown(KeyCode.E) && skillPrefabs.Length > 2) currentSkill = 2;
    }

    void DetectTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                currentTarget = hit.collider.transform;

                currentIndicator.SetActive(true);
                Collider col = hit.collider;
                Vector3 pos = currentTarget.position;
                pos.y = col.bounds.min.y + 0.02f;
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
        GameObject projectile = Instantiate(skillPrefabs[currentSkill], firePoint.position, Quaternion.identity);
        Vector3 direction = (currentTarget.position - firePoint.position).normalized;
        projectile.transform.rotation = Quaternion.LookRotation(direction);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = direction * projectileSpeed;

        Debug.Log("💰 Mana เหลือ: " + battleMana);
    }

    void CastSpreadSkill()
    {
        Vector3 baseDir = (currentTarget.position - firePoint.position).normalized;
        float angleStep = spreadAngle / (spreadCount - 1);
        float startAngle = -spreadAngle / 2f;

        for (int i = 0; i < spreadCount; i++)
        {
            float angle = startAngle + (angleStep * i);
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 dir = rotation * baseDir;

            GameObject projectile = Instantiate(skillPrefabs[currentSkill], firePoint.position, Quaternion.LookRotation(dir));
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = dir * projectileSpeed;
        }

        Debug.Log("💰 Mana เหลือ: " + battleMana);
    }
}