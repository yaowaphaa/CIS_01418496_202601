using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject floorPrefab;   // ลาก Floor prefab มาใส่ตรงนี้
    public Transform player;         // ลาก Player มาใส่

    public int tileCount = 5;
    public float tileLength = 20f;

    private float spawnX = 0;

    void Start()
    {
        for (int i = 0; i < tileCount; i++)
        {
            SpawnFloor();
        }
    }

    void Update()
    {
        if (player.position.x > spawnX - (tileCount * tileLength))
        {
            SpawnFloor();
        }
    }

    void SpawnFloor()
    {
        Instantiate(floorPrefab, new Vector3(spawnX, 0, 0), Quaternion.identity);
        spawnX += tileLength;
    }
}