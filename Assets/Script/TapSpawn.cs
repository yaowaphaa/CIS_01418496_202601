using UnityEngine;

public class TrapSpawner : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject objectToSpawn;    // ลาก Prefab ของที่จะให้หล่นมาใส่ (เช่น หิน, ถังไม้)
    public Transform spawnPoint;        // ลาก Empty Object ที่วางไว้บนฟ้ามาใส่

    [Header("Spawn Settings")]
    public float fallForce = 5f;        // แรงถีบให้หล่นเร็วขึ้น (ถ้าไม่อยากให้ถีบใส่ 0)
    public bool spawnOnlyOnce = true;   // ให้กับดักทำงานครั้งเดียวหรือไม่

    private bool hasSpawned = false;

    private void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าคนเหยียบคือ Player หรือไม่
        if (other.CompareTag("Player"))
        {
            if (spawnOnlyOnce && hasSpawned) return;

            SpawnTrap();
            hasSpawned = true;
        }
    }

    void SpawnTrap()
    {
        if (objectToSpawn != null && spawnPoint != null)
        {
            // 1. สร้างวัตถุออกมาที่จุดเกิด
            GameObject spawnedObj = Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);

            // 2. ตรวจสอบว่าวัตถุมี Rigidbody หรือไม่ (เพื่อให้มันหล่นได้)
            Rigidbody rb = spawnedObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // ปิด Kinematic เพื่อให้ฟิสิกส์ทำงาน
                rb.isKinematic = false;
                rb.useGravity = true;

                // เพิ่มแรงถีบลงข้างล่าง (Optional)
                rb.AddForce(Vector3.down * fallForce, ForceMode.Impulse);
            }
            else
            {
                Debug.LogWarning("Prefab ที่เอามาใส่ไม่มี Rigidbody นะครับ มันจะไม่หล่น!");
            }
        }
    }
}
