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

    [Header("Spread Skill")]
    public int spreadCount = 5;
    public float spreadAngle = 30f;

    [Header("Dash Skill")]
    public DashSkill dashSkill;

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
        SelectSkill();
        DetectTarget();
        UpdateCrosshair();

        if (Input.GetMouseButtonDown(0) && currentSkill != -1 && currentTarget != null)
        {
            TryCastSkill();
        }
    }

    void UpdateCrosshair()
    {
        if (currentSkill != -1 && currentTarget != null && crosshair != null)
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
        if (currentSkill == 0)
        {
            CastSkill();
        }
        else if (currentSkill == 1)
        {
            CastSpreadSkill();
        }
        else if (currentSkill == 2 && dashSkill != null)
        {
            dashSkill.StartDash(currentTarget, () =>
            {
                Debug.Log("Dash เสร็จ");
            });
        }

        lastUsedTimes[currentSkill] = Time.time;

        if (crosshair != null)
            crosshair.gameObject.SetActive(false);

        currentSkill = -1;
    }

    void SelectSkill()
    {
        if (Input.GetKeyDown(KeyCode.Q)) currentSkill = 0;
        else if (Input.GetKeyDown(KeyCode.W)) currentSkill = 1;
        else if (Input.GetKeyDown(KeyCode.E)) currentSkill = 2;
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