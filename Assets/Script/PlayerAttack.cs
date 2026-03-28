using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Battle Mana")]
    public int battleMana = 0;
    public int[] manaCosts;

    [Header("Persistent Coins (ใช้ซื้อของ)")]
    public int totalCoins = 0;

    [Header("Skill Cooldowns")]
    public float[] skillCooldowns;
    private float[] lastUsedTimes;

    // =========================
    // SKILL SYSTEM
    // =========================
    public RectTransform crosshair;
    public GameObject[] skillPrefabs;
    public Transform firePoint;
    public float projectileSpeed = 15f;
    
    [Header("Dash Skill")]
    public DashSkill dashSkill;

    [Header("Shield Skill")]
    public ShieldSkill shieldSkill;

    [Header("Spread Skill")]
    public int spreadCount = 5;
    public float spreadAngle = 30f;


    public GameObject targetIndicatorPrefab;

    private int currentSkill = -1;
    private Transform currentTarget;
    private GameObject currentIndicator;

    public Animator anim;

    private Renderer lastRenderer;
    private Color originalColor;

    void Start()
    {
        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        Debug.Log("Animator ที่ใช้งานอยู่: " + (anim != null ? anim.name : "❌ ไม่มี"));

        battleMana = 0;
        lastUsedTimes = new float[skillPrefabs.Length];

        currentIndicator = Instantiate(targetIndicatorPrefab);
        currentIndicator.SetActive(false);
    }

    void Update()
    {
        SelectSkill();   // เลือกสกิลตามปุ่ม
        DetectTarget();
        UpdateCrosshair();

        // 🎯 กดเมาส์เพื่อใช้สกิลอื่น ๆ (ยกเว้นโล่)
        if (Input.GetMouseButtonDown(0) && currentSkill != -1 && currentTarget != null)
        {
            // ยกเว้นสกิลโล่ index 2
            if (currentSkill != 2)
                TryCastSkill();
        }

        // ⚡ ใช้สกิลโล่ทันทีเมื่อกด E
        if (Input.GetKeyDown(KeyCode.E) && shieldSkill != null)
        {
            currentSkill = 2; // index โล่
            TryCastSkill();
        }
    }

    void UpdateCrosshair()
    {
        // แสดง crosshair เฉพาะสกิลที่ต้องมีเป้าหมาย (ไม่ใช่โล่)
            if (currentSkill != -1 && currentSkill != 2 && crosshair != null)
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
    // MANA / COINS
    // =========================
    public void AddMana(int amount)
    {
        if (amount <= 0) return;

        battleMana += amount;
        totalCoins += amount;

        Debug.Log("💰 เก็บเหรียญ: " + amount +
                  " | ด่าน: " + battleMana +
                  " | สะสม: " + totalCoins);
    }

    bool UseMana(int amount)
    {
        if (battleMana < amount)
        {
            Debug.Log("❌ เหรียญไม่พอ! ต้องใช้ " + amount);
            return false;
        }

        battleMana -= amount;

        Debug.Log("🎯 ใช้เหรียญ: " + amount +
                  " | เหลือ: " + battleMana +
                  " | สะสม: " + totalCoins);

        return true;
    }

    // =========================
    // SKILL CAST
    // =========================
    void TryCastSkill()
    {
        if (currentSkill == -1) return;

        if (currentSkill >= skillPrefabs.Length || currentSkill >= manaCosts.Length)
        {
            Debug.LogWarning("Skill ยังไม่มี!");
            return;
        }

        if (Time.time < lastUsedTimes[currentSkill] + skillCooldowns[currentSkill])
        {
            float remain = (lastUsedTimes[currentSkill] + skillCooldowns[currentSkill]) - Time.time;
            Debug.Log("⏳ CD: " + remain.ToString("F1"));
            return;
        }

        int cost = manaCosts[currentSkill];
        if (!UseMana(cost)) return;

        // 🎬 เล่น Animation
        if (anim != null)
            anim.SetTrigger("Attack");

        // ⚔ ใช้สกิล
        switch (currentSkill)
        {
            case 0: // ปกติ
                CastSkill();
                break;
            case 1: // Dash
                if (dashSkill != null)
                    dashSkill.StartDash(currentTarget, () => Debug.Log("Dash เสร็จ"));
                break;
            case 2: // โล่
                if (shieldSkill != null)
                    shieldSkill.ActivateShield();
                break;
            case 3: // Spread
                CastSpreadSkill();
                break;
        }

        lastUsedTimes[currentSkill] = Time.time;

        // ✅ ใช้แล้วให้ crosshair หายไป
        if (crosshair != null)
            crosshair.gameObject.SetActive(false);

        // รีเซ็ต currentSkill หลังใช้
        currentSkill = -1;
    }
    void SelectSkill()
    {
        if (Input.GetKeyDown(KeyCode.Q)) currentSkill = 0;
        else if (Input.GetKeyDown(KeyCode.W)) currentSkill = 1;
        //else if (Input.GetKeyDown(KeyCode.E)) currentSkill = 2; ห้ามเอาคอมเมนท์ออก 
        else if (Input.GetKeyDown(KeyCode.R)) currentSkill = 3;
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

                Vector3 pos = currentTarget.position;
                pos.y = hit.collider.bounds.min.y + 0.02f;
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
        GameObject proj = Instantiate(skillPrefabs[currentSkill], firePoint.position, Quaternion.identity);

        Vector3 dir = (currentTarget.position - firePoint.position).normalized;
        proj.transform.rotation = Quaternion.LookRotation(dir);

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = dir * projectileSpeed;
    }

    void CastSpreadSkill()
    {
        Vector3 baseDir = (currentTarget.position - firePoint.position).normalized;

        float step = spreadAngle / (spreadCount - 1);
        float start = -spreadAngle / 2f;

        for (int i = 0; i < spreadCount; i++)
        {
            float angle = start + step * i;
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * baseDir;

            GameObject proj = Instantiate(skillPrefabs[currentSkill], firePoint.position, Quaternion.LookRotation(dir));

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = dir * projectileSpeed;
        }
    }
}
