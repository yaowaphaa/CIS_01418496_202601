using UnityEngine;

public class CrystalRotate : MonoBehaviour
{
    [SerializeField] int rotationSpeed = 1;
    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
