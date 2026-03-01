using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public GameObject floorPrefab;
    public Transform player;

    public int tileCount = 5;
    public float tileLength = 240f;   // ใส่ค่าจริงของคุณ
    public float deleteDistance = 200f; // ระยะที่จะลบแมพข้างหลัง

    private float spawnX = 0;
    private List<GameObject> activeTiles = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < tileCount; i++)
        {
            SpawnFloor();
        }
    }

    void Update()
    {
        // สร้างแมพข้างหน้า
        if (player.position.x > spawnX - (tileCount * tileLength))
        {
            SpawnFloor();
        }

        // ลบแมพข้างหลัง
        DeleteOldTiles();
    }

    void SpawnFloor()
    {
        GameObject tile = Instantiate(
            floorPrefab,
            new Vector3(spawnX, 0, 0),
            Quaternion.identity
        );

        activeTiles.Add(tile);
        spawnX += tileLength;
    }

    void DeleteOldTiles()
    {
        if (activeTiles.Count == 0) return;

        GameObject firstTile = activeTiles[0];

        if (player.position.x - firstTile.transform.position.x > deleteDistance)
        {
            Destroy(firstTile);
            activeTiles.RemoveAt(0);
        }
    }
}