using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;          // ตัวผู้เล่น
    public float minDistanceFromPlayer = 10f; // ห้าม spawn ใกล้กว่านี้
    public int minEnemies = 20;
    public int maxEnemies = 50;
    public float minDistanceBetweenEnemies = 2f; // ระยะห่าง XZ
    public float spawnDelay = 0.05f;             
    public int maxAttemptsPerEnemy = 500; 
    public float startDistance = 6f;     // ระยะห่างช่วงเริ่มเกม
    public float normalDistance = 2f;    // ระยะปกติ (ค่าเดิม)
    public float distanceLerpTime = 5f;  // เวลาค่อย ๆ ลดระยะ    

    private List<Vector3> spawnedPositions = new List<Vector3>();
    private GameObject[] grounds;
    private int totalEnemies;

    void Start()
    {
        // หา Ground ทุกตัวที่มี Tag = "Ground" และมี Collider
        GameObject[] allGrounds = GameObject.FindGameObjectsWithTag("Ground");
        List<GameObject> validGrounds = new List<GameObject>();
        foreach (var g in allGrounds)
        {
            if (g.GetComponent<Collider>() != null) 
                validGrounds.Add(g);
        }
        grounds = validGrounds.ToArray();

        if (grounds.Length == 0)
        {
            Debug.LogWarning("⚠ ไม่มี Ground ที่มี Collider ในซีน!");
            return;
        }

        totalEnemies = Random.Range(minEnemies, maxEnemies + 1);
        minDistanceBetweenEnemies = startDistance;
        StartCoroutine(SpawnEnemies());
        StartCoroutine(ReduceDistanceOverTime());
    }

    IEnumerator SpawnEnemies()
    {
        int spawnedCount = 0;

        while (spawnedCount < totalEnemies)
        {
            Vector3 spawnPos = GetRandomPositionOnGround();
            if (spawnPos != Vector3.zero)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

                // ปรับ Y ของศัตรูให้ชนพื้น
                Collider col = enemy.GetComponent<Collider>();
                if (col != null)
                {
                    Vector3 pos = enemy.transform.position;
                    pos.y += col.bounds.extents.y;
                    enemy.transform.position = pos;
                }

                spawnedPositions.Add(spawnPos);
                spawnedCount++;
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    Vector3 GetRandomPositionOnGround()
    {
        int attempts = 0;
        while (attempts < maxAttemptsPerEnemy)
        {
            GameObject ground = grounds[Random.Range(0, grounds.Length)];
            Collider groundCol = ground.GetComponent<Collider>();
            if (groundCol == null) { attempts++; continue; }

            // ใช้ bounds ของ Collider แทน Renderer
            Vector3 min = groundCol.bounds.min;
            Vector3 max = groundCol.bounds.max;

            float x = Random.Range(min.x, max.x);
            float z = Random.Range(min.z, max.z);
            Vector3 spawnPos = new Vector3(x, max.y + 5f, z);

            // Raycast ลงพื้นเพื่อหาตำแหน่ง Y
            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, 50f))
                spawnPos.y = hit.point.y;
            else { attempts++; continue; }

           bool tooClose = false;

            // 🔹 เช็คห่างจาก enemy ตัวอื่น
            foreach (var pos in spawnedPositions)
            {
                if (Vector2.Distance(new Vector2(pos.x, pos.z), new Vector2(spawnPos.x, spawnPos.z)) < minDistanceBetweenEnemies)
                {
                    tooClose = true;
                    break;
                }
            }

            // 🔹 เช็คห่างจาก player
            if (!tooClose && player != null)
            {
                float distToPlayer = Vector2.Distance(
                    new Vector2(player.position.x, player.position.z),
                    new Vector2(spawnPos.x, spawnPos.z)
                );

                if (distToPlayer < minDistanceFromPlayer)
                {
                    tooClose = true;
                }
            }

            // 🔹 ถ้าผ่านทุกเงื่อนไข
            if (!tooClose) return spawnPos;

            attempts++;

            if (player == null)
            {
                Debug.LogWarning("⚠ ยังไม่ได้ใส่ Player!");
            }
        }

        // fallback: ถ้าไม่เจอที่เหมาะสม
        GameObject fallbackGround = grounds[Random.Range(0, grounds.Length)];
        Collider fallbackCol = fallbackGround.GetComponent<Collider>();
        Vector3 fallbackMin = fallbackCol.bounds.min;
        Vector3 fallbackMax = fallbackCol.bounds.max;
        float fx = Random.Range(fallbackMin.x, fallbackMax.x);
        float fz = Random.Range(fallbackMin.z, fallbackMax.z);
        Vector3 fallbackPos = new Vector3(fx, fallbackMax.y + 5f, fz);
        if (Physics.Raycast(fallbackPos, Vector3.down, out RaycastHit fhit, 50f))
            fallbackPos.y = fhit.point.y;
        return fallbackPos;
    }
    IEnumerator ReduceDistanceOverTime()
    {
        yield return new WaitForSeconds(1.5f); // รอให้ spawn ช่วงแรกกระจายก่อน

        float time = 0f;

        while (time < distanceLerpTime)
        {
            minDistanceBetweenEnemies = Mathf.Lerp(startDistance, normalDistance, time / distanceLerpTime);
            time += Time.deltaTime;
            yield return null;
        }

        minDistanceBetweenEnemies = normalDistance;
    }
}