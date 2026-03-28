using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public GameObject floorPrefab;
    public Transform player;
    public int tileCount = 5;
    public float tileLength = 240f;
    public float deleteDistance = 200f;

    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public int minEnemiesPerTile = 2;
    public int maxEnemiesPerTile = 5;
    public float minDistanceFromPlayer = 5f; // ห้าม spawn ใกล้ผู้เล่น
    public float minDistanceBetweenEnemies = 3f;

    private float spawnX = 0;
    private List<GameObject> activeTiles = new List<GameObject>();
    private List<Vector3> spawnedEnemies = new List<Vector3>();

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
        // สร้าง Tile
        GameObject tile = Instantiate(
            floorPrefab,
            new Vector3(spawnX, 0, 0),
            Quaternion.identity
        );

        activeTiles.Add(tile);

        // Spawn enemies บน Tile
        SpawnEnemiesOnTile(tile);

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

    void SpawnEnemiesOnTile(GameObject tile)
    {
        Collider tileCol = tile.GetComponent<Collider>();
        if (tileCol == null)
        {
            Debug.LogWarning("Tile ไม่มี Collider!");
            return;
        }

        int enemyCount = Random.Range(minEnemiesPerTile, maxEnemiesPerTile + 1);

        for (int i = 0; i < enemyCount; i++)
        {
            int attempts = 0;
            while (attempts < 50) // ลองหาตำแหน่ง spawn ไม่เกิน 50 ครั้ง
            {
                float x = Random.Range(tileCol.bounds.min.x, tileCol.bounds.max.x);
                float z = Random.Range(tileCol.bounds.min.z, tileCol.bounds.max.z);
                Vector3 spawnPos = new Vector3(x, tileCol.bounds.max.y + 1f, z);

                // Raycast ลงพื้น
                if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, 50f))
                {
                    spawnPos.y = hit.point.y;

                    // ตรวจระยะจากผู้เล่น
                    if (Vector3.Distance(player.position, spawnPos) < minDistanceFromPlayer)
                    {
                        attempts++;
                        continue;
                    }

                    // ตรวจระยะจากศัตรูตัวอื่น
                    bool tooClose = false;
                    foreach (var pos in spawnedEnemies)
                    {
                        if (Vector3.Distance(pos, spawnPos) < minDistanceBetweenEnemies)
                        {
                            tooClose = true;
                            break;
                        }
                    }
                    if (tooClose)
                    {
                        attempts++;
                        continue;
                    }

                    // spawn ศัตรู
                    GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                    spawnedEnemies.Add(spawnPos);
                    break;
                }
                else
                {
                    attempts++;
                }
            }
        }
    }
}