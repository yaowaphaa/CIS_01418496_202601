using UnityEngine;
using System.Collections;

public class PlayerRise : MonoBehaviour
{
    public float impactCount = 3;
    public float preImpactBounce = 0.2f;
    public float preImpactSpeed = 5f;
    public float impactSpeed = 12f;
    public float undergroundY = -1f;

    public bool IsRising { get; private set; } = true;

    private Vector3 startPosition;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        startPosition = transform.position;
        transform.position = new Vector3(startPosition.x, undergroundY, startPosition.z);

        StartCoroutine(RiseUp());
    }

    IEnumerator RiseUp()
    {
        Vector3 basePos = transform.position;

        for (int i = 0; i < impactCount; i++)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * preImpactSpeed;
                transform.position = basePos + Vector3.up * Mathf.Sin(t * Mathf.PI) * preImpactBounce;
                yield return null;
            }

            transform.position = basePos;
            yield return new WaitForSeconds(0.05f);
        }

        while (Mathf.Abs(transform.position.y - startPosition.y) > 0.001f)
        {
            float newY = Mathf.MoveTowards(
                transform.position.y,
                startPosition.y,
                impactSpeed * Time.deltaTime
            );

            transform.position = new Vector3(
                startPosition.x,
                newY,
                startPosition.z
            );

            yield return null;
        }

        transform.position = startPosition;
        IsRising = false;

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
    }
}