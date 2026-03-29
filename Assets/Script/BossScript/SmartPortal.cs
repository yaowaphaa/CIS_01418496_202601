using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SmartPortal : MonoBehaviour
{
    [Header("--- Follow Player ---")]
    public Transform player;
    public float distance = 15f;

    [Header("--- References ---")]
    public GameObject door;
    public string nextScene;

    [Header("--- Boss Trigger ---")]
    public BossHealth boss;

    private Vector3 savedTargetScale;
    private bool isSpawned = false;
    private float fixedZ;

    void Awake()
    {
        if (door != null)
        {
            savedTargetScale = door.transform.localScale;
            door.transform.localScale = Vector3.zero;
            fixedZ = door.transform.position.z;
        }
    }

    void Start()
    {
        if (boss != null)
        {
            boss.OnTriggerHP += SpawnNow;
        }
    }

    void Update()
    {
        if (door != null && player != null && !isSpawned)
        {
            door.transform.position = new Vector3(
                player.position.x + distance,
                door.transform.position.y,
                fixedZ
            );
        }
    }

    IEnumerator SpawnRoutine()
    {
        float t = 0f;
        float duration = 0.8f;

        Quaternion startRot = door.transform.rotation;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;

            float scale = Mathf.Sin(p * Mathf.PI * 0.5f);
            door.transform.localScale = savedTargetScale * scale;

            float rotY = Mathf.Lerp(0f, 360f, p);
            door.transform.rotation = startRot * Quaternion.Euler(0f, rotY, 0f);

            yield return null;
        }

        // เด้งตอนจบ
        door.transform.localScale = savedTargetScale * 1.4f;
        yield return new WaitForSeconds(0.05f);
        door.transform.localScale = savedTargetScale;

        door.transform.rotation = startRot;

        isSpawned = true;
    }

    void SpawnNow()
    {
        if (!isSpawned)
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isSpawned && other.CompareTag("Player"))
        {
            string scene = SceneManager.GetActiveScene().name;
            if (scene == "BossScene")  GameProgress.ClearBoss(0);
            else if (scene == "BossScene1") GameProgress.ClearBoss(1);
            else if (scene == "BossScene2") GameProgress.ClearBoss(2);

            SceneManager.LoadScene(nextScene);
        }
    }
}