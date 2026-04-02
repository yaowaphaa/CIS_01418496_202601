using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    public int hpoint = 3;

    [Header("Scene Settings")]
    public string lobbySceneName = "Lobby"; 

    public void DecreaseHPoint(int amount)
    {
        hpoint -= amount;
        Debug.Log("แต้มลดลงเหลือ: " + hpoint);

        if (hpoint <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over! กำลังกลับไปที่หน้า Lobby...");
        SceneManager.LoadScene(lobbySceneName);
    }
}
