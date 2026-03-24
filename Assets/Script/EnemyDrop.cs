using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [System.Serializable]
    public class DropItem
    {
        public GameObject itemPrefab;
        [Range(0f, 100f)]
        public float dropChance = 100f;
    }

    public DropItem[] dropItems;
    public int minDrop = 1;
    public int maxDrop = 15;

    public void Drop()
    {
        if (dropItems.Length == 0) return;

        int dropCount = Random.Range(minDrop, maxDrop + 1);

        for (int i = 0; i < dropCount; i++)
        {
            DropItem item = dropItems[Random.Range(0, dropItems.Length)];
            float rand = Random.Range(0f, 100f);

            if (rand <= item.dropChance && item.itemPrefab != null)
            {
                // 🔹 เพิ่ม offset ให้แต่ละก้อนไม่ทับกัน
                Vector3 offset = new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    0,
                    Random.Range(-0.5f, 0.5f)
                );

                GameObject dropped = Instantiate(
                    item.itemPrefab,
                    transform.position + offset,
                    Quaternion.identity
                );

                // 🔹 ใส่แรงเด้ง
                Rigidbody rb = dropped.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 randomDir = new Vector3(
                        Random.Range(-1f, 1f),
                        1f,
                        Random.Range(-1f, 1f)
                    );
                    rb.AddForce(randomDir.normalized * 5f, ForceMode.Impulse);
                }

                // 🔹 ใส่ MagnetItem ให้ดูดเข้าผู้เล่น
                if (dropped.GetComponent<MagnetItem>() == null)
                    dropped.AddComponent<MagnetItem>();
            }
        }
    }

    public void Die()
    {
        Drop();
        Destroy(gameObject);
    }
}