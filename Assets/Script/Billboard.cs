using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public Transform playerCamera; // Transform ของ Camera ผู้เล่น

    void LateUpdate()
    {
        if (playerCamera != null)
        {
            // หันหน้าไปที่กล้อง
            transform.LookAt(playerCamera.position);

            // หมุน 180° ถ้ารูปกลับด้าน
            transform.Rotate(0,-90f, 25f);
        }
    }
}