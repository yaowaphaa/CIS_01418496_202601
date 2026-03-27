using UnityEngine;

public class SmartPortal : MonoBehaviour
{
    public void ActivatePortal(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
        gameObject.SetActive(true);
    }
}