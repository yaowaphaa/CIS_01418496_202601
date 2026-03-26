using UnityEngine;
using UnityEngine.SceneManagement; // สำคัญ: ต้องเพิ่มบรรทัดนี้เพื่อใช้คำสั่งเปลี่ยน Scene

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    public int hpoint = 3;

    [Header("Scene Settings")]
    public string lobbySceneName = "Lobby"; // ชื่อของ Scene ที่ต้องการให้ไป (พิมพ์ให้ตรงกับใน Unity)

    public void DecreaseHPoint(int amount)
    {
        hpoint -= amount;
        Debug.Log("แต้มลดลงเหลือ: " + hpoint);

        // ถ้า hpoint น้อยกว่าหรือเท่ากับ 0 ให้เปลี่ยน Scene
        if (hpoint <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over! กำลังกลับไปที่หน้า Lobby...");

        // คำสั่งเปลี่ยนไป Scene ที่ระบุชื่อไว้
        SceneManager.LoadScene(lobbySceneName);
    }
}
