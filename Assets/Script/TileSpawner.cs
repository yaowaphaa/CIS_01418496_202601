using UnityEngine;
using System.Collections.Generic;
public class TileSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs; // สิ่งของที่จะสุ่มมาวาง
    public Transform[] mySpawnPoints;    // จุดเกิดที่อยู่ใน Prefab ชิ้นนี้
    [Range(0, 1)] public float spawnChance = 0.5f; // โอกาสที่จะเกิด (0-100%)

    void Start()
    {
        // สั่งให้สุ่มวางของทันทีที่แมพชิ้นนี้ถูกสร้าง
        SpawnObstacles();
        foreach (Transform point in mySpawnPoints)
        {
            Debug.Log("Spawning at: " + point.position + " | Name: " + point.name);
        }
    }

   void SpawnObstacles()
{
    // 1. สร้าง List ชั่วคราวเพื่อเก็บจุดเกิดที่มีทั้งหมด
    List<Transform> availablePoints = new List<Transform>(mySpawnPoints);

    // 2. กำหนดจำนวนสูงสุดที่จะให้เกิด (ไม่ให้เกินจำนวนจุดที่มี)
    int maxToSpawn = Mathf.Min(mySpawnPoints.Length, 10); // สมมติอยากให้เกิดมากสุดแค่ 10 อัน

    for (int i = 0; i < maxToSpawn; i++)
    {
        if (availablePoints.Count == 0) break;

        // สุ่มว่า "รอบนี้" จะให้ของเกิดไหม (ใช้โอกาส % เหมือนเดิม)
        if (Random.value < spawnChance)
        {
            // 3. สุ่มเลือกจุดจาก List ที่เหลืออยู่
            int randomIndex = Random.Range(0, availablePoints.Count);
            Transform selectedPoint = availablePoints[randomIndex];

            int prefabIndex = Random.Range(0, obstaclePrefabs.Length);
            GameObject selectedPrefab = obstaclePrefabs[prefabIndex];

            // 4. สร้างและตั้งค่า (ล็อก Y, Rotation, Scale ตาม Prefab)
            GameObject obj = Instantiate(selectedPrefab);
            
            obj.transform.rotation = selectedPrefab.transform.rotation;
            obj.transform.localScale = selectedPrefab.transform.localScale;

            // ล็อก Y ตาม Prefab, XZ ตามจุดเกิด
            float spawnY = selectedPrefab.transform.position.y;
            obj.transform.position = new Vector3(selectedPoint.position.x, spawnY, selectedPoint.position.z);

            // เข้าเป็นลูกของแม่ (รักษาค่าที่ตั้งไว้)
            obj.transform.SetParent(this.transform, true);

            // 5. !!! สำคัญที่สุด: ลบจุดนี้ออกจากรายการ เพื่อไม่ให้สุ่มได้จุดเดิมอีก !!!
            availablePoints.RemoveAt(randomIndex);
        }
    }
}
}