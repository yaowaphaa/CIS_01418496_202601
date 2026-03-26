using UnityEngine;
using System.Collections;

public class PlayerKnockback : MonoBehaviour
{
    private Renderer[] renderers;
    private Color[] originalColors;
    private Color[] originalEmissionColors;
    public float hitFlashDuration = 0.2f;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

        originalColors = new Color[renderers.Length];
        originalEmissionColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = new Material(renderers[i].material);

            originalColors[i] = renderers[i].material.color;

            if (renderers[i].material.HasProperty("_EmissionColor"))
            {
                originalEmissionColors[i] =
                    renderers[i].material.GetColor("_EmissionColor");
            }
        }
    }

    // 🔥 ชนมอนแล้วฆ่าเลย
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(999); // 💀 ตายทันที
                Debug.Log("ผู้เล่นชนมอน → มอนตาย!");
            }

            // (ถ้าอยากให้ตัวแดงตอนชน)
            StartCoroutine(FlashRed());
        }
    }

    // 🔴 เอฟเฟกต์ตัวแดง
    private IEnumerator FlashRed()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.EnableKeyword("_EMISSION");
            renderers[i].material.SetColor("_EmissionColor", Color.red * 2f);
        }

        yield return new WaitForSeconds(hitFlashDuration);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.DisableKeyword("_EMISSION");
        }
    }
}