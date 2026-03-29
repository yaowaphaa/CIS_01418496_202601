using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public Transform player;

    public float spawnDistance = 20f;
    public int coinsPerRow = 5;
    public float spacing = 1.5f;
    public float spawnOffset = 10f;
    public float rowX;
    private float nextSpawnX;

    void Start()
    {
        nextSpawnX = player.position.x + spawnDistance;
    }

    void Update()
    {
        if (player.position.x + spawnDistance > nextSpawnX)
        {
            SpawnRow();
            nextSpawnX += spawnOffset;
        }
    }

    void SpawnRow()
    {
        float randomZ = Random.Range(-2f, 2f);

        for (int i = 0; i < coinsPerRow; i++)
        {
            Vector3 spawnPos = new Vector3(
                nextSpawnX + i * spacing,
                rowX,
                randomZ
            );

            Instantiate(coinPrefab, spawnPos, Quaternion.identity);
        }
    }
}