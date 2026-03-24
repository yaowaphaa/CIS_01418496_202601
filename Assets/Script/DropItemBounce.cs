using UnityEngine;

public class DropItemBounce : MonoBehaviour
{
    public float forceUp = 5f;
    public float forceSide = 2f;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 randomDir = new Vector3(
                Random.Range(-1f, 1f),
                1f,
                Random.Range(-1f, 1f)
            );

            rb.AddForce(randomDir.normalized * forceUp, ForceMode.Impulse);
        }
    }
}