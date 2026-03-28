using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PreBossPortal : MonoBehaviour
{
    [Header("--- Follow Player ---")]
    public Transform player;
    public float distance = 15f; // ระยะหน้าผู้เล่น

    [Header("--- Spawn Timing ---")]
    public float timeToWait = 10f;
    public float scaleSpeed = 5f;

    [Header("--- References ---")]
    public GameObject door;
    public string nextScene;

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
        StartCoroutine(SpawnRoutine());
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
        yield return new WaitForSeconds(timeToWait);

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

        // ⭐ เด้งตอนจบ
        door.transform.localScale = savedTargetScale * 1.4f;
        yield return new WaitForSeconds(0.05f);
        door.transform.localScale = savedTargetScale;

        // ⭐ กลับท่าเดิมเป๊ะ
        door.transform.rotation = startRot;

        isSpawned = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isSpawned && other.CompareTag("Player"))
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}